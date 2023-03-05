using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    [Header("Settings")]
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode punch = KeyCode.J;
    public KeyCode kick = KeyCode.K;
    public KeyCode fireball = KeyCode.O;
    public KeyCode highBlock = KeyCode.U;
    public KeyCode lowBlock = KeyCode.I;

    [Header("References")]
    public StateController controller;

    [Header("Status")]
    public InputCommand command;
    public bool movedLeft;
    public bool movedRight;
    public bool punched;
    public bool kicked;
    public bool fireballed;
    public bool highBlocking;
    public bool lowBlocking;

    private void Awake() {
        if (controller == null) controller = GetComponent<StateController>();
    }

    private void Update() {
        if (controller == null) return;

        movedLeft = Input.GetKey(moveLeft);
        movedRight = Input.GetKey(moveRight);
        punched = Input.GetKeyDown(punch);
        kicked = Input.GetKeyDown(kick);
        fireballed = Input.GetKeyDown(fireball);
        highBlocking = Input.GetKey(highBlock);
        lowBlocking = Input.GetKey(lowBlock);

        command = InputCommand.None;
        if (movedLeft) command = InputCommand.MoveLeft;
        if (movedRight) command = InputCommand.MoveRight;
        if (punched) command = InputCommand.Punch;
        if (kicked) command = InputCommand.Kick;
        if (fireballed) command = InputCommand.FireBall;
        if (highBlocking) command = InputCommand.HighBlock;
        if (lowBlocking) command = InputCommand.LowBlock;
        
        controller.SendCommand(command);
    }
}
