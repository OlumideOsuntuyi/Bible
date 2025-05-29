using System;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ProgressBar : MonoBehaviour
{
    public float range { get; private set; }
    public Image bubble;
    public Gradient color;
    public float min, max;
    public TMP_Text label;
    public bool useLabel;
    public float currentValue;
    public float blendSpeed;
    public bool doSmoothLerp;


    private float previousValue;
    private float newrange;

    private void Awake()
    {
        previousValue = -1;
    }
    void Update()
    {
        if (previousValue != currentValue || (doSmoothLerp && range != newrange))
        {
            previousValue = currentValue;
            newrange = Mathf.InverseLerp(min, max, currentValue);
            range = doSmoothLerp ? Mathf.Lerp(range, newrange, Time.deltaTime * blendSpeed) : newrange;

            if (bubble)
            {
                bubble.fillAmount = range;
                bubble.color = color.Evaluate(range);
            }

            if (useLabel && label)
            {
                label.text = $"{Mathf.Round(range * 100)}%";
            }
        }
    }
}