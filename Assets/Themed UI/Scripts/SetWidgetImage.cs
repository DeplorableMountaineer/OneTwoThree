using System;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Themed_UI {
    /*
     * Themeable component for any UI game object containing an image component.  Sets the sprite
     * according to current theme and widget type.
     */
    [DisallowMultipleComponent]
    public class SetWidgetImage : MonoBehaviour, IThemeable {
        [Tooltip("Select which image to use")]
        [SerializeField] private WidgetComponentType widgetComponentType = WidgetComponentType.Other;
        private Theme _theme;
        private ThemeSettings _themeSettings;


        // Start is called before the first frame update
        void Start() {
            ApplyTheme();
        }

        private void SetWidgetComponentImage(Image im, WidgetComponentType wct) {
            switch(wct) {
                case WidgetComponentType.Other:
                    break;
                case WidgetComponentType.Button:
                    im.sprite = _themeSettings.buttonImage;
                    break;
                case WidgetComponentType.SliderBackground:
                    im.sprite = _themeSettings.sliderBackgroundImage;
                    break;
                case WidgetComponentType.SliderHandle:
                    im.sprite = _themeSettings.sliderHandleImage;
                    break;
                case WidgetComponentType.Panel:
                    im.sprite = _themeSettings.panelImage;
                    break;
                case WidgetComponentType.ToggleBackground:
                    im.sprite = _themeSettings.toggleBackgroundImage;
                    break;
                case WidgetComponentType.ToggleCheck:
                    im.sprite = _themeSettings.toggleCheckImage;
                    break;
                case WidgetComponentType.TiledUiBackground:
                    im.sprite = _themeSettings.tiledBackgroundImage;
                    break;
                case WidgetComponentType.Dropdown:
                    im.sprite = _themeSettings.dropdownImage;
                    break;
                case WidgetComponentType.InputField:
                    im.sprite = _themeSettings.inputFieldImage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(wct), wct, null);
            }
        }

        [Serializable]
        public enum WidgetComponentType {
            Other,
            Button,
            SliderBackground,
            SliderHandle,
            Panel,
            ToggleBackground,
            ToggleCheck,
            TiledUiBackground,
            Dropdown,
            InputField
        }

        public void ApplyTheme() {
            _theme = GetComponentInParent<Theme>();
            if(!_theme) {
                return;
            }

            _themeSettings = _theme.theme;
            if(!_themeSettings) {
                return;
            }

            Image im = GetComponent<Image>();
            if(im) {
#if UNITY_EDITOR
                Undo.RecordObject(im, "Changing Image Sprite");
#endif
                SetWidgetComponentImage(im, widgetComponentType);
#if UNITY_EDITOR
                PrefabUtility.RecordPrefabInstancePropertyModifications(im);
#endif
            }
        }
    }
}
