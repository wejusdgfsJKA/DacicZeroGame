using UnityEngine;

/// <summary>
/// Damage package.
/// </summary>
public struct DmgInfo
{
    /// <summary>
    /// How much damage this attack has dealt.
    /// </summary>
    public int Damage { get; set; }
    /// <summary>
    /// The source of the damage.
    /// </summary>
    public Transform Source { get; set; }
    /// <summary>
    /// The collider we hit.
    /// </summary>
    public Collider ColliderHit { get; set; }
}
