using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HitBox : MonoBehaviour {

    [Header("Settings")]
    public bool destroyOnHit;
    public LayerMask mask;
    public string canDamage;
    public int damage;
    public int stunFrames;
    public float stunPush;
    public bool canBeHighBlocked;
    public bool canBeLowBlocked;
    public UnityAction onHit;

    [Header("Status")]
    public CharacterFacing direction;
    public bool isEndOfCombo;
    public bool activated;
    public List<Damageable> hits = new();
    private List<Collider2D> results = new();

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
        var damaged = false;
        for (var i = 0; i < hitCount; i++) {
            var hit = results[i];
            if (!hit.CompareTag(canDamage)) continue;
            if (!hit.TryGetComponent(out Damageable damageable)) continue;
            if (hits.Contains(damageable)) continue;
            hits.Add(damageable);

            damaged = true;
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
                    
            if (damageable.TakeDamage(damageInfo)) onHit?.Invoke();
        }
        
        if (damaged && destroyOnHit) Destroy(gameObject);
    }
}
