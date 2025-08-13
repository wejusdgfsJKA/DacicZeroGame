using System.Collections.Generic;
using UnityEngine;

public static class GlobalSettings
{
    public static Dictionary<int, LayerMask> TargetMasks { get; } = new() {
        {6, 1 << 7},
        {7, 1 << 6}
    };
    public static int MaxTargets { get; } = 20;
}
