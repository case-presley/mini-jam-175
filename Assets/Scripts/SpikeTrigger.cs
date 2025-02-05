using UnityEngine;

/// <summary>
/// Detects when the player collides with spikes and triggers the death sequence.
/// </summary>
public class SpikeTrigger : MonoBehaviour
{
    /// <summary>
    /// Called when a collider enters the trigger zone.
    /// </summary>
    /// <param name="collider">The collider that entered the trigger.</param>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            // Get the PlayerDeath component from the colliding object
            PlayerDeath deathScript = collider.GetComponent<PlayerDeath>();

            if (deathScript != null)
            {
                deathScript.Death();
            }
            else
            {
                Debug.LogError("PlayerDeath component is missing on the player object.");
            }
        }
    }
}
