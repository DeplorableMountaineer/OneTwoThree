using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Themed_UI {
    /*
     * Themeable component for any widget object with a transition colorblock for tinting the UI element according to
     * whether it is selected, clicked, highlighted, or disabled.
     */
    [DisallowMultipleComponent]
    public class SetWidgetColorblock : MonoBehaviour, IThemeable {
        [Tooltip("If true, widget itself is invisible, but children still visible")]
        [SerializeField] private bool invisible = true;

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

            Selectable widget = GetComponent<Selectable>();
            if(widget) {
#if UNITY_EDITOR
                Undo.RecordObject(widget, "Changing Widget Colorblock");
#endif
                ColorBlock cb = widget.colors;
                ColorBlock newCb;
                if(invisible) {
                    newCb = _themeSettings.menuItemColors;
                }
                else {
                    newCb = _themeSettings.uiColors;
                }

                cb.normalColor = newCb.normalColor;
                cb.highlightedColor = newCb.highlightedColor;
                cb.selectedColor = newCb.selectedColor;
                cb.pressedColor = newCb.pressedColor;
                cb.disabledColor = newCb.disabledColor;
                cb.colorMultiplier = newCb.colorMultiplier;
                cb.fadeDuration = newCb.fadeDuration;
                widget.colors = cb;
#if UNITY_EDITOR
                PrefabUtility.RecordPrefabInstancePropertyModifications(widget);
#endif
            }
        }
    }
}
