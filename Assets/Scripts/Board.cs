using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// ReSharper disable StringIndexOfIsCultureSpecific.1
public enum MoveState {
    Selecting,
    Swapping,
    Deleting,
    Falling,
    Filling,
    GameOver,
    ReadyToRestart
}

public class Board : MonoBehaviour {
    [SerializeField] private int numRows = 10;
    [SerializeField] private int numCols = 10;
    [SerializeField] private GameObject emptyPrefab;
    [SerializeField] private Color highlighedColor;
    [SerializeField] private float slideTime = 0.2f;
    [SerializeField] private float[] levelUpAmounts;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private GameObject scoreAdderPrefab;
    [SerializeField] private AudioClip beep;
    [SerializeField] private GameObject gameOver;

    private Dictionary<Sprite, int> _blocks = new Dictionary<Sprite, int>();
    private Sprite[] _keys;
    private int _selectedX;
    private int _selectedY;
    private GameObject _selectedTile;
    private GameObject _previousSelectedTile;
    private float _highScore;
    private MoveState _moveState = MoveState.Selecting;
    private GameObject[,] _grid = null;
    private GameObject[] _rows = null;

    private int _numTilesMoving = 0;
    private float[] _tileWeights = new float[] {1, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    private float _score = 0;
    private Timer _timer;
    private int _level = 0;

    private bool _buildingBoard = true;
    private AudioSource _audioSource;

    private const string HighScoreKey = "High Score";

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
        _timer = FindObjectOfType<Timer>();
    }

    private void Start() {
        if(PlayerPrefs.HasKey(HighScoreKey)) {
            _highScore = PlayerPrefs.GetFloat(HighScoreKey);
        }

        scoreText.text = "0";
        highScoreText.text = _highScore.ToString();
        Resources.LoadAll<Sprite>("Sprites");
        foreach(Sprite s in Resources.FindObjectsOfTypeAll<Sprite>()) {
            if(s.name.Contains("NumberGrid_")) {
                int num = int.Parse(s.name.Substring(1 + s.name.IndexOf("_")));
                _blocks[s] = num;
            }
        }

        _keys = _blocks.Keys.ToArray();
        GenerateBoard();
        StartCoroutine(FixBoard());
    }

    private void OnApplicationQuit() {
        PlayerPrefs.SetFloat(HighScoreKey, _highScore);
        PlayerPrefs.Save();
    }

    private void OnMouseDown() {
        if(_moveState == MoveState.ReadyToRestart) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if(_moveState != MoveState.Selecting) return;
        Debug.Assert(Camera.main != null, "Camera.main != null");
        Vector2 selected = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _selectedX = Mathf.RoundToInt(selected.x);
        _selectedY = Mathf.RoundToInt(selected.y);
        SelectTile(FindSelectedTile());
        if(_selectedTile && _previousSelectedTile) {
            _moveState = MoveState.Swapping;
            bool swapped = SwapIfAdjacent(_selectedTile, _previousSelectedTile);
            DeselectTile(_selectedTile);
            _selectedTile = null;
            DeselectTile(_previousSelectedTile);
            _previousSelectedTile = null;
            if(swapped) {
                StartCoroutine(FixBoard());
            }
            else {
                _moveState = MoveState.Selecting;
            }
        }
    }

    private IEnumerator FixBoard() {
        while(true) {
            yield return new WaitUntil(FinishedMoving);
            _moveState = MoveState.Deleting;
            HashSet<GameObject> matches = ComputeMatches();
            if(matches.Count == 0) break;
            float bonus = 0;
            float bonusIncrement = 1;
            float scoreAdder = 0;
            foreach(GameObject match in matches) {
                float ds = 1 + _blocks[match.GetComponent<SpriteRenderer>().sprite]%10;
                scoreAdder += ds;
                bonus++;
            }

            if(bonus > 4) {
                bonus *= 2;
                if(bonus > 13) {
                    bonus *= 2;
                    if(bonus >= 40) {
                        bonus *= 2;
                    }
                }
            }

            if(!_buildingBoard) {
                _score += (bonus - 2)*scoreAdder;
                StartCoroutine(SpawnScore((bonus - 2)*scoreAdder));
                int addToTime = Mathf.Min(120, Mathf.CeilToInt((bonus - 2)*scoreAdder));
                _timer.AddSeconds(addToTime);
                _audioSource.pitch = Mathf.Pow(2f, (((float)addToTime) - 30f)/60f);
                _audioSource.Play();

                if(_score > levelUpAmounts[_level]) {
                    LevelUp();
                }

                scoreText.text = _score.ToString();
                if(_score > _highScore) {
                    _highScore = _score;
                    highScoreText.text = _highScore.ToString();
                }
            }

            yield return DestroyMatches(matches);
            _moveState = MoveState.Falling;
            yield return Fall();
            _moveState = MoveState.Filling;
            yield return FillHoles();
        }

        _moveState = MoveState.Selecting;
        _buildingBoard = false;
    }

    private IEnumerator SpawnScore(float amount) {
        GameObject go = Instantiate(scoreAdderPrefab, FindObjectOfType<Canvas>().transform);
        go.GetComponentInChildren<TextMeshProUGUI>().text = "+" + amount.ToString();
        yield return new WaitForSeconds(1.5f);
        Destroy(go);
    }

    private void LevelUp() {
        _level++;
        Debug.Log("Level up! " + (_level + 1));

        float totalWeight = 0;
        for(int i = _tileWeights.Length - 1; i > 0; i--) {
            _tileWeights[i] += .3f*_tileWeights[i - 1];
        }

        for(int i = 0; i < _tileWeights.Length; i++) {
            totalWeight += _tileWeights[i];
        }

        for(int i = 0; i < _tileWeights.Length; i++) {
            _tileWeights[i] /= totalWeight;
        }
    }

    private bool FinishedMoving() {
        return _numTilesMoving == 0;
    }

    private bool SwapIfAdjacent(GameObject selectedTile, GameObject previousSelectedTile) {
        Vector2 position = selectedTile.transform.position;
        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        position = previousSelectedTile.transform.position;
        int x2 = Mathf.RoundToInt(position.x);
        int y2 = Mathf.RoundToInt(position.y);
        if(x == x2 && Mathf.Abs(y - y2) == 1) {
            Swap(selectedTile, previousSelectedTile);
            return true;
        }

        if(y == y2 && Mathf.Abs(x - x2) == 1) {
            Swap(selectedTile, previousSelectedTile);
            return true;
        }

        return false;
    }

    private void Swap(GameObject selectedTile, GameObject previousSelectedTile) {
        Vector2 pos1 = previousSelectedTile.transform.position;
        Vector2 pos2 = selectedTile.transform.position;
        _moveState = MoveState.Swapping;
        StartCoroutine(Move(selectedTile, pos1, slideTime));
        StartCoroutine(Move(previousSelectedTile, pos2, slideTime));
    }

    IEnumerator Move(GameObject go, Vector2 where, float time) {
        _numTilesMoving++;
        Vector2 position = go.transform.position;
        float dx = (where.x - position.x)/time*1f/60f;
        float dy = (where.y - position.y)/time*1f/60f;
        if(Math.Abs(dx) < .001) {
            while(Mathf.Abs(go.transform.position.y - where.y) >= Mathf.Abs(dy)) {
                go.transform.Translate(0, dy, 0);
                yield return null;
            }
        }
        else {
            while(Mathf.Abs(go.transform.position.x - where.x) >= Mathf.Abs(dx)) {
                go.transform.Translate(dx, 0, 0);
                yield return null;
            }
        }

        go.transform.position = where;
        _numTilesMoving--;
    }

    private IEnumerator FillHoles() {
        yield return null;
        List<Vector2Int> modified = new List<Vector2Int>();
        for(int row = numRows - 1; row >= 0; row--) {
            for(int col = numCols - 1; col >= 0; col--) {
                if(_grid[col, row] == null) {
                    _grid[col, row] = CreateTile(transform, col, row);
                    modified.Add(new Vector2Int(col, row));
                }
            }
        }

        while(modified.Count > 2 && !IsViable()) {
            AdjustRandomTile(modified);
        }
    }

    private void AdjustRandomTile(List<Vector2Int> modified = null) {
        int row = Random.Range(0, numRows);
        int col = Random.Range(0, numCols);
        Sprite s = _grid[col, row].GetComponent<SpriteRenderer>().sprite;
        if(modified == null) {
            row = Random.Range(0, numRows);
            col = Random.Range(0, numCols);
        }
        else {
            int index = Random.Range(0, modified.Count);
            col = modified[index].x;
            row = modified[index].y;
        }

        _grid[col, row].GetComponent<SpriteRenderer>().sprite = s;
    }

    private bool IsViable() {
        Dictionary<Sprite, int> counts = new Dictionary<Sprite, int>();
        foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
            if(counts.ContainsKey(sr.sprite)) {
                counts[sr.sprite]++;
                if(counts[sr.sprite] >= 3) return true;
            }
            else {
                counts[sr.sprite] = 1;
            }
        }

        return false;
    }

    private void SelectTile(GameObject tile) {
        if(!tile) {
            DeselectTile(_selectedTile);
            _selectedTile = null;
            DeselectTile(_previousSelectedTile);
            _previousSelectedTile = null;
        }
        else if(tile == _selectedTile) {
            DeselectTile(_selectedTile);
            _selectedTile = _previousSelectedTile;
            _previousSelectedTile = null;
        }
        else if(tile == _previousSelectedTile) {
            DeselectTile(_previousSelectedTile);
            _previousSelectedTile = null;
        }
        else {
            if(_previousSelectedTile) {
                DeselectTile(_previousSelectedTile);
            }

            _previousSelectedTile = _selectedTile;
            _selectedTile = tile;
            _selectedTile.GetComponent<SpriteRenderer>().color = highlighedColor;
        }
    }

    private void DeselectTile(GameObject tile) {
        tile.GetComponent<SpriteRenderer>().color = Color.white;
    }

    private GameObject FindSelectedTile() {
        foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
            Vector2 position = sr.transform.position;
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);
            if(x == _selectedX && y == _selectedY) {
                return sr.gameObject;
            }
        }

        return null;
    }

    public void GenerateBoard() {
        if(_grid == null) {
            _grid = new GameObject[numCols, numRows];
        }

        if(_rows == null) {
            _rows = new GameObject[numRows];
            for(int row = 0; row < numRows; row++) {
                _rows[row] = Instantiate(emptyPrefab, transform);
                _rows[row].name = "Row_" + row.ToString();
            }
        }

        for(int row = 0; row < numRows; row++) {
            for(int col = 0; col < numCols; col++) {
                _grid[col, row] = CreateTile(_rows[row].transform, col, row);
            }
        }

        while(!IsViable()) {
            AdjustRandomTile();
        }
    }

    private GameObject CreateTile(Transform parent, int col, int row) {
        GameObject tile = Instantiate(emptyPrefab, parent);
        SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();

        Sprite block = RandomBlock();
        sr.sprite = block;
        tile.name = _blocks[block].ToString();
        tile.transform.position = GridToPos(col, row);
        return tile;
    }

    private Sprite RandomBlock() {
        Sprite key = null;
        int targetNum = RandomIndex();

        while(key == null || _blocks[key]%10 != targetNum) {
            key = _keys[Random.Range(0, _keys.Length)];
        }

        return key;
    }

    private int RandomIndex() {
        float r = Random.value;
        float c = 0;
        for(int i = 0; i < _tileWeights.Length; i++) {
            c += _tileWeights[i];
            if(r <= c) {
                return i;
            }
        }

        return _tileWeights.Length - 1;
    }

    IEnumerator DestroyMatches(HashSet<GameObject> matches) {
        yield return null;
        foreach(GameObject match in matches) {
            Destroy(match);
            Vector3 position = match.transform.position;
            int x, y;
            PosToGrid(position, out x, out y);
            _grid[x, y] = null;
        }
    }

    IEnumerator Fall() {
        yield return null;
        for(int row = 0; row < numRows; row++) {
            for(int col = numCols - 1; col >= 0; col--) {
                if(_grid[col, row] == null) {
                    for(int i = row + 1; i < numRows; i++) {
                        if(_grid[col, i] != null) {
                            StartCoroutine(Move(_grid[col, i], GridToPos(col, row), (i - row)*slideTime));
                            _grid[col, row] = _grid[col, i];
                            _grid[col, i] = null;
                            break;
                        }
                    }
                }
            }
        }

        yield return new WaitUntil(FinishedMoving);
    }

    private HashSet<GameObject> ComputeMatches() {
        HashSet<GameObject> matches = new HashSet<GameObject>();
        ComputeGrid();

        //Horizontal Matches
        for(int row = 0; row < numRows; row++) {
            for(int col = 0; col < numCols - 2; col++) {
                if(_grid[col, row].GetComponent<SpriteRenderer>().sprite ==
                   _grid[col + 1, row].GetComponent<SpriteRenderer>().sprite
                   && _grid[col, row].GetComponent<SpriteRenderer>().sprite ==
                   _grid[col + 2, row].GetComponent<SpriteRenderer>().sprite) {
                    matches.Add(_grid[col, row]);
                    matches.Add(_grid[col + 1, row]);
                    matches.Add(_grid[col + 2, row]);
                }
            }
        }

        //Vertical Matches
        for(int col = 0; col < numCols; col++) {
            for(int row = 0; row < numRows - 2; row++) {
                if(_grid[col, row].GetComponent<SpriteRenderer>().sprite ==
                   _grid[col, row + 1].GetComponent<SpriteRenderer>().sprite
                   && _grid[col, row].GetComponent<SpriteRenderer>().sprite ==
                   _grid[col, row + 2].GetComponent<SpriteRenderer>().sprite) {
                    matches.Add(_grid[col, row]);
                    matches.Add(_grid[col, row + 1]);
                    matches.Add(_grid[col, row + 2]);
                }
            }
        }

        foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
            sr.color = Color.white;
        }

        foreach(GameObject match in matches) {
            match.GetComponent<SpriteRenderer>().color = highlighedColor;
        }

        return matches;
    }

    private void ComputeGrid() {
        if(_grid == null) {
            _grid = new GameObject[numCols, numRows];
        }

        foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) {
            Vector3 position = sr.transform.position;
            int x, y;
            PosToGrid(position, out x, out y);
            _grid[x, y] = sr.gameObject;
        }
    }

    private void PosToGrid(Vector2 position, out int x, out int y) {
        x = Mathf.RoundToInt(position.x + numCols*.5f);
        y = Mathf.RoundToInt(position.y + numRows*.5f);
    }

    private Vector2 GridToPos(int x, int y) {
        return new Vector2(x - numCols*.5f, y - numRows*.5f);
    }

    public void GameOver() {
        _moveState = MoveState.GameOver;
        _timer.enabled = false;
        PlayerPrefs.SetFloat(HighScoreKey, _highScore);
        PlayerPrefs.Save();
        StartCoroutine(NewGame());
    }

    public IEnumerator NewGame() {
        yield return new WaitForSeconds(1);
        GetComponent<BoxCollider2D>().size = new Vector2(25, 15);
        gameOver.SetActive(true);
        yield return null;
        _moveState = MoveState.ReadyToRestart;
    }
}
