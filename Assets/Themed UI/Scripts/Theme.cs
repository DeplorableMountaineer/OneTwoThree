using UnityEngine;

namespace Themed_UI {
    /*
     * Component of the canvas whose children include UI elements modified by the theme settings.  Used to specify the
     * theme and to command UI element component scripts to perform the modifications for previewing and in game.
     * Runs in editor so UI can be previewed in the editor.
     */
    [ExecuteAlways] [DisallowMultipleComponent]
    public class Theme : MonoBehaviour {
        [Header("The selected theme goes here")]
        [SerializeField] public ThemeSettings theme;

        [Header("Check the Preview box to apply all UI Theme settings to children after making changes")]
        [SerializeField] private bool preview;

        private void Start() {
            preview = true; // ensure update is run when game starts
        }

        public void Refresh() {
            preview = true;
        }

        //Runs in editor
        void Update() {
            if(preview) { //preview checkbox has been selected
                preview = false; //turn checkbox off; this update is only done once
                foreach(IThemeable pv in GetComponentsInChildren<IThemeable>()) {
                    pv.ApplyTheme(); //Run preview on everything previewable under the canvas
                }
            }
        }
    }
}
