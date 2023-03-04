using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    [Header("Settings")]
    public KeyCode moveRight = KeyCode.D;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode dodgeRight = KeyCode.D;
    public KeyCode dodgeLeft = KeyCode.A;
    public KeyCode punch = KeyCode.J;
    public KeyCode kick = KeyCode.K;
    public KeyCode upperBlock = KeyCode.U;
    public KeyCode lowerBlock = KeyCode.I;
    public KeyCode fireball = KeyCode.O;

    [Header("References")]
    public CharacterController cc;

    private void Awake() {
        if (cc == null) cc = GetComponent<CharacterController>();
    }

    private void Update() {
        if (cc == null) return;

        if (Input.GetKey(moveRight)) {
            cc.SweepAndMove(cc.moveSpeed, 1.0f, true);
        } else if (Input.GetKey(moveLeft)) {
            cc.SweepAndMove(cc.moveSpeed, -1.0f, true);
        }
    }
}
