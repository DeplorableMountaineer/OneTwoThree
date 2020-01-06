using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Fader : MonoBehaviour {
    [SerializeField] private float startFadeValue = default;
    [SerializeField] private bool fadeInOnStart = true;
    [SerializeField] private float fadeInTime = 1;
    [SerializeField] private float fadeOutTime = 2;
    [SerializeField] private UnityEvent onFinishedFading = default;
    [SerializeField] private Image image = default;

    public void StartFadeIn() {
        StartCoroutine(FadeInAsync());
        if(onFinishedFading != null) {
            onFinishedFading.Invoke();
        }
    }

    public void StartFadeOut() {
        StartCoroutine(FadeOutAsync());
        if(onFinishedFading != null) {
            onFinishedFading.Invoke();
        }
    }

    private void Start() {
        if(!image) {
            image = GetComponentInChildren<Image>();
        }

        if(fadeInOnStart) {
            StartCoroutine(FadeInAsync());
        }
        else {
            SetFaderValue(startFadeValue);
        }
    }

    private IEnumerator FadeInAsync() {
        float time = 0;
        SetFaderValue(1);
        while(time <= fadeInTime) {
            time += Time.deltaTime;
            float amount = 1 - time/fadeInTime;
            SetFaderValue(amount);
            yield return null;
        }

        SetFaderValue(0);
    }

    private IEnumerator FadeOutAsync() {
        float time = 0;
        SetFaderValue(0);
        while(time <= fadeOutTime) {
            time += Time.deltaTime;
            float amount = time/fadeOutTime;
            SetFaderValue(amount);
            yield return null;
        }

        SetFaderValue(1);
    }

    private void SetFaderValue(float value) {
        Color color = image.color;
        color = new Color(color.r, color.g, color.b, value);
        image.color = color;
    }
}
