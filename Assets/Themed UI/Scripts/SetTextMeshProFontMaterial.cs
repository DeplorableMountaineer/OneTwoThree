using System;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Themed_UI {
    /*
     * Themeable component for any UI game object containing a TextMeshProUGUI component.  Sets the base font material
     * according to the current theme.
     */
    [DisallowMultipleComponent]
    public class SetTextMeshProFontMaterial : MonoBehaviour, IThemeable {
        [Tooltip("Font material to use")]
        [SerializeField] public FontMaterialSelection fontMaterialSelection = FontMaterialSelection.Default;
        private Theme _theme;
        private ThemeSettings _themeSettings;

        // Start is called before the first frame update
        void Start() {
            ApplyTheme();
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

            TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();
#if UNITY_EDITOR
            Undo.RecordObject(tmp, "Changing Text Mesh Pro Font Material");
#endif
            switch(fontMaterialSelection) {
                case FontMaterialSelection.Default:
                    tmp.fontSharedMaterial = _themeSettings.defaultFontMaterial;
                    break;
                case FontMaterialSelection.Alt1OrGreen:
                    tmp.fontSharedMaterial = _themeSettings.fontMaterialAlt1OrGreen;
                    break;
                case FontMaterialSelection.Alt2OrBlue:
                    tmp.fontSharedMaterial = _themeSettings.fontMaterialAlt2OrBlue;
                    break;
                case FontMaterialSelection.Alt3OrHighlighted:
                    tmp.fontSharedMaterial = _themeSettings.fontMaterialAlt3OrHighlighted;
                    break;
                case FontMaterialSelection.Alt4OrCaution:
                    tmp.fontSharedMaterial = _themeSettings.fontMaterialAlt4OrCaution;
                    break;
                case FontMaterialSelection.Alt5OrDanger:
                    tmp.fontSharedMaterial = _themeSettings.fontMaterialAlt5OrDanger;
                    break;
                case FontMaterialSelection.Alt6OrExtremeDanger:
                    tmp.fontSharedMaterial = _themeSettings.fontMaterialAlt6OrExtremeDanger;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
#if UNITY_EDITOR
            PrefabUtility.RecordPrefabInstancePropertyModifications(tmp);
#endif
        }

        public enum FontMaterialSelection {
            Default,
            Alt1OrGreen,
            Alt2OrBlue,
            Alt3OrHighlighted,
            Alt4OrCaution,
            Alt5OrDanger,
            Alt6OrExtremeDanger
        }
    }
}
