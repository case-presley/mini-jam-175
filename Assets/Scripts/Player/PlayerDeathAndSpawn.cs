using UnityEngine;

public class PlayerDeathAndSpawn : MonoBehaviour
{
    public GameObject player;                           // Reference to the player prefab
    public Transform mapOneSpawn;                       // Reference to the spawn point
    public bool onMapOne = true;                        // Flag to track if the player is on Map One

    private GameObject currentPlayerInstance;           // Current player instance

    void Start()
    {
        // Check if a player instance exists at the start
        if (currentPlayerInstance == null)
        {
            Debug.Log("No player found. Spawning player...");
            Spawn("MapOne"); // Spawn the player on Map One as default
        }
    }

    public void Death()
    {
        // Destroy the current player instance if it exists
        if (currentPlayerInstance != null)
        {
            Destroy(currentPlayerInstance);
        }

        // Handle respawning
        if (onMapOne)
        {
            Spawn("MapOne");
        }
        else
        {
            Debug.LogWarning("Player is not on a recognized map.");
        }
    }

    public void Spawn(string map)
    {
        if (map == "MapOne")
        {
            if (mapOneSpawn == null)
            {
                Debug.LogError("MapOneSpawn is not assigned!");
                return;
            }

            // Spawn the player at the specified spawn point
            currentPlayerInstance = Instantiate(player, mapOneSpawn.position, mapOneSpawn.rotation);

            // Update the camera's target
            CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                cameraFollow.UpdateTarget(currentPlayerInstance.transform);
            }

            Debug.Log("Player spawned on Map One.");
        }
        else
        {
            Debug.LogWarning("Cannot spawn player on the specified map.");
        }
    }
}
