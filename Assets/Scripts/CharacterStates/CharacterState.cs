using System;

[Flags]
public enum CharacterState {
    Idle = 1 << 0,
    Move = 1 << 1,
    Punch = 1 << 2,
    Kick = 1 << 3,
    FireBall = 1 << 4,
    HighBlock = 1 << 5,
    LowBlock = 1 << 6,
    Dodge = 1 << 7,
    Stunned = 1 << 8
}