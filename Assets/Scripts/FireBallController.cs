using System;
using UnityEngine;

[RequireComponent(typeof(HitBox))]
public class FireBallController : MonoBehaviour {

    public int frames;
    public float speed;
    public CharacterFacing direction;
    public HitBox hitBox;
    public Damageable damageable;
    public AudioClip hitSfx;
    [Range(.0f, 1.0f)]
    public float hitVol = 1.0f;

    private void Awake() {
        if (hitBox == null) hitBox = GetComponent<HitBox>();
        if (hitBox != null) hitBox.Activate(direction);
        if (damageable != null) damageable.onDamaged += (dmg, info) => SFXManager.PlaySFX(hitSfx, info.hitPosition, hitVol, 128);
    }

    private void Update() {
        var pos = transform.position;
        pos.x += (int) direction * speed;
        transform.position = pos;
        frames -= 1;
        if (frames == 0) Destroy(this);
    }
}