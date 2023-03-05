using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour {

    [Header("Lighting Settings")]
    public float blockLightIntensity = .5f;
    public float blockLightScale = 12.0f;
    public float blockLightPhase1 = 1.0f;
    public float blockLightPhase2 = 1.0f;
    public float perfectBlockLightIntensity = 1.0f;
    public float perfectBlockLightScale = 18.0f;
    public float perfectBlockLightPhase1 = 2.0f;
    public float perfectBlockLightPhase2 = 2.0f;
    public float hitLightIntensity = .5f;
    public float hitLightScale = 12.0f;
    public float hitLightPhase1 = 1.0f;
    public float hitLightPhase2 = 1.0f;

    [Header("References")]
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
            if (perfect) CombatLevelManager.SparkLocally(info.hitPosition, perfectBlockLightPhase1, perfectBlockLightPhase2, perfectBlockLightIntensity, perfectBlockLightScale);
            else CombatLevelManager.SparkLocally(info.hitPosition, blockLightPhase1, blockLightPhase2, blockLightIntensity, blockLightScale);
        };
        
        onLowBlocked += (dmg, info, perfect) => {
            var vfxName = perfect ? VFXManager.PERFECT_BLOCK : VFXManager.BLOCKED_HIT;
            var vfx = VFXManager.instance.Get(vfxName);
            vfx.transform.position = info.hitPosition;
            vfx.SetActive(true);
            var ps = vfx.GetComponent<ParticleSystem>();
            ps.Play(true);
            
            VFXManager.instance.RecycleOnTime(vfxName, vfx, ps.main.duration + .1f);
            if (perfect) CombatLevelManager.SparkLocally(info.hitPosition, perfectBlockLightPhase1, perfectBlockLightPhase2, perfectBlockLightIntensity, perfectBlockLightScale);
            else CombatLevelManager.SparkLocally(info.hitPosition, blockLightPhase1, blockLightPhase2, blockLightIntensity, blockLightScale);
        };
        
        onDamaged += (dmg, info) => {
            var vfx = VFXManager.instance.Get(VFXManager.NORMAL_HIT);
            vfx.transform.position = info.hitPosition;
            vfx.SetActive(true);
            var ps = vfx.GetComponent<ParticleSystem>();
            ps.Play(true);
            
            VFXManager.instance.RecycleOnTime(VFXManager.NORMAL_HIT, vfx, ps.main.duration + .1f);
            CombatLevelManager.SparkLocally(info.hitPosition, hitLightPhase1, hitLightPhase2, hitLightIntensity, hitLightScale);
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
