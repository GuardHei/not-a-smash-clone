using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StateController : MonoBehaviour {
    
    public static readonly int AnimStateIndex = Animator.StringToHash("stateIndex");

    [Header("Settings")]
    public int punchStartFrames;
    public int punchDamageFrames;
    public int punchEndFrames;
    public int kickStartFrames;
    public int kickDamageFrames;
    public int kickEndFrames;
    public int fireBallStartFrames;
    public int fireBallEndFrames;
    public int perfectBlockFrames;
    public int blockEndFrames;
    
    [Header("MP Related")]
    public int maxMp;
    public int currMp;
    public int fireBallMpCost;
    public int hitMpRegen;
    public int blockMpRegen;
    public int perfectBlockMpRegen;

    [Header("SFX")]
    public AudioSource mouth;
    public AudioClip normalHitSfx;
    public AudioClip blockSfx;
    public AudioClip perfectBlockSfx;
    [Range(.0f, 1.0f)]
    public float normalHitVol = 1.0f;
    [Range(.0f, 1.0f)]
    public float blockVol = 1.0f;
    [Range(.0f, 1.0f)]
    public float perfectBlockVol = 1.0f;
    [Range(.0f, 1.0f)]
    public float blockSpatial = .0f;
    [Range(.0f, 1.0f)]
    public float perfectBlockSpatial = .0f;
    public AudioClip punchSfx;
    public AudioClip kickSfx;
    public AudioClip fireBallSfx;
    [Range(.0f, 1.0f)]
    public float punchVol;
    [Range(.0f, 1.0f)]
    public float kickVol;
    [Range(.0f, 1.0f)]
    public float fireBallVol;
    public float fireBallSfxDelay = 1.0f;
    public List<AudioClip> footStepSfx;
    [Range(.0f, 1.0f)]
    public float footStepVol = 1.0f;
    public int footStepSfxInterval = 60;
    public AudioSource footStepSrc;
    

    [Header("References")]
    public CharacterMotor motor;
    public Animator animator;
    public Damageable damageable;
    public HitBox punchHitBox;
    public HitBox kickHitBox;
    public Transform fireBallOrigin;
    public GameObject fireBallPrototype;
    public Transform footStepOrigin;

    [Header("Status")]
    public bool damaged;
    public DamageInfo damageInfo;
    public int stunFrames;
    public float stunPush;
    public CharacterFacing stunPushDirection;
    public int stateStartFrame;
    public int currFrame;
    public int frameProgress;
    public CharacterState currState;
    public InputCommand currCommand;
    public CharacterFacing moveDirection;

    private void Awake() {
        currState = CharacterState.Idle;
        currCommand = InputCommand.None;

        punchHitBox.Deactivate();
        kickHitBox.Deactivate();

        punchHitBox.onHit += () => AddMp(hitMpRegen);

        if (damageable != null) {
            damageable.onDamaged += TakeDamage;
            damageable.onHighBlocked += (dmg, info, perfect) => {
                if (perfect) {
                    AddMp(perfectBlockMpRegen);
                    SFXManager.PlaySFX(perfectBlockSfx, info.hitPosition, perfectBlockVol, 200, .0f, perfectBlockSpatial);
                } else {
                    AddMp(blockMpRegen);
                    SFXManager.PlaySFX(blockSfx, info.hitPosition, blockVol, 195, .0f, blockSpatial);
                }
            };
            damageable.onLowBlocked += (dmg, info, perfect) => {
                if (perfect) {
                    AddMp(perfectBlockMpRegen);
                    SFXManager.PlaySFX(perfectBlockSfx, info.hitPosition, perfectBlockVol, 200, .0f, perfectBlockSpatial);
                } else {
                    AddMp(blockMpRegen);
                    SFXManager.PlaySFX(blockSfx, info.hitPosition, blockVol, 195, .0f, blockSpatial);
                }
            };
        }
    }

    private void Update() {
        if (motor == null) return;
        
        currFrame = Time.frameCount;
        frameProgress = currFrame - stateStartFrame;

        switch (currState) {
            case CharacterState.Idle: UpdateIdle(); break;
            case CharacterState.Move: UpdateMove(); break;
            case CharacterState.Punch: UpdatePunch(); break;
            case CharacterState.Kick: UpdateKick(); break;
            case CharacterState.FireBall: UpdateFireBall(); break;
            case CharacterState.HighBlock: UpdateHighBlock(); break;
            case CharacterState.LowBlock: UpdateLowBlock(); break;
            case CharacterState.Stunned: UpdateStunned(); break;
            case CharacterState.BlockRecovery: UpdateBlockRecovery(); break;
        }
    }

    private void StartIdle() {
        currState = CharacterState.Idle;
        stateStartFrame = currFrame;
        animator.SetInteger(AnimStateIndex, (int) AnimCharacterState.Idle);
    }

    private void StartMove(CharacterFacing direction) {
        currState = CharacterState.Move;
        stateStartFrame = currFrame;
        moveDirection = direction;
        if (motor.defaultFacing == direction) animator.SetInteger(AnimStateIndex, (int) AnimCharacterState.MoveFront);
        else animator.SetInteger(AnimStateIndex, (int) AnimCharacterState.MoveBack);
        PlayFootStepSfx();
        PlayFootStepVfx();
    }

    private void StartPunch() {
        currState = CharacterState.Punch;
        stateStartFrame = currFrame;
        animator.SetInteger(AnimStateIndex, (int) AnimCharacterState.Punch);
        SFXManager.PlayFixed(mouth, punchSfx, punchVol);
    }
    
    private void StartKick() {
        currState = CharacterState.Kick;
        stateStartFrame = currFrame;
        animator.SetInteger(AnimStateIndex, (int) AnimCharacterState.Kick);
        SFXManager.PlayFixed(mouth, kickSfx, kickVol);
    }

    private void StartFireBall() {
        currState = CharacterState.FireBall;
        stateStartFrame = currFrame;
        animator.SetInteger(AnimStateIndex, (int) AnimCharacterState.FireBall);
        SFXManager.PlayFixed(mouth, fireBallSfx, fireBallVol, fireBallSfxDelay);
    }

    private void StartHighBlock() {
        currState = CharacterState.HighBlock;
        stateStartFrame = currFrame;
        animator.SetInteger(AnimStateIndex, (int) AnimCharacterState.HighBlock);
    }

    private void StartLowBlock() {
        currState = CharacterState.LowBlock;
        stateStartFrame = currFrame;
        animator.SetInteger(AnimStateIndex, (int) AnimCharacterState.LowBlock);
    }

    private void StartStunned() {
        damaged = false;
        stunFrames = damageInfo.stunFrames;
        stunPush = damageInfo.stunPush;
        stunPushDirection = damageInfo.direction;
        currState = CharacterState.Stunned;
        stateStartFrame = currFrame;
        animator.SetInteger(AnimStateIndex, (int) AnimCharacterState.Stunned);
    }

    private void StartBlockRecovery() {
        currState = CharacterState.BlockRecovery;
        stateStartFrame = currFrame;
        animator.SetInteger(AnimStateIndex, (int) AnimCharacterState.BlockRecovery);
    }

    private void UpdateIdle() {
        if (damaged) {
            StartStunned();
            return;
        }
        
        switch (currCommand) {
            case InputCommand.MoveLeft: StartMove(CharacterFacing.Left); break;
            case InputCommand.MoveRight: StartMove(CharacterFacing.Right); break;
            case InputCommand.Punch: StartPunch(); break;
            case InputCommand.Kick: StartKick(); break;
            case InputCommand.FireBall:
                if (HaveEnoughMpFor(fireBallMpCost)) StartFireBall();
                break;
            case InputCommand.HighBlock: StartHighBlock(); break;
            case InputCommand.LowBlock: StartLowBlock(); break;
        }
    }

    private void UpdateMove() {
        if (damaged) {
            StartStunned();
            return;
        }
        
        motor.SweepAndMove(motor.moveSpeed, (int) moveDirection, true);

        if (frameProgress % footStepSfxInterval == 0) {
            PlayFootStepSfx();
            PlayFootStepVfx();
        }

        switch (currCommand) {
            case InputCommand.None: StartIdle(); break;
            case InputCommand.MoveLeft:
                if (moveDirection == CharacterFacing.Right) StartMove(CharacterFacing.Left);
                break;
            case InputCommand.MoveRight:
                if (moveDirection == CharacterFacing.Left) StartMove(CharacterFacing.Right);
                break;
            case InputCommand.Punch: StartPunch(); break;
            case InputCommand.Kick: StartKick(); break;
            case InputCommand.FireBall:
                if (HaveEnoughMpFor(fireBallMpCost)) StartFireBall();
                break;
            case InputCommand.HighBlock: StartHighBlock(); break;
            case InputCommand.LowBlock: StartLowBlock(); break;
        }
    }

    private void UpdatePunch() {
        if (damaged) {
            punchHitBox.Deactivate();
            StartStunned();
            return;
        }
        
        if (frameProgress > punchStartFrames && frameProgress <= punchStartFrames + punchDamageFrames) punchHitBox.Activate(motor.facing);
        if (frameProgress > punchStartFrames + punchDamageFrames) punchHitBox.Deactivate();

        switch (currCommand) {
            case InputCommand.HighBlock:
                punchHitBox.Deactivate();
                StartHighBlock();
                return;
            case InputCommand.LowBlock:
                punchHitBox.Deactivate();
                StartLowBlock();
                return;
        }

        if (frameProgress >= punchStartFrames + punchDamageFrames + punchEndFrames) StartIdle();
    }

    private void UpdateKick() {
        if (damaged) {
            kickHitBox.Deactivate();
            StartStunned();
            return;
        }
        
        if (frameProgress > kickStartFrames && frameProgress <= kickStartFrames + kickDamageFrames) kickHitBox.Activate(motor.facing);
        if (frameProgress > kickStartFrames + kickDamageFrames) kickHitBox.Deactivate();
        
        switch (currCommand) {
            case InputCommand.HighBlock:
                kickHitBox.Deactivate();
                StartHighBlock();
                return;
            case InputCommand.LowBlock:
                kickHitBox.Deactivate();
                StartLowBlock();
                return;
        }
        
        if (frameProgress >= kickStartFrames + kickDamageFrames + kickEndFrames) StartIdle();
    }

    private void UpdateFireBall() {
        if (frameProgress == fireBallStartFrames + 1) SpawnFireBall();
        
        if (damaged) {
            StartStunned();
            return;
        }
        
        switch (currCommand) {
            case InputCommand.HighBlock:
                StartHighBlock();
                return;
            case InputCommand.LowBlock:
                StartLowBlock();
                return;
        }
        
        if (frameProgress >= fireBallStartFrames + fireBallEndFrames) StartIdle();
    }

    private void UpdateHighBlock() {
        if (damaged) {
            StartStunned();
            return;
        }
        
        if (currCommand != InputCommand.HighBlock) StartBlockRecovery();
    }

    private void UpdateLowBlock() {
        if (damaged) {
            StartStunned();
            return;
        }
        
        if (currCommand != InputCommand.LowBlock) StartBlockRecovery();
    }

    private void UpdateStunned() {
        if (damaged) {
            StartStunned();
            return;
        }
        
        motor.SweepAndMove(stunPush, (int) stunPushDirection, true);
        if (frameProgress >= stunFrames) StartIdle();
    }

    private void UpdateBlockRecovery() {
        if (damaged) {
            StartStunned();
            return;
        }
        
        if (frameProgress >= blockEndFrames) StartIdle();
    }

    private void SpawnFireBall() {
        ConsumeMp(fireBallMpCost);
        Instantiate(fireBallPrototype, fireBallOrigin.position, fireBallOrigin.rotation);
    }

    public void AddMp(int mp) => currMp = Mathf.Min(currMp + mp, maxMp);

    public bool HaveEnoughMpFor(int cost) => currMp >= cost;

    public bool ConsumeMp(int cost) {
        if (!HaveEnoughMpFor(cost)) return false;
        currMp -= cost;
        return true;
    }

    public void SendCommand(InputCommand command) {
        currCommand = command;
    }

    private void TakeDamage(Damageable damageable, DamageInfo info) {
        SFXManager.PlayFixed(mouth, normalHitSfx, normalHitVol);
        damaged = true;
        damageInfo = info;
    }

    private void PlayFootStepSfx() {
        if (footStepSrc == null || footStepSfx == null || footStepSfx.Count == 0) return;
        var sfx = footStepSfx[Random.Range(0, footStepSfx.Count)];
        SFXManager.PlayFixed(footStepSrc, sfx, footStepVol);
    }

    private void PlayFootStepVfx() {
        if (footStepOrigin == null) return;
        var vfx = VFXManager.instance.Get(VFXManager.FOOT_STEP);
        vfx.transform.position = footStepOrigin.position;
        vfx.SetActive(true);
        var ps = vfx.GetComponent<ParticleSystem>();
        ps.Play(true);
            
        VFXManager.instance.RecycleOnTime(VFXManager.FOOT_STEP, vfx, ps.main.duration + .1f);
    }
}
