using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Screen = UnityEngine.Device.Screen;

public class CombatLevelManager : MonoBehaviour {

    public static CombatLevelManager instance;
    
    [Range(30, 240)]
    public int targetFrameRate = 60;

    [Header("Lights")]
    public Light2D globalUnlitLight;
    public Light2D localPointLight;

    public Coroutine lastSparkRoutine;

    private void Awake() {
        instance = this;
        SetFramerate();
        
        localPointLight.gameObject.SetActive(false);
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

    public static void SparkLocally(Vector2 pos, float phase1 = 1.0f, float phase2 = 2.0f, float intensity = 1.0f, float scale = 12.0f) {
        if (instance.lastSparkRoutine != null) instance.StopCoroutine(instance.lastSparkRoutine);
        instance.lastSparkRoutine = instance.StartCoroutine(instance.SparkLocallyRoutine(pos, phase1, phase2, intensity, scale));
    }

    private IEnumerator SparkLocallyRoutine(Vector2 pos, float phase1 = 1.0f, float phase2 = 2.0f, float intensity = 1.0f, float scale = 12.0f) {
        localPointLight.gameObject.SetActive(true);
        localPointLight.transform.position = pos;
        localPointLight.transform.localScale = new Vector3(scale, scale, 1.0f);
        localPointLight.intensity = intensity;
        yield return new WaitForSeconds(phase1);
        var startTime = Time.time;
        var progress = .0f;
        while (progress < 1.0f) {
            localPointLight.intensity = Mathf.Lerp(intensity, .0f, progress);
            yield return null;
            progress = (Time.time - startTime) / phase2;
        }
        localPointLight.intensity = .0f;
        localPointLight.gameObject.SetActive(false);
    }
}
