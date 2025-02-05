using UnityEngine;

/// <summary>
/// Handles player death and triggers the respawn process.
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    private PlayerSpawn playerSpawn;

    /// <summary>
    /// Finds the PlayerSpawn instance in the scene at the start.
    /// </summary>
    private void Start()
    {
        playerSpawn = Object.FindFirstObjectByType<PlayerSpawn>();

        if (playerSpawn == null)
        {
            Debug.LogError("PlayerSpawn component is missing in the scene.");
        }
    }

    /// <summary>
    /// Handles the player's death by destroying the object and triggering the respawn process.
    /// </summary>
    public void Death()
    {
        Debug.Log("Player has died.");

        // Destroy the player instance and trigger respawn
        Destroy(gameObject);
        RespawnPlayer();
    }

    /// <summary>
    /// Calls the PlayerSpawn method to respawn the player.
    /// </summary>
    private void RespawnPlayer()
    {
        if (playerSpawn != null)
        {
            playerSpawn.PlayerSpawnMethod();
        }
        else
        {
            Debug.LogError("PlayerSpawn is missing. Respawn failed.");
        }
    }
}
