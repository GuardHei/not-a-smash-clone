using System;
using System.Collections.Generic;
using UnityEngine;
using Screen = UnityEngine.Device.Screen;

public class CombatLevelManager : MonoBehaviour {

    [Range(30, 240)]
    public int targetFrameRate = 60;

    private void Awake() {
        SetFramerate();
    }

    public void SetFramerate() {
        var refreshRate = Screen.currentResolution.refreshRate;
        if (refreshRate % targetFrameRate == 0) {
            QualitySettings.vSyncCount = refreshRate / targetFrameRate;
            print("VSyncCount " + QualitySettings.vSyncCount);
        } else {
            Application.targetFrameRate = targetFrameRate;
            print("Target Frame Rate " + Application.targetFrameRate);
        }
    }
}
