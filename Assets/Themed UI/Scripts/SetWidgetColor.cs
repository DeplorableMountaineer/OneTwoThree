using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Themed_UI {
    /*
     * Themeable component for any UI game object containing text or an image that can be tinted.  Sets the tint color
     * according to current theme and color tag.
     */
    [DisallowMultipleComponent]
    public class SetWidgetColor : MonoBehaviour, IThemeable {
        [Tooltip("Color modification tag")]
        [SerializeField] public ColorTag colorTag = ColorTag.Unchanged;
        private Theme _theme;
        private ThemeSettings _themeSettings;
        private Dictionary<ColorTag, Color> _colorFromTag = new Dictionary<ColorTag, Color>();
        private bool _fontMaterialUsedAutomatically;

        // Start is called before the first frame update
        void Start() {
            ApplyTheme();
        }


        private void PopulateColorDictionary() {
            _colorFromTag[ColorTag.White] = _themeSettings.white;
            _colorFromTag[ColorTag.Blue] = _themeSettings.blue;
            _colorFromTag[ColorTag.Green] = _themeSettings.green;
            _colorFromTag[ColorTag.Gray] = _themeSettings.gray;
            _colorFromTag[ColorTag.Highlighted] = _themeSettings.highlightedColor;
            _colorFromTag[ColorTag.Caution] = _themeSettings.cautionColor;
            _colorFromTag[ColorTag.Danger] = _themeSettings.dangerColor;
            _colorFromTag[ColorTag.ExtremeDanger] = _themeSettings.extremeDangerColor;
        }

        [Serializable]
        public enum ColorTag {
            Unchanged,
            White,
            Blue,
            Green,
            Gray,
            Highlighted,
            Caution,
            Danger,
            ExtremeDanger
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

            if(colorTag != ColorTag.Unchanged) {
                PopulateColorDictionary();

                Image im = GetComponent<Image>();
                if(im) {
#if UNITY_EDITOR
                    Undo.RecordObject(im, "Changing Image Color");
#endif
                    im.color = _colorFromTag[colorTag];
#if UNITY_EDITOR
                    PrefabUtility.RecordPrefabInstancePropertyModifications(im);
#endif
                }

                Text text = GetComponent<Text>();
                if(text) {
#if UNITY_EDITOR
                    Undo.RecordObject(text, "Changing Text Color");
#endif
                    text.color = _colorFromTag[colorTag];
#if UNITY_EDITOR
                    PrefabUtility.RecordPrefabInstancePropertyModifications(text);
#endif
                }

                TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();
                if(tmp) {
#if UNITY_EDITOR
                    Undo.RecordObject(tmp, "Changing Text Color");
#endif
                    tmp.color = _colorFromTag[colorTag];
#if UNITY_EDITOR
                    PrefabUtility.RecordPrefabInstancePropertyModifications(tmp);
#endif
                    SetTextMeshProFontMaterial setTextMeshProFontMaterial =
                        GetComponent<SetTextMeshProFontMaterial>();
                    if(_themeSettings.useFontMaterial) {
                        if(setTextMeshProFontMaterial.fontMaterialSelection ==
                           SetTextMeshProFontMaterial.FontMaterialSelection.Default) {
                            _fontMaterialUsedAutomatically = true;
                            switch(colorTag) {
                                case ColorTag.Unchanged:
                                    break;
                                case ColorTag.White:
                                    break;
                                case ColorTag.Blue:
                                    setTextMeshProFontMaterial.fontMaterialSelection =
                                        SetTextMeshProFontMaterial.FontMaterialSelection.Alt2OrBlue;
                                    break;
                                case ColorTag.Green:
                                    setTextMeshProFontMaterial.fontMaterialSelection =
                                        SetTextMeshProFontMaterial.FontMaterialSelection.Alt1OrGreen;
                                    break;
                                case ColorTag.Gray:
                                    break;
                                case ColorTag.Highlighted:
                                    setTextMeshProFontMaterial.fontMaterialSelection =
                                        SetTextMeshProFontMaterial.FontMaterialSelection.Alt3OrHighlighted;
                                    break;
                                case ColorTag.Caution:
                                    setTextMeshProFontMaterial.fontMaterialSelection =
                                        SetTextMeshProFontMaterial.FontMaterialSelection.Alt4OrCaution;
                                    break;
                                case ColorTag.Danger:
                                    setTextMeshProFontMaterial.fontMaterialSelection =
                                        SetTextMeshProFontMaterial.FontMaterialSelection.Alt5OrDanger;
                                    break;
                                case ColorTag.ExtremeDanger:
                                    setTextMeshProFontMaterial.fontMaterialSelection =
                                        SetTextMeshProFontMaterial.FontMaterialSelection.Alt6OrExtremeDanger;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            setTextMeshProFontMaterial.ApplyTheme();
                        }
                    }
                    else if(_fontMaterialUsedAutomatically) {
                        setTextMeshProFontMaterial.fontMaterialSelection =
                            SetTextMeshProFontMaterial.FontMaterialSelection.Default;
                        _fontMaterialUsedAutomatically = false;
                        setTextMeshProFontMaterial.ApplyTheme();
                    }
                }
            }
        }
    }
}
