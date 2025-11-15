using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject player;
    private Vector3 camera_position = new Vector3(0f, 0f, -10f);

    private void Start()
    {
        //camera = Camera.main;
    }
    void LateUpdate()
    {
        camera.transform.position = player.transform.position + camera_position;
    }
}
