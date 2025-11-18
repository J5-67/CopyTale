using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTarget;
    private Vector3 offset = new Vector3(0f, 0f, -10f);

    void LateUpdate()
    {
        if (playerTarget != null)
        {
            transform.position = playerTarget.position + offset;
        }
    }
}