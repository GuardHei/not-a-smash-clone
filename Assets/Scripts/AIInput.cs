using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInput : MonoBehaviour {
    
    [Header("References")]
    public CharacterMotor motor;

    private void Awake() {
        if (motor == null) motor = GetComponent<CharacterMotor>();
    }

    private void Update() {
        if (motor == null) return;

        motor.SweepAndMove(motor.moveSpeed, -1.0f, true);
    }
}
