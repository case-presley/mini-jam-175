using UnityEngine;

/// <summary>
/// Handles spawning and respawning the player at a designated spawn point.
/// </summary>
public class PlayerSpawn : MonoBehaviour
{
    [Tooltip("The player prefab to spawn.")]
    public GameObject playerPrefab;

    private Transform spawnPoint;
    private GameObject currentPlayerInstance;

    /// <summary>
    /// Initializes the spawn system and spawns the player at game start.
    /// </summary>
    private void Start()
    {
        // Find the spawn point dynamically using a tag
        GameObject foundSpawn = GameObject.FindWithTag("SpawnPoint");

        if (foundSpawn != null)
        {
            spawnPoint = foundSpawn.transform;
            Debug.Log("Spawn point found at " + spawnPoint.position);
        }
        else
        {
            Debug.LogError("Spawn point not found. Ensure it is tagged correctly.");
            return;
        }

        // Spawn the player at the start of the game
        PlayerSpawnMethod();
    }

    /// <summary>
    /// Spawns a new player instance at the designated spawn point.
    /// </summary>
    public void PlayerSpawnMethod()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point is missing. Cannot spawn player.");
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is missing. Assign it in the Inspector.");
            return;
        }

        // Destroy the old player instance before spawning a new one
        if (currentPlayerInstance != null)
        {
            Destroy(currentPlayerInstance);
            Debug.Log("Previous player instance destroyed.");
        }

        // Spawn a new player instance
        currentPlayerInstance = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log("Player spawned at " + spawnPoint.position);

        // Update the camera to follow the new player
        CameraFollow cameraFollow = Object.FindFirstObjectByType<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.UpdateTarget(currentPlayerInstance.transform);
            Debug.Log("Camera now follows the new player.");
        }
        else
        {
            Debug.LogError("No CameraFollow script found in the scene.");
        }
    }
}
