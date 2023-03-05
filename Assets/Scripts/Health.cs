using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour {

    [Range(1, 1000)]
    public int maxHealth;
    public int currHealth;
    public bool dead;
    
    public UnityEvent onHit;
    public UnityEvent onHeal;
    public UnityEvent onDeath;

    private void Awake() {
        currHealth = maxHealth;
    }

    public void Hit(int damage) {
        if (dead) return;
        damage = Mathf.Max(damage, 0);
        currHealth = Mathf.Max(currHealth - damage, 0);
        onHit?.Invoke();
        if (currHealth <= 0) Die();
    }

    public void Heal(int healing) {
        if (dead) return;
        healing = Mathf.Max(healing, 0);
        currHealth = Mathf.Min(currHealth + healing, maxHealth);
        onHeal?.Invoke();
    }

    private void Die() {
        if (dead) return;
        dead = true;
        onDeath?.Invoke();
    }

    public void SelfDestroy() => Destroy(gameObject);
}
