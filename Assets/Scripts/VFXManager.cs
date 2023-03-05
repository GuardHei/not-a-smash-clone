using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour {

    public const string NORMAL_HIT = "normal_hit";
    public const string BLOCKED_HIT = "blocked_hit";
    public const string PERFECT_BLOCK = "perfect_block";
    public const string FOOT_STEP = "foot_step";
    public const int INIT_SIZE = 4;

    public static VFXManager instance;

    public GameObject normalHitVfx;
    public GameObject blockedHitVfx;
    public GameObject perfectBlockVfx;
    public GameObject footStepVfx;

    private readonly Dictionary<string, GameObject> vfxPrototypes = new();
    private readonly Dictionary<string, Queue<GameObject>> vfxPools = new();

    private void Awake() {
        instance = this;
        vfxPools[NORMAL_HIT] = new Queue<GameObject>(INIT_SIZE);
        vfxPools[BLOCKED_HIT] = new Queue<GameObject>(INIT_SIZE);
        vfxPools[PERFECT_BLOCK] = new Queue<GameObject>(INIT_SIZE);
        vfxPools[FOOT_STEP] = new Queue<GameObject>(INIT_SIZE);

        vfxPrototypes[NORMAL_HIT] = normalHitVfx;
        vfxPrototypes[BLOCKED_HIT] = blockedHitVfx;
        vfxPrototypes[PERFECT_BLOCK] = perfectBlockVfx;
        vfxPrototypes[FOOT_STEP] = footStepVfx;

        for (var i = 0; i < INIT_SIZE; i++) {
            var vfx = Instantiate(vfxPrototypes[NORMAL_HIT]);
            vfx.SetActive(false);
            vfxPools[NORMAL_HIT].Enqueue(vfx);
            vfx = Instantiate(vfxPrototypes[BLOCKED_HIT]);
            vfx.SetActive(false);
            vfxPools[BLOCKED_HIT].Enqueue(vfx);
            vfx = Instantiate(vfxPrototypes[PERFECT_BLOCK]);
            vfx.SetActive(false);
            vfxPools[PERFECT_BLOCK].Enqueue(vfx);
            vfx = Instantiate(vfxPrototypes[FOOT_STEP]);
            vfx.SetActive(false);
            vfxPools[FOOT_STEP].Enqueue(vfx);
        }
    }

    public GameObject Get(string name) {
        if (!vfxPools.ContainsKey(name)) return null;
        var queue = vfxPools[name];
        if (queue.Count > 0) return queue.Dequeue();
        var pop = Instantiate(vfxPrototypes[name]);
        pop.SetActive(false);
        return pop;
    }

    public void Recycle(string name, GameObject go) {
        if (!vfxPools.ContainsKey(name)) return;
        go.SetActive(false);
        vfxPools[name].Enqueue(go);
    }

    public void RecycleOnTime(string name, GameObject go, float time) => StartCoroutine(RecycleOnTimeRoutine(name, go, time));

    private IEnumerator RecycleOnTimeRoutine(string name, GameObject go, float time) {
        yield return new WaitForSeconds(time);
        Recycle(name, go);
    }
}
