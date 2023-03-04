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

    [Header("References")]
    public CharacterMotor motor;
    public AnimatorController animator;

    [Header("Status")]
    public int stateStartFrame;
    public int currFrame;
    public CharacterState currState;
    public InputCommand currCommand;
    public CharacterFacing moveDirection;
    public int incomingStun;
    public float incomingPush;
    public int currentStun;
    public float currentPush;

    private void Awake() {
        currCommand = InputCommand.None;
        
        
    }

    private void Update() {
        if (motor == null) return;
        
        currFrame = Time.frameCount;

        switch (currState) {
            case CharacterState.Idle: UpdateDodge(); break;
            case CharacterState.Move: UpdateMove(); break;
            case CharacterState.Dodge: UpdateDodge(); break;
            case CharacterState.Punch: UpdatePunch(); break;
            case CharacterState.Kick: UpdateKick(); break;
            case CharacterState.FireBall: UpdateFireBall(); break;
            case CharacterState.HighBlock: UpdateHighBlock(); break;
            case CharacterState.LowBlock: UpdateLowBlock(); break;
            case CharacterState.Stunned: UpdateStunned(); break;
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

    private void StartDodge(CharacterFacing direction) {
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

    private void StartStunned(float frames) {
        currState = CharacterState.Stunned;
        stateStartFrame = currFrame;
    }

    private void UpdateIdle() {
        if (incomingStun > .0f) {
            StartStunned(incomingStun);
            return;
        }

        switch (currCommand) {
            case InputCommand.MoveLeft: StartMove(CharacterFacing.Left); break;
            case InputCommand.MoveRight: StartMove(CharacterFacing.Right); break;
            case InputCommand.DodgeLeft: StartDodge(CharacterFacing.Left); break;
            case InputCommand.DodgeRight: StartDodge(CharacterFacing.Right); break;
            case InputCommand.Punch: StartPunch(); break;
            case InputCommand.Kick: StartKick(); break;
            case InputCommand.FireBall: StartFireBall(); break;
            case InputCommand.HighBlock: StartFireBall(); break;
            case InputCommand.LowBlock: StartFireBall(); break;
        }
    }

    private void UpdateMove() {
        motor.SweepAndMove(motor.moveSpeed, (int) moveDirection, true);

        if (incomingStun > .0f) {
            StartStunned(incomingStun);
            return;
        }

        switch (currCommand) {
            case InputCommand.None: StartIdle(); break;
            case InputCommand.MoveLeft: {
                StartMove(CharacterFacing.Left);
                break;
            }
            case InputCommand.MoveRight: {
                StartMove(CharacterFacing.Right);
                break;
            }
            case InputCommand.DodgeLeft: StartDodge(CharacterFacing.Left); break;
            case InputCommand.DodgeRight: StartDodge(CharacterFacing.Right); break;
            case InputCommand.Punch: StartPunch(); break;
            case InputCommand.Kick: StartKick(); break;
            case InputCommand.FireBall: StartFireBall(); break;
            case InputCommand.HighBlock: StartFireBall(); break;
            case InputCommand.LowBlock: StartFireBall(); break;
        }
    }

    private void UpdateDodge() {
        
    }

    private void UpdatePunch() {
        if (incomingStun > .0f) {
            StartStunned(incomingStun);
            return;
        }
    }

    private void UpdateKick() {
        if (incomingStun > .0f) {
            StartStunned(incomingStun);
            return;
        }
    }

    private void UpdateFireBall() {
        if (incomingStun > .0f) {
            StartStunned(incomingStun);
            return;
        }
    }

    private void UpdateHighBlock() {
        
    }

    private void UpdateLowBlock() {
        
    }

    private void UpdateStunned() {
        
    }

    public void SendCommand(InputCommand command) {
        currCommand = command;
    }
}
