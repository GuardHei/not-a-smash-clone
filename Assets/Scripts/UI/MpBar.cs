using UnityEngine;
using UnityEngine.UI;

public class MpBar : MonoBehaviour {
    
    public Image bar;
    public bool reversed;
    public float minFilled;
    public float maxFilled;
    public StateController state;

    [Header("Status")]
    [Range(.0f, 1.0f)]
    public float currFilled;
    public int lastMp = -1;

    private void Update() {
        if (state == null || bar == null) return;
        if (state.currMp != lastMp) {
            lastMp = state.currMp;
            UpdateBar();
        }
    }

    public void UpdateBar() {
        if (state == null || bar == null) return;
        var progress = (float) state.currMp / (float) state.maxMp;
        progress = Mathf.Min(progress, 1.0f);
        progress = Mathf.Max(progress, .0f);
        currFilled = Mathf.Lerp(minFilled, maxFilled, progress);
        currFilled = reversed ? 1.0f - currFilled : currFilled;
        bar.fillAmount = currFilled;
    }
}