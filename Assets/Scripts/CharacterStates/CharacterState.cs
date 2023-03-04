using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class CharacterState {

    /// Character States
    /// 0. Idle -> [1, 2, 3, 4, 5, 6, 7, 8] -> () => {0}
    /// 1. Move -> [1, 2, 3, 4, 5, 6, 7, 8] -> () => {0}
    /// 2. Punch (Upper attack) -> [5, 6, 7, 8] -> (1, 2, 3, 4, 5, 6, 7) => {0}
    /// 3. Kick (Lower attack) -> [5, 6, 7, 8] -> (1, 2, 3, 4, 5, 6, 7) => {0}
    /// 4. Fireball (Special attack) -> [5, 6, 7, 8] -> (1, 2, 3, 4, 5, 6, 7) => {0}
    /// 5. Upper Block -> [8] -> (1, 2, 3, 4, 5, 6, 7) => {0}
    /// 6. Lower Block -> [8] -> (1, 2, 3, 4, 5, 6, 7) => {0}
    /// 7. Dodge -> [] -> (1, 2, 3, 4, 5, 6, 7) => {0}
    /// 8. Stunned -> [] -> () => {0}

    public CharacterStateType type = CharacterStateType.Idle;
    public CharacterStateType canInterrupt;
    public CharacterStateType defaultTransition = CharacterStateType.Idle;

    public abstract bool IsFinished {
        get;
    }

    public virtual CharacterState Interrupt(CharacterState source) {
        if (IsFinished) return source;
        return canInterrupt.HasFlag(source.type) ? source : this;
    }
}

[Flags]
public enum CharacterStateType {
    Idle = 1 << 0,
    Move = 1 << 1,
    Punch = 1 << 2,
    Kick = 1 << 3,
    Fireball = 1 << 4,
    UpperBlock = 1 << 5,
    LowerBlock = 1 << 6,
    Dodge = 1 << 7,
    Stunned = 1 << 8
}