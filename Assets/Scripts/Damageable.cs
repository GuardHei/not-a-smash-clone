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

        onHighBlocked += (dmg, info, perfect) => {
            var vfxName = perfect ? VFXManager.PERFECT_BLOCK : VFXManager.BLOCKED_HIT;
            var vfx = VFXManager.instance.Get(vfxName);
            vfx.transform.position = info.hitPosition;
            vfx.SetActive(true);
            var ps = vfx.GetComponent<ParticleSystem>();
            ps.Play(true);
            
            VFXManager.instance.RecycleOnTime(vfxName, vfx, ps.main.duration + .1f);
        };
        
        onLowBlocked += (dmg, info, perfect) => {
            var vfxName = perfect ? VFXManager.PERFECT_BLOCK : VFXManager.BLOCKED_HIT;
            var vfx = VFXManager.instance.Get(vfxName);
            vfx.transform.position = info.hitPosition;
            vfx.SetActive(true);
            var ps = vfx.GetComponent<ParticleSystem>();
            ps.Play(true);
            
            VFXManager.instance.RecycleOnTime(vfxName, vfx, ps.main.duration + .1f);
        };
        
        onDamaged += (dmg, info) => {
            var vfx = VFXManager.instance.Get(VFXManager.NORMAL_HIT);
            vfx.transform.position = info.hitPosition;
            vfx.SetActive(true);
            var ps = vfx.GetComponent<ParticleSystem>();
            ps.Play(true);
            
            VFXManager.instance.RecycleOnTime(VFXManager.NORMAL_HIT, vfx, ps.main.duration + .1f);
        };
    }

    public bool TakeDamage(DamageInfo info) {
        if (controller != null && motor != null) {
            if (info.canBeHighBlocked) {
                if (controller.currState == CharacterState.HighBlock && motor.facing != info.direction) {
                    var perfectBlock = controller.currFrame - controller.stateStartFrame <= controller.perfectBlockFrames;
                    onHighBlocked?.Invoke(this, info, perfectBlock);
                    return false;
                }
            }

            if (info.canBeLowBlocked) {
                if (controller.currState == CharacterState.LowBlock && motor.facing != info.direction) {
                    var perfectBlock = controller.currFrame - controller.stateStartFrame <= controller.perfectBlockFrames;
                    onLowBlocked?.Invoke(this, info, perfectBlock);
                    return false;
                }
            }
        }

        Utils.Print(tag + " takes " + info.damage + " dmg, aiming " + info.direction);
        
        health.Hit(info.damage);
        onDamaged?.Invoke(this, info);

        return true;
    }
}

public struct DamageInfo {
    public int damage;
    public int stunFrames;
    public float stunPush;
    public CharacterFacing direction;
    public bool canBeHighBlocked;
    public bool canBeLowBlocked;
    public bool isEndOfCombo;
    public Vector2 hitPosition;
}
