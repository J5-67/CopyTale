using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    private Movement2D movement2D;
    private Animator playerAnimator;

    private string isMovingParam = "IsMoving";
    private string animXParam = "AnimX";
    private string animYParam = "AnimY";

    private void Awake()
    {
        TryGetComponent(out movement2D);
        TryGetComponent(out playerAnimator);
    }
    void Update()
    {
        //인풋시스템
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        movement2D.MoveTo(new Vector3(inputX, inputY, 0f));

        //애니메이션 파라미터
        bool isMoving = (Mathf.Abs(inputX) > 0f || Mathf.Abs(inputY) > 0f);
        float finalAnimX = 0f;
        float finalAnimY = 0f;

        if (isMoving)
        {
            if (Mathf.Abs(inputX) > 0f)
            {
                finalAnimX = inputX;
                finalAnimY = 0f;
            }
            else
            {
                finalAnimX = 0f;
                finalAnimY = inputY;
            }
        }

        playerAnimator.SetBool(isMovingParam, isMoving);
        playerAnimator.SetFloat(animXParam, finalAnimX);
        playerAnimator.SetFloat(animYParam, finalAnimY);
    }
}
