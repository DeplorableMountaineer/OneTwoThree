using UnityEngine;
using UnityEngine.UI;

namespace Themed_UI {
    /*
     * Scriptable object containing the data of a theme to be applied to UI elements.
     */
    [CreateAssetMenu(menuName = "Theme Settings", order = 1)]
    public class ThemeSettings : ScriptableObject {
        [Header("Text Mesh Pro font materials")]
        [SerializeField] public Material defaultFontMaterial;
        [SerializeField] public Material fontMaterialAlt1OrGreen;
        [SerializeField] public Material fontMaterialAlt2OrBlue;
        [SerializeField] public Material fontMaterialAlt3OrHighlighted;
        [SerializeField] public Material fontMaterialAlt4OrCaution;
        [SerializeField] public Material fontMaterialAlt5OrDanger;
        [SerializeField] public Material fontMaterialAlt6OrExtremeDanger;

        [Header("Image Variant and Text Tints")]
        [SerializeField] public Color white = new Color(0.984f, 0.969f, 0.965f);
        [SerializeField] public Color blue = new Color(0.537f, 0.91f, 0.882f);
        [SerializeField] public Color green = new Color(0.349f, 0.965f, 0);
        [SerializeField] public Color gray = new Color(0.608f, 0.522f, 0.49f);
        [SerializeField] public Color highlightedColor = new Color(0.859f, 0.576f, 0.373f);
        [SerializeField] public Color cautionColor = new Color(1, 0.91f, 0);
        [SerializeField] public Color dangerColor = new Color(1, 0.157f, 0);
        [SerializeField] public Color extremeDangerColor = new Color(0.576f, 0.157f, 0.953f);
        [SerializeField] public bool useFontMaterial;

        [Header("Textures for various widgets")]
        [SerializeField] public Sprite tiledBackgroundImage;
        [SerializeField] public Sprite buttonImage;
        [SerializeField] public Sprite sliderBackgroundImage;
        [SerializeField] public Sprite sliderHandleImage;
        [SerializeField] public Sprite panelImage;
        [SerializeField] public Sprite toggleBackgroundImage;
        [SerializeField] public Sprite toggleCheckImage;
        [SerializeField] public Sprite dropdownImage;
        [SerializeField] public Sprite inputFieldImage;


        [Header("UI Transition Colors")]
        [SerializeField] public ColorBlock uiColors = ColorBlock.defaultColorBlock;

        [Header("Menu Item Transition Colors")]
        [SerializeField] public ColorBlock menuItemColors = ColorBlock.defaultColorBlock;

        [Header("Other Colors")]
        [SerializeField] public Color sliderFillColor = new Color(0.608f, 0.522f, 0.49f, .5f);
    }
}
