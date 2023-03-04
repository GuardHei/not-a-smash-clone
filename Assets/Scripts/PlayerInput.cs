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
    public KeyCode highBlock = KeyCode.U;
    public KeyCode lowBlock = KeyCode.I;
    public KeyCode fireball = KeyCode.O;

    [Header("References")]
    public CharacterMotor motor;

    private void Awake() {
        if (motor == null) motor = GetComponent<CharacterMotor>();
    }

    private void Update() {
        if (motor == null) return;

        if (Input.GetKey(moveRight)) {
            motor.SweepAndMove(motor.moveSpeed, 1.0f, true);
        } else if (Input.GetKey(moveLeft)) {
            motor.SweepAndMove(motor.moveSpeed, -1.0f, true);
        }
    }
}
