using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Themed_UI {
    public class ColorPicker : MonoBehaviour {
        [Header("Set to prefab components")]
        [SerializeField] public Image previewImage;
        [SerializeField] private TMP_InputField hexInput;
        [SerializeField] private Slider slider1;
        [SerializeField] private Slider slider2;
        [SerializeField] private Slider slider3;
        [SerializeField] private TMP_InputField input1;
        [SerializeField] private TMP_InputField input2;
        [SerializeField] private TMP_InputField input3;
        [SerializeField] private TextMeshProUGUI indicator1;
        [SerializeField] private TextMeshProUGUI indicator2;
        [SerializeField] private TextMeshProUGUI indicator3;
        [SerializeField] private Button rgbButton;
        [SerializeField] private Button hsvButton;
        [SerializeField] public Button setButton;
        [SerializeField] public Button cancelButton;

        private float _slider1;
        private float _slider2;
        private float _slider3;
        private bool _rgbMode = true;

        private IEnumerator Start() {
            SetSliderFromColor(previewImage.color);
            SetHexFromColor(previewImage.color);
            yield return new WaitForSeconds(0.1f);
            SetChannelInputsFromColor(previewImage.color);
            rgbButton.interactable = false;
            hsvButton.interactable = true;
        }

        public void SetColor(Color color) {
            SetSliderFromColor(color);
            SetHexFromColor(color);
            SetChannelInputsFromColor(color);
            previewImage.color = color;
        }

        public void SetRgbMode() {
            if(_rgbMode) return;
            _rgbMode = true;
            SetSliderFromColor(previewImage.color);
            SetChannelInputsFromColor(previewImage.color);
            indicator1.text = "R";
            indicator2.text = "G";
            indicator3.text = "B";
            rgbButton.interactable = false;
            hsvButton.interactable = true;
        }

        public void SetHsvMode() {
            if(!_rgbMode) return;
            _rgbMode = false;
            SetSliderFromColor(previewImage.color);
            SetChannelInputsFromColor(previewImage.color);
            indicator1.text = "H";
            indicator2.text = "S";
            indicator3.text = "V";
            rgbButton.interactable = true;
            hsvButton.interactable = false;
        }

        public void SliderChanged_1(float value) {
            _slider1 = value;
            UpdateColorFromSlider();
        }

        public void SliderChanged_2(float value) {
            _slider2 = value;
            UpdateColorFromSlider();
        }

        public void SliderChanged_3(float value) {
            _slider3 = value;
            UpdateColorFromSlider();
        }

        public void HexFieldChanged(string value) {
            UpdateColorFromHex();
        }

        public void ChannelInputChanged1(string value) {
            UpdateColorFromChannelInputs();
        }

        public void ChannelInputChanged2(string value) {
            UpdateColorFromChannelInputs();
        }

        public void ChannelInputChanged3(string value) {
            UpdateColorFromChannelInputs();
        }

        private void UpdateColorFromChannelInputs() {
            Color color;
            if(_rgbMode) {
                _slider1 = float.Parse(input1.text)/255.0f;
                _slider2 = float.Parse(input2.text)/255.0f;
                _slider3 = float.Parse(input3.text)/255.0f;
                color = new Color(_slider1, _slider2, _slider3);
            }
            else {
                _slider1 = float.Parse(input1.text)/360.0f;
                _slider2 = float.Parse(input2.text)/100.0f;
                _slider3 = float.Parse(input3.text)/100.0f;
                color = Color.HSVToRGB(_slider1, _slider2, _slider3);
            }

            SetHexFromColor(color);
            SetSliderFromColor(color);
            previewImage.color = color;
            SetChannelInputsFromColor(color);
        }

        private void UpdateColorFromHex() {
            Color color;
            if(!ColorUtility.TryParseHtmlString(hexInput.text, out color)) {
                if(!ColorUtility.TryParseHtmlString("#" + hexInput.text, out color)) {
                    SetHexFromColor(previewImage.color);
                    return;
                }
            }

            SetHexFromColor(color);
            SetSliderFromColor(color);
            previewImage.color = color;
            SetChannelInputsFromColor(color);
        }

        private void UpdateColorFromSlider() {
            Color color;
            if(_rgbMode) {
                color = new Color(_slider1, _slider2, _slider3);
            }
            else {
                color = Color.HSVToRGB(_slider1, _slider2, _slider3);
            }

            previewImage.color = color;
            SetHexFromColor(color);
            SetChannelInputsFromColor(color);
        }

        private void SetSliderFromColor(Color color) {
            if(_rgbMode) {
                _slider1 = color.r;
                _slider2 = color.g;
                _slider3 = color.b;
            }
            else {
                Color.RGBToHSV(color, out _slider1, out _slider2, out _slider3);
            }

            slider1.SetValueWithoutNotify(_slider1);
            slider2.SetValueWithoutNotify(_slider2);
            slider3.SetValueWithoutNotify(_slider3);
        }


        private void SetHexFromColor(Color color) {
            hexInput.SetTextWithoutNotify(string.Format("{0,06:X6}",
                Mathf.RoundToInt(255*color.b) +
                256*(Mathf.RoundToInt(255*color.g) + 256*(Mathf.RoundToInt(255*color.r)))));
        }

        private void SetChannelInputsFromColor(Color color) {
            if(_rgbMode) {
                input1.SetTextWithoutNotify(Mathf.RoundToInt(255*color.r).ToString());
                input2.SetTextWithoutNotify(Mathf.RoundToInt(255*color.g).ToString());
                input3.SetTextWithoutNotify(Mathf.RoundToInt(255*color.b).ToString());
            }
            else {
                float h, s, v;
                Color.RGBToHSV(color, out h, out s, out v);
                input1.SetTextWithoutNotify(Mathf.RoundToInt(360*h).ToString());
                input2.SetTextWithoutNotify(Mathf.RoundToInt(100*s).ToString());
                input3.SetTextWithoutNotify(Mathf.RoundToInt(100*v).ToString());
            }
        }
    }
}
