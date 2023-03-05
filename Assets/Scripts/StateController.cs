using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class StateController : MonoBehaviour {

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

    [Header("References")]
    public CharacterMotor motor;
    public AnimatorController animator;
    public Damageable damageable;
    public HitBox punchHitBox;
    public HitBox kickHitBox;

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

        if (damageable != null) {
            damageable.onDamaged += TakeDamage;
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
    }

    private void StartMove(CharacterFacing direction) {
        currState = CharacterState.Move;
        stateStartFrame = currFrame;
        moveDirection = direction;
    }

    private void StartPunch() {
        currState = CharacterState.Punch;
        stateStartFrame = currFrame;
    }
    
    private void StartKick() {
        currState = CharacterState.Kick;
        stateStartFrame = currFrame;
    }

    private void StartFireBall() {
        currState = CharacterState.FireBall;
        stateStartFrame = currFrame;
    }

    private void StartHighBlock() {
        currState = CharacterState.HighBlock;
        stateStartFrame = currFrame;
    }

    private void StartLowBlock() {
        currState = CharacterState.LowBlock;
        stateStartFrame = currFrame;
    }

    private void StartStunned() {
        damaged = false;
        stunFrames = damageInfo.stunFrames;
        stunPush = damageInfo.stunPush;
        stunPushDirection = damageInfo.direction;
        currState = CharacterState.Stunned;
        stateStartFrame = currFrame;
    }

    private void StartBlockRecovery() {
        currState = CharacterState.BlockRecovery;
        stateStartFrame = currFrame;
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
            case InputCommand.FireBall: StartFireBall(); break;
            case InputCommand.HighBlock: StartFireBall(); break;
            case InputCommand.LowBlock: StartFireBall(); break;
        }
    }

    private void UpdateMove() {
        if (damaged) {
            StartStunned();
            return;
        }
        
        motor.SweepAndMove(motor.moveSpeed, (int) moveDirection, true);

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
            case InputCommand.FireBall: StartFireBall(); break;
            case InputCommand.HighBlock: StartFireBall(); break;
            case InputCommand.LowBlock: StartFireBall(); break;
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
        
    }

    public void SendCommand(InputCommand command) {
        currCommand = command;
    }

    private void TakeDamage(Damageable damageable, DamageInfo info) {
        damaged = true;
        damageInfo = info;
    }
}
