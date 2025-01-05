using UnityEngine;

public class SpikeTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player hit spike. ");
            other.GetComponent<PlayerDeathAndSpawn>().Death();
        }
    }
}
