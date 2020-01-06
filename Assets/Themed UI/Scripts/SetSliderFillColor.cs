using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Themed_UI {
    /*
     * Themeable component for any UI game object containing a slider component.  Sets the tint of the colorbar
     * indicating "amount" selected on the slider according to current theme.
     */
    [DisallowMultipleComponent]
    public class SetSliderFillColor : MonoBehaviour, IThemeable {
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

            Image im = GetComponent<Image>();
#if UNITY_EDITOR
            Undo.RecordObject(im, "Changing Image Color");
#endif
            im.color = _themeSettings.sliderFillColor;
#if UNITY_EDITOR
            PrefabUtility.RecordPrefabInstancePropertyModifications(im);
#endif
        }
    }
}
