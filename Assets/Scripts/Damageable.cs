using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour {

    public Health health;
    public CharacterMotor motor;
    public StateController controller;

    public UnityAction<Damageable, DamageInfo, bool> onHighBlocked;
    public UnityAction<Damageable, DamageInfo, bool> onLowBlocked;
    public UnityAction<Damageable, DamageInfo> onDamaged;

    private void Awake() {
        if (health == null) health = GetComponent<Health>();
        if (motor == null) motor = GetComponent<CharacterMotor>();
        if (controller == null) controller = GetComponent<StateController>();
    }

    public void TakeDamage(DamageInfo info) {
        if (info.canBeHighBlocked) {
            if (controller.currState == CharacterState.HighBlock && motor.facing != info.direction) {
                var perfectBlock = controller.currFrame - controller.stateStartFrame <= controller.perfectBlockFrames;
                onHighBlocked?.Invoke(this, info, perfectBlock);
                return;
            }
        }

        if (info.canBeLowBlocked) {
            if (controller.currState == CharacterState.LowBlock && motor.facing != info.direction) {
                var perfectBlock = controller.currFrame - controller.stateStartFrame <= controller.perfectBlockFrames;
                onLowBlocked?.Invoke(this, info, perfectBlock);
                return;
            }
        }
        
        health.Hit(info.damage);
        onDamaged?.Invoke(this, info);
    }
}

public struct DamageInfo {
    public int damage;
    public CharacterFacing direction;
    public bool canBeHighBlocked;
    public bool canBeLowBlocked;
    public bool isEndOfCombo;
    public Vector2 hitPosition;
}
