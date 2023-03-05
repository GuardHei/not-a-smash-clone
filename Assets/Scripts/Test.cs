using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public ParticleSystem ps;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ps.Play();
        }
    }
}