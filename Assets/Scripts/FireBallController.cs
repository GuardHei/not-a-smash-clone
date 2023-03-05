using System;
using UnityEngine;

[RequireComponent(typeof(HitBox))]
public class FireBallController : MonoBehaviour {

    public int frames;
    public float speed;
    public CharacterFacing direction;
    public HitBox hitBox;

    private void Awake() {
        if (hitBox == null) hitBox = GetComponent<HitBox>();
        if (hitBox != null) hitBox.Activate(direction);
    }

    private void Update() {
        var pos = transform.position;
        pos.x += (int) direction * speed;
        transform.position = pos;
        frames -= 1;
        if (frames == 0) Destroy(this);
    }
}