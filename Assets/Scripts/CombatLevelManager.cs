using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Screen = UnityEngine.Device.Screen;

public class CombatLevelManager : MonoBehaviour {

    public static CombatLevelManager instance;
    
    [Range(30, 240)]
    public int targetFrameRate = 60;
    public KeyCode restartKey = KeyCode.Backspace;

    [Header("Gameplay")]
    public GameObject player;
    public GameObject opponent;
    public KO ko;
    public GameObject winUI;
    public GameObject loseUI;
    public GameObject ui3;
    public GameObject ui2;
    public GameObject ui1;
    public GameObject uiStart;
    public AudioClip openingSfx;
    public float timeTo3;
    public float timeTo2;
    public float timeTo1;
    public float timeToStart;
    public float timeToEnd;

    [Header("Lights")]
    public Light2D globalUnlitLight;
    public Light2D lighteningLight;
    public Light2D localPointLight;
    [Range(1.0f, 60.0f)]
    public float minLighteningInterval;
    [Range(1.0f, 60.0f)]
    public float maxLighteningInterval;
    public float lighteningIntensity;
    public float minLighteningPhase1;
    public float maxLighteningPhase1;
    public float minLighteningPhase2;
    public float maxLighteningPhase2;
    public AudioClip lighteningSfx;
    public float lighteningVol;
    [Range(.0f, 3.0f)]
    public float lighteningSfxPushTime;

    public Coroutine lastSparkRoutine;

    private void Awake() {
        instance = this;
        SetFramerate();
        
        lighteningLight.gameObject.SetActive(false);
        localPointLight.gameObject.SetActive(false);
        
        GameStart();
    }

    private void Update() {
        if (Input.GetKeyUp(restartKey)) Restart();
    }

    public void SetFramerate() {
        var refreshRate = Screen.currentResolution.refreshRate;
        if (refreshRate % targetFrameRate == 0) {
            QualitySettings.vSyncCount = refreshRate / targetFrameRate;
            Utils.Print("VSyncCount " + QualitySettings.vSyncCount);
        } else {
            Application.targetFrameRate = targetFrameRate;
            Utils.Print("Target Frame Rate " + Application.targetFrameRate);
        }
    }

    public void GameStart() {
        if (winUI != null) winUI.SetActive(false);
        if (loseUI != null) loseUI.SetActive(false);
        if (ui1 != null) ui1.SetActive(false);
        if (ui2 != null) ui2.SetActive(false);
        if (ui3 != null) ui3.SetActive(false);
        if (uiStart != null) uiStart.SetActive(false);
        SetControl(false, false);
        StartCoroutine(GameStartRoutine());
    }

    private IEnumerator GameStartRoutine() {
        yield return new WaitForSeconds(timeTo3);
        if (ui3 != null) ui3.SetActive(true);
        yield return new WaitForSeconds(timeTo2);
        if (ui2 != null) ui2.SetActive(true);
        if (ui3 != null) ui3.SetActive(false);
        yield return new WaitForSeconds(timeTo1);
        if (ui1 != null) ui1.SetActive(true);
        if (ui2 != null) ui2.SetActive(false);
        yield return new WaitForSeconds(timeToStart);
        if (uiStart != null) uiStart.SetActive(true);
        if (ui1 != null) ui1.SetActive(false);
        SetControl(true, true);
        StartCoroutine(SpawnLighteningRoutine());
        yield return new WaitForSeconds(timeToEnd);
        if (uiStart != null) uiStart.SetActive(true);
    }

    public void PlayerWin() {
        if (winUI != null) winUI.SetActive(true);
        GameOver();
    }

    public void PlayerLose() {
        if (loseUI != null) winUI.SetActive(true);
        GameOver();
    }

    public void GameOver() {
        SetControl(false, false);
        if (ko != null) ko.Play();
    }

    public void SetControl(bool p, bool o) {
        player.GetComponent<PlayerInput>().enabled = p;
        
    }

    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    
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

    private IEnumerator SpawnLighteningRoutine() {
        while (true) {
            var interval = Random.Range(minLighteningInterval, maxLighteningInterval);
            Utils.Print("Lightening coming in " + interval + "s");
            var soundInterval = Mathf.Max(interval - lighteningSfxPushTime, .0f);
            yield return new WaitForSeconds(soundInterval);
            SFXManager.PlaySFX(lighteningSfx, Vector2.zero, lighteningVol, 128, .0f);
            var restInterval = interval - soundInterval;
            yield return new WaitForSeconds(restInterval);
            lighteningLight.gameObject.SetActive(true);
            lighteningLight.intensity = lighteningIntensity;
            yield return new WaitForSeconds(Random.Range(minLighteningPhase1, maxLighteningPhase1));
            var phase2 = Random.Range(minLighteningPhase2, maxLighteningPhase2);
            var startTime = Time.time;
            var progress = .0f;
            while (progress < 1.0f) {
                lighteningLight.intensity = Mathf.Lerp(lighteningIntensity, .0f, progress);
                yield return null;
                progress = (Time.time - startTime) / phase2;
            }
            lighteningLight.intensity = .0f;
            lighteningLight.gameObject.SetActive(false);
        }
    }
}
