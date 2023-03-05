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
    Stunned = 1 << 8,
    BlockRecovery = 1 << 9
}

public enum AnimCharacterState {
    Idle = 0,
    MoveFront = 1,
    MoveBack = 2,
    Punch = 3,
    Kick = 4,
    FireBall = 5,
    HighBlock = 6,
    LowBlock = 7,
    Stunned = 8,
    BlockRecovery = 9
}