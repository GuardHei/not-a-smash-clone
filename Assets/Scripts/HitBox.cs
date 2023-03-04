using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class HitBox : MonoBehaviour {

    [Header("Settings")]
    public LayerMask mask;
    public string tag;
    public int damage;
    public bool canBeHighBlocked;
    public bool canBeLowBlocked;

    [Header("Status")]
    public CharacterFacing direction;
    public bool isEndOfCombo;
    private BoxCollider2D box;
    private List<Damageable> hits = new();
    private Collider2D[] results = new Collider2D[5];

    private void Awake() {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.enabled = false;
    }

    private void Activate(CharacterFacing direction, bool isEndOfCombo = false) {
        this.direction = direction;
        this.isEndOfCombo = isEndOfCombo;
        
        hits.Clear();
        gameObject.SetActive(true);
    }

    private void Deactivate() {
        gameObject.SetActive(false);
    }

    private void Update() {
        var bounds = box.bounds;
        var hitCount = Physics2D.OverlapBoxNonAlloc(bounds.center, bounds.size, .0f, results, mask);
        for (var i = 0; i < hitCount; i++) {
            var hit = results[i];
            if (!hit.CompareTag(tag)) continue;
            if (!TryGetComponent(out Damageable damageable)) continue;
            if (hits.Contains(damageable)) continue;
            hits.Add(damageable);
                    
            var damageInfo = new DamageInfo {
                damage = damage,
                canBeHighBlocked = canBeHighBlocked,
                canBeLowBlocked = canBeLowBlocked,
                direction = direction,
                isEndOfCombo = isEndOfCombo,
                hitPosition = bounds.center
            };
                    
            damageable.TakeDamage(damageInfo);
        }
    }
}
