using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    [SerializeField] private bool stopped = default;
    [SerializeField] private float startSeconds = 60;
    [SerializeField] private float numSeconds = 60;
    [SerializeField] private bool autoRepeat = default;
    [SerializeField] private bool countUp = default;
    [SerializeField] private UnityEvent onAlarm = default;

    private long _startTicks;
    private long _currentElapsedTicks;
    private TextMeshProUGUI _display;
    private Text _display2;
    private float _lastSeconds = -1;

    private void Start() {
        _display = GetComponent<TextMeshProUGUI>();
        _display2 = GetComponent<Text>();

        _startTicks = DateTime.Now.Ticks;
    }

    private void Update() {
        if(stopped) return;
        _currentElapsedTicks = DateTime.Now.Ticks - _startTicks;
        UpdateDisplay();
        if(_currentElapsedTicks >= numSeconds*TimeSpan.TicksPerSecond) {
            Alarm();
            stopped = !autoRepeat;
            _startTicks = DateTime.Now.Ticks;
        }
    }

    public void AddSeconds(int seconds) {
        _startTicks += seconds*TimeSpan.TicksPerSecond;
        UpdateDisplay();
    }

    private void Alarm() {
        if(onAlarm != null) {
            onAlarm.Invoke();
        }
    }

    private void UpdateDisplay() {
        if(_display || _display2) {
            float seconds = ((float)_currentElapsedTicks)/TimeSpan.TicksPerSecond;
            if(countUp) {
                seconds = Mathf.Floor(startSeconds + seconds);
            }
            else {
                seconds = Mathf.Ceil(startSeconds - seconds);
            }

            if(Math.Abs(seconds - _lastSeconds) < 0.1f) return;
            _lastSeconds = seconds;
            if(seconds < 0) {
                if(_display) _display.text = "--";
                if(_display2) _display2.text = "--";
                return;
            }

            string timeText = new TimeSpan((long)(seconds*TimeSpan.TicksPerSecond)).ToString();
            if(timeText.StartsWith("00:")) {
                timeText = timeText.Substring(3);
                if(timeText.StartsWith("00:")) {
                    timeText = timeText.Substring(3);
                    if(timeText.StartsWith("0")) {
                        timeText = timeText.Substring(1);
                    }
                }
            }

            if(_display) _display.text = timeText;
            if(_display2) _display2.text = timeText;
        }
    }
}
