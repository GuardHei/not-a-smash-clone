using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMotor : MonoBehaviour {
    
    [Header("Movement Settings")]
    [Range(.001f, 10.0f)]
    public float moveSpeed = 1.0f;
    [Range(.0f, 1.0f)]
    public float pushForce = .5f;
    public LayerMask canPush;
    public LayerMask blockedBy;

    [Header("Events")]
    public UnityAction<CharacterFacing, CharacterFacing> onDirectionChanged;

    [Header("References")]
    public Rigidbody2D rb;

    [Header("Status")]
    public CharacterFacing facing = CharacterFacing.Right;

    private RaycastHit2D[] moveHits = new RaycastHit2D[100];

    private void Awake() {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        rb.isKinematic = true;
    }

    [Header("Movement Debug")]
    public GameObject d_other;
    public CharacterMotor d_needsToPush;
    public float d_distance;
    public float d_direction;
    public int d_hitCount;
    public float d_closestDistance;
    public float d_remainingDistance;

    private void ClearDebugVariables() {
        d_other = null;
        d_needsToPush = null;
        d_distance = .0f;
        d_direction = .0f;
        d_hitCount = 0;
        d_closestDistance = .0f;
        d_remainingDistance = .0f;
    }

    public void SweepAndMove(float distance, float direction, bool pushes) {
        if (rb == null) return;
        
        ClearDebugVariables();

        var currPos = transform.position;
        
        var moveFilter = new ContactFilter2D {
            useTriggers = false,
            useLayerMask = true,
            layerMask = canPush | blockedBy,
        };

        var displacement = direction * distance;
        
        var hitCount = rb.Cast(new Vector2(direction, .0f), moveFilter, moveHits, distance);

        d_distance = distance;
        d_direction = direction;
        d_hitCount = hitCount;
        
        if (hitCount == 0) Move(displacement);
        else {
            var closestDistance = distance;
            CharacterMotor needsToPush = null;
            for (var i = 0; i < hitCount; i++) {
                var moveHit = moveHits[i];
                
                if (direction > .0f && moveHit.point.x < currPos.x) continue;
                if (direction < .0f && moveHit.point.x > currPos.x) continue;

                if (moveHit.distance < closestDistance) {
                    closestDistance = moveHit.distance;
                    var other = moveHit.transform.gameObject;
                    needsToPush = canPush.Contains(other.layer) ? other.GetComponent<CharacterMotor>() : null;

                    d_other = other;
                } else if (moveHit.distance == closestDistance) {
                    if (needsToPush != null) continue;
                    var other = moveHit.transform.gameObject;
                    needsToPush = canPush.Contains(other.layer) ? other.GetComponent<CharacterMotor>() : null;
                    
                    d_other = other;
                }
            }

            var remainingDistance = distance - closestDistance;
            
            d_needsToPush = needsToPush;
            d_closestDistance = closestDistance;
            d_remainingDistance = remainingDistance;
            
            if (!pushes || needsToPush == null || remainingDistance <= float.Epsilon) Move(closestDistance * direction);
            else {
                remainingDistance *= pushForce;
                needsToPush.SweepAndMove(remainingDistance, direction, false);
                SweepAndMove(remainingDistance, direction, false);
            }
        }
    }

    public void Move(float displacement) {
        var pos = transform.position;
        pos.x += displacement;
        transform.position = pos;
    }
    
    public void Turn() {
        if (facing == CharacterFacing.Right) TurnLeft();
        else TurnRight();
    }

    public void TurnRight() {
        if (facing == CharacterFacing.Right) return;
        onDirectionChanged?.Invoke(CharacterFacing.Left, CharacterFacing.Right);
    }

    public void TurnLeft() {
        if (facing == CharacterFacing.Left) return;
        onDirectionChanged?.Invoke(CharacterFacing.Right, CharacterFacing.Left);
    }
}

public enum CharacterFacing {
    Right = 1,
    Left = -1
}