using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class Callback : UnityEvent<float> {
}

public class Lerp : MonoBehaviour {
    [SerializeField] private Callback callback = default;
    [SerializeField] private UnityEvent onEnd = default;
    [SerializeField] private float numSeconds = 1;
    [SerializeField] private float start = default;
    [SerializeField] private float end = 1;
    [SerializeField] private bool beginOnStart = true;
    [SerializeField] private bool repeat = default;

    private long _numTicks;
    private bool _active;

    private void Start() {
        if(beginOnStart) Begin();
    }

    private void Update() {
        float seconds = ((float)(DateTime.Now.Ticks - _numTicks))/TimeSpan.TicksPerSecond;
        if(seconds >= 0 && seconds <= numSeconds) {
            float value = start + (end - start)/numSeconds*seconds;
            if(callback != null) callback.Invoke(value);
        }
        else if(seconds > numSeconds && _active) {
            _active = false;
            if(callback != null) callback.Invoke(end);
            if(onEnd != null) onEnd.Invoke();
            if(repeat) Begin();
        }
    }

    public void Begin() {
        _active = true;
        _numTicks = DateTime.Now.Ticks;
        if(callback != null) callback.Invoke(start);
    }
}
