/// <summary>
/// Global player settings. Things like player speed coefficient and interaction distance. 
/// </summary>
public static class GlobalPlayerConfig {
    public static float PlayerGroundCheckRadius { get; set; } = 0.4f;
    /// <summary>
    /// How fast can the player move.
    /// </summary>
    public static float PlayerSpeed { get; set; } = 7;
    /// <summary>
    /// How quickly the player accelerates/decelerates toward target velocity.
    /// </summary>
    public static float PlayerAcceleration { get; set; } = 75f;
    /// <summary>
    /// Multiplier applied to PlayerAcceleration while airborne.
    /// </summary>
    public static float AirControlMultiplier { get; set; } = 0.25f;
    /// <summary>
    /// Multiplier applied to PlayerAcceleration while sliding. Code is made with it being 0 in mind.
    /// </summary>
    public static float SlidingControlMultiplier { get; set; } = 0f;
    /// <summary>
    /// How much to multiply the speed by when player is crouching.
    /// </summary>
    public static float PlayerCrouchSpeedMultiplier { get; set; } = 0.3f;
    /// <summary>
    /// How much to multiply the speed by when player is sprinting.
    /// </summary>
    public static float PlayerSprintSpeedMultiplier { get; set; } = 1.5f;
    /// <summary>
    /// Height of the player's capuse collider when not crouching.
    /// </summary>
    public static float PlayerStandingHeight { get; set; } = 2f;
    /// <summary>
    /// Height of the player's capuse collider when crouching.
    /// </summary>
    public static float PlayerCrouchingHeight { get; set; } = 1f;
    /// <summary>
    /// Y coordinate of the campivot when not crouching.
    /// </summary>
    public static float PlayerCameraStandingHeight { get; set; } = 0.5f;
    /// <summary>
    /// Y coordinate of the campivot when crouching.
    /// </summary>
    public static float PlayerCameraCrouchingHeight { get; set; } = 0.2f;
    /// <summary>
    /// How much force to apply when jumping.
    /// </summary>
    public static float JumpForce { get; set; } = 7f;
    /// <summary>
    /// How much force to apply when groundpounding. (should be negative)
    /// </summary>
    public static float GroundPoundForce { get; set; } = -14f;
    /// <summary>
    /// How much gravity to apply downwards when not grounded.
    /// </summary>
    public static float Gravity { get; set; } = 15f;
    /// <summary>
    /// At what distance can the player interact with an interactable.
    /// </summary>
    public static float InteractionDistance { get; set; } = 3;
    /// <summary>
    /// What the player can walk on. 1<<0 | 1<<1 would take the first two layers.
    /// </summary>
    public static int GroundLayerMask { get; set; } = 1 << 0;
}
