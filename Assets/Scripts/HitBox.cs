using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour {

    [Header("Settings")]
    public LayerMask mask;
    public string canDamage;
    public int damage;
    public int stunFrames;
    public float stunPush;
    public bool canBeHighBlocked;
    public bool canBeLowBlocked;

    [Header("Status")]
    public CharacterFacing direction;
    public bool isEndOfCombo;
    public bool activated;
    public BoxCollider2D box;
    public List<Damageable> hits = new();
    private List<Collider2D> results = new();

    private void Awake() {
        box = GetComponent<BoxCollider2D>();
        box.isTrigger = true;
        box.enabled = false;

        activated = false;
    }

    public void Activate(CharacterFacing direction, bool isEndOfCombo = false) {
        if (activated) return;
        
        this.direction = direction;
        this.isEndOfCombo = isEndOfCombo;
        
        hits.Clear();
        activated = true;
        gameObject.SetActive(true);
    }

    public void Deactivate() {
        if (!activated) return;
        
        hits.Clear();
        activated = false;
        gameObject.SetActive(false);
    }

    private void Update() {
        if (!activated) return;
        var overlapFilter = new ContactFilter2D {
            useTriggers = true,
            useLayerMask = true,
            layerMask = mask
        };
        
        var hitCount = Physics2D.OverlapBox(transform.position, transform.lossyScale, .0f, overlapFilter, results);
        for (var i = 0; i < hitCount; i++) {
            var hit = results[i];
            if (!hit.CompareTag(canDamage)) continue;
            if (!hit.TryGetComponent(out Damageable damageable)) continue;
            if (hits.Contains(damageable)) continue;
            hits.Add(damageable);
                    
            var damageInfo = new DamageInfo {
                damage = damage,
                stunFrames = stunFrames,
                stunPush = stunPush,
                canBeHighBlocked = canBeHighBlocked,
                canBeLowBlocked = canBeLowBlocked,
                direction = direction,
                isEndOfCombo = isEndOfCombo,
                hitPosition = transform.position
            };
                    
            damageable.TakeDamage(damageInfo);
        }
    }
}
