using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFaker : MonoBehaviour {

    public Transform realThing;
    public Transform listener;
    public Vector3 offset;

    public void Update() {
        if (realThing == null || listener == null) return;
        var newPos = realThing.position;
        var listenerX = listener.position.x;
        newPos.z = Mathf.Abs(newPos.x - listenerX);
        newPos.x = listenerX;
        transform.position = newPos + offset;
    }
}
