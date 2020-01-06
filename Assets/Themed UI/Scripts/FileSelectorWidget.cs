using TMPro;
using UnityEngine;

namespace Themed_UI {
    public class FileSelectorWidget : MonoBehaviour {
        [Header("Set to popup prefab")]
        [SerializeField] private GameObject fileSelectorPrefab;
        
        [Header("File Browser Popup Settings, copied to file browser popup")]
        [Tooltip("Name of file to be selected, or leave blank")]
        [SerializeField] public string selectedFile;
        [Tooltip("Path, relative to Application.persistentDataPath")]
        [SerializeField] public string path;
        [Tooltip("Text label for 'confirm' button")]
        [SerializeField] private string confirmText = "Confirm";
        [Tooltip("File items to put in list even if not actual files in directory")]
        [SerializeField] private string[] createIfNotPresent;
        [Tooltip("If true, user can type new filenames not already present in list")]
        [SerializeField] private bool canTypeNew ;
        
        private GameObject _popup;

        private void Start() {
            GetComponentInChildren<TMP_InputField>().text = selectedFile;
            GetComponentInChildren<TMP_InputField>().interactable = canTypeNew;
        }

        public void OnClick() {
            _popup = Instantiate(fileSelectorPrefab, FindObjectOfType<Theme>().transform);
            FileBrowser fb = _popup.GetComponent<FileBrowser>();
            fb.selectedFile = GetComponentInChildren<TMP_InputField>().text;
            fb.path = path;
            fb.confirmText = confirmText;
            fb.createIfNotPresent = createIfNotPresent;
            fb.canTypeNew = canTypeNew;
            fb.ResetBrowser();
            fb.confirmButton.onClick.AddListener(SetFile);
            fb.cancelButton.onClick.AddListener(Cancel);
            FindObjectOfType<Theme>().Refresh();
        }

        public void SetFile() {
            GetComponentInChildren<TMP_InputField>().text = _popup.GetComponent<FileBrowser>().selectedFile;
            Destroy(_popup);
        }

        public void Cancel() {
            Destroy(_popup);
        }

        public string GetFile() {
            return GetComponentInChildren<TMP_InputField>().text;
        }
    }
}
