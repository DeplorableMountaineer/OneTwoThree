using UnityEngine;
using UnityEngine.UI;

namespace Themed_UI {
    public class ColorWidget : MonoBehaviour {
        [Header("Set to popup prefab")]
        [SerializeField] private GameObject colorPickerPrefab;
        private GameObject _popup;

        public void OnClick() {
            _popup = Instantiate(colorPickerPrefab, FindObjectOfType<Theme>().transform);
            ColorPicker cp = _popup.GetComponent<ColorPicker>();
            cp.setButton.onClick.AddListener(SetColor);
            cp.cancelButton.onClick.AddListener(Cancel);
            cp.SetColor(GetComponentInChildren<Image>().color);
            FindObjectOfType<Theme>().Refresh();
        }

        public void SetColor() {
            GetComponentInChildren<Image>().color = _popup.GetComponent<ColorPicker>().previewImage.color;
            Destroy(_popup);
        }

        public void Cancel() {
            Destroy(_popup);
        }

        public Color GetColor() {
            return GetComponentInChildren<Image>().color;
        }
    }
}
