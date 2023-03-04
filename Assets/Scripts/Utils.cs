using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

    public static bool Contains(this LayerMask mask, int layer) => mask == (mask | (1 << layer));
}