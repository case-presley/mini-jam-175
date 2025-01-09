using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag("Player")) // Check if the colliding object is tagged as "Player"
		{
    		// Get the PlayerDeathAndSpawn component attached to the colliding GameObject
    		PlayerDeathAndSpawn deathScript = collider.GetComponent<PlayerDeathAndSpawn>();
    
    		// Check if the component exists before calling the method
    		if (deathScript != null)
    		{
        		deathScript.Death(); // Call the Death method on the specific instance
    		}
    		else
   	 		{
        		Debug.LogError("PlayerDeathAndSpawn component is missing on the player object.");
    		}
		}
	}
}
