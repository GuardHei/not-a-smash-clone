using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

    public static bool OutputsDebugInfo = false;

    public static bool Contains(this LayerMask mask, int layer) => mask == (mask | (1 << layer));
    
    public static void Print(string msg) {
#if UNITY_EDITOR
        if (OutputsDebugInfo) Debug.Log(msg);
#endif
    }
}