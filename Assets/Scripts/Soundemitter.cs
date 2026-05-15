using UnityEngine;

/// <summary>
/// Attach this to objects that can make noise (player, dropped items, doors, etc.)
/// </summary>
public class SoundEmitter : MonoBehaviour
{
    [Header("Sound Settings")]
    public float soundRadius = 15f;  // How far the sound travels
    public bool debugMode = false;

    /// <summary>
    /// Call this when something makes a noise
    /// </summary>
    public void EmitSound(Vector3 soundPosition, float radiusOverride = -1f)
    {
        float actualRadius = radiusOverride > 0 ? radiusOverride : soundRadius;

        if (debugMode)
        {
            Debug.Log("Sound emitted at " + soundPosition + " with radius " + actualRadius);
        }

        // Find all AI enemies in the scene
        GrannyStyleAI[] enemies = Object.FindObjectsByType<GrannyStyleAI>(FindObjectsSortMode.None);

        foreach (GrannyStyleAI enemy in enemies)
        {
            // Tell each enemy about the sound
            enemy.HearNoise(soundPosition);
        }
    }

    /// <summary>
    /// Shortcut to emit sound at this object's position
    /// </summary>
    public void EmitSoundHere()
    {
        EmitSound(transform.position);
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        if (debugMode)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, soundRadius);
        }
    }
}