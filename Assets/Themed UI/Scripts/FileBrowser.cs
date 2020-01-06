using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Themed_UI {
    public class FileBrowser : MonoBehaviour {
        [Tooltip("Name of file to be selected, or leave blank")]
        [SerializeField] public string selectedFile;
        [Tooltip("Path, relative to Application.persistentDataPath")]
        [SerializeField] public string path;
        [Tooltip("Text label for 'confirm' button")]
        [SerializeField] public string confirmText = "Confirm";
        [Tooltip("File items to put in list even if not actual files in directory")]
        [SerializeField] public string[] createIfNotPresent;
        [Tooltip("If true, user can type new filenames not already present in list")]
        [SerializeField] public bool canTypeNew;
        [Tooltip("Prefab for individual file items in scrollable list")]
        [SerializeField] private GameObject itemPrefab;
        [Header("Set to prefab components")]
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private ScrollRect scrollview;
        [SerializeField] public Button confirmButton;
        [SerializeField] public Button cancelButton;

        private string _path;
        private List<String> _files;
        private List<String> _filenames = new List<string>();

        public void ResetBrowser() {
            _filenames.Clear();

            _path = Path.Combine(Application.persistentDataPath, path);
            Debug.Log(_path);
            if(!Directory.Exists(_path)) {
                Directory.CreateDirectory(_path);
            }

            _files = Directory.EnumerateFiles(_path).ToList();

            foreach(string file in _files) {
                _filenames.Add(Path.GetFileNameWithoutExtension(file));
            }

            _filenames.Sort();

            int insertIndex = 0;
            foreach(string toAdd in createIfNotPresent) {
                if(!_filenames.Contains(toAdd)) {
                    _filenames.Insert(insertIndex, toAdd);
                    insertIndex++;
                }
            }

            RectTransform content = scrollview.content;

            foreach(Transform child in scrollview.content) {
                Destroy(child.gameObject);
            }

            content.sizeDelta = new Vector2(content.sizeDelta.x, 100*_filenames.Count);


            foreach(string fileName in _filenames) {
                GameObject button = Instantiate(itemPrefab, scrollview.content);
                TextMeshProUGUI textmesh = button.GetComponentInChildren<TextMeshProUGUI>();
                textmesh.text = ToDisplayName(fileName);
                textmesh.GetComponent<SetWidgetColor>().colorTag = SetWidgetColor.ColorTag.Blue;
                button.GetComponentInChildren<Button>().onClick.AddListener(SelectFile);
            }

            FindObjectOfType<Theme>().Refresh();
            confirmButton.interactable = !string.IsNullOrEmpty(selectedFile);
            confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = confirmText;
            if(confirmButton.interactable) {
                inputField.text = ToDisplayName(selectedFile);
            }
        }

        private void Start() {
            ResetBrowser();
        }

        private void SelectFile() {
            string filename = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TextMeshProUGUI>()
                .text;
            inputField.SetTextWithoutNotify(filename);
            selectedFile = ToFileName(filename);
            confirmButton.interactable = !string.IsNullOrEmpty(selectedFile);
        }

        public void SelectFileFromInputField() {
            string filename = ToFileName(inputField.text);
            if(canTypeNew || _filenames.Contains(filename)) {
                selectedFile = filename;
            }
            else {
                inputField.text = ToDisplayName(selectedFile);
            }

            confirmButton.interactable = !string.IsNullOrEmpty(selectedFile);
        }

        private string ToDisplayName(string fileName) {
            string result = "";
            foreach(char c in fileName) {
                if(c == '_') {
                    result += "*";
                }
                else {
                    result += c;
                }
            }

            return result;
        }

        private string ToFileName(string displayName) {
            string result = "";
            foreach(char c in displayName) {
                if(c == '*') {
                    result += "_";
                }
                else {
                    result += c;
                }
            }

            return result;
        }
    }
}
