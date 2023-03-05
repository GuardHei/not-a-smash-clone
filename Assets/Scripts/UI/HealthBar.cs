using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Image bar;
    public bool reversed;
    public float minFilled;
    public float maxFilled;
    public Health health;

    [Header("Status")]
    [Range(.0f, 1.0f)]
    public float currFilled;

    public void UpdateBar(Health health) {
        if (health == null || bar == null) return;
        var progress = (float) health.currHealth / (float) health.maxHealth;
        progress = Mathf.Min(progress, 1.0f);
        progress = Mathf.Max(progress, .0f);
        currFilled = Mathf.Lerp(minFilled, maxFilled, progress);
        currFilled = reversed ? 1.0f - currFilled : currFilled;
        bar.fillAmount = currFilled;
    }
}