using System;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {

    public Health health;

    private void Awake() {
        if (health == null) health = GetComponent<Health>();
    }

    public void TakeDamage() {
        
    }
}
