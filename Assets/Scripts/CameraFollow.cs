using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // The player object to follow
    public float smoothSpeed = 2f; // Adjust for smoothness
    public Vector3 offset;         // Offset to maintain distance from the player

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            desiredPosition.z = -10f;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
        else
        {
            Debug.LogWarning("Camera target is missing.");
        }
    }

    // Method to update the camera target
    public void UpdateTarget(Transform newTarget)
    {
        target = newTarget;
        Debug.Log("Camera target updated.");
    }
}
