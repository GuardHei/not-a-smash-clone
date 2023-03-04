using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CMotor : MonoBehaviour {

    public float speed;

    public Rigidbody2D rb;

    private void Awake() {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        // rb.isKinematic = true;
    }

    private void Update() {
        if (Input.GetKey(KeyCode.A)) {
            rb.velocity = new Vector2(-speed, 0f);
        } else if (Input.GetKey(KeyCode.D)) {
            rb.velocity = new Vector2(speed, 0f);
        } else {
            rb.velocity = Vector2.zero;
        }
    }
}
