using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour {
    
    public const int INIT_SIZE = 8;

    public static SFXManager instance;

    public AudioSource playerSrc;
    public AudioSource opponentSrc;
    public GameObject sfxSrcPrototype;

    public Queue<GameObject> sfxPool = new ();

    private void Awake() {
        instance = this;

        for (var i = 0; i < INIT_SIZE; i++) {
            var sfx = Instantiate(sfxSrcPrototype);
            sfx.SetActive(false);
            sfxPool.Enqueue(sfx);
        }
    }

    public static void PlaySFX(AudioClip sfx, Vector2 position, float volume = 1.0f, int priority = -1) {
        if (sfx == null) return;
        GameObject src;
        if (instance.sfxPool.Count > 0) {
            src = instance.sfxPool.Dequeue();
            src.SetActive(true);
        } else src = Instantiate(instance.sfxSrcPrototype);
        src.transform.position = position;
        var audio = src.GetComponent<AudioSource>();
        audio.priority = priority;
        audio.PlayOneShot(sfx, audio.volume);
        instance.RecycleOnTime(src, sfx.length);
    }

    public void Recycle(GameObject go) {
        go.SetActive(false);
        instance.sfxPool.Enqueue(go);
    }

    public void RecycleOnTime(GameObject go, float time) => StartCoroutine(RecycleOnTimeRoutine(go, time));

    private IEnumerator RecycleOnTimeRoutine(GameObject go, float time) {
        yield return new WaitForSeconds(time);
        Recycle(go);
    }

    public static void PlayPlayer(AudioClip sfx, float volume = -1.0f) => PlayFixed(instance.playerSrc, sfx, volume);
    
    public static void PlayOpponent(AudioClip sfx, float volume = -1.0f) => PlayFixed(instance.opponentSrc, sfx, volume);

    public static void PlayFixed(AudioSource src, AudioClip sfx, float volume = -1.0f) {
        if (src == null || sfx == null) return;
        src.Stop();
        src.clip = sfx;
        if (volume >= .0f) src.volume = volume;
    }
}
