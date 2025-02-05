using UnityEngine;

/// <summary>
/// Controls the camera to smoothly follow the player.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Tooltip("The target the camera should follow.")]
    public Transform target;

    [Tooltip("The speed at which the camera follows the player.")]
    public float smoothSpeed = 5f;

    [Tooltip("Offset from the player's position to maintain distance.")]
    public Vector3 offset = new Vector3(0f, 2f, -10f);

    /// <summary>
    /// Updates the camera's position to follow the target smoothly.
    /// </summary>
    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera target is missing. Camera will not follow.");
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = -10f; // Ensure the camera stays in the correct depth

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        smoothedPosition.z = -10f; // Reinforce z-position lock

        transform.position = smoothedPosition;
    }

    /// <summary>
    /// Updates the target the camera follows.
    /// </summary>
    /// <param name="newTarget">The new target transform.</param>
    public void UpdateTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            Debug.LogError("UpdateTarget() received a null target.");
            return;
        }

        target = newTarget;
        Debug.Log("Camera target updated to: " + newTarget.name);
    }
}
