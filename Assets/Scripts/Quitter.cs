using UnityEngine;

public class Quitter : MonoBehaviour {
    private void OnMouseDown() {
        Vector2 selected = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int selectedX = Mathf.RoundToInt(selected.x);

        if(selectedX > 8) {
            Debug.Log("Quit");
            Application.Quit();
            FindObjectOfType<Board>().GameOver();
        }
    }
}
