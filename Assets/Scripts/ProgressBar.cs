using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
    [SerializeField] private Slider _slider;

    public void SetMax(int m) {
        _slider.maxValue = m;
    }

    public void SetValue(int v) {
        _slider.value = v;
    }

    public float GetValue() {
        return _slider.value;
    }
}
