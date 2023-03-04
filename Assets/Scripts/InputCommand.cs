using System;

[Flags]
public enum InputCommand {
    None = 0,
    MoveLeft = 1 << 0,
    MoveRight = 1 << 1,
    DodgeLeft = 1 << 2,
    DodgeRight = 1 << 3,
    Punch = 1 << 4,
    Kick = 1 << 5,
    FireBall = 1 << 6,
    HighBlock = 1 << 7,
    LowBlock = 1 << 8
}
