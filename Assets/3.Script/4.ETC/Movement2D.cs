using UnityEngine;

public class Movement2D : MonoBehaviour
{
    public float Move_Speed = 0f;

    [SerializeField]
    private Vector3 Move_Direction = Vector3.zero;

    public void MoveTo(Vector3 Direction)
    {
        Move_Direction = Direction;
    }

    private void Update()
    {
        transform.position += Move_Direction * Move_Speed * Time.deltaTime;
    }
}
