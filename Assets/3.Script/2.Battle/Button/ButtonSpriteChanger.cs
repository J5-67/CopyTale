using UnityEngine;
using UnityEngine.UI; // UI Image 컴포넌트를 사용하기 위해 필요

public class ButtonSpriteChanger : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; // 2D Sprite 오브젝트의 SpriteRenderer

    // 유니티 에디터에서 설정할 스프라이트 변수들
    [Header("스프라이트 설정")]
    [SerializeField]
    private Sprite originalSprite;  // 버튼의 기본 스프라이트
    [SerializeField]
    private Sprite highlightSprite; // 버튼이 하이라이트될 때 사용할 스프라이트

    void Awake()
    {
        // Awake에서 컴포넌트 참조를 가져옵니다.
        //spriteRenderer = GetComponent<SpriteRenderer>();
        TryGetComponent(out spriteRenderer);
    }

    // 플레이어가 영역에 진입했을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetSprite(highlightSprite); // 하이라이트 스프라이트로 변경
        }
    }

    // 플레이어가 영역에서 이탈했을 때
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SetSprite(originalSprite); // 원래 스프라이트로 복구
        }
    }

    // 스프라이트를 설정하는 내부 도우미 함수
    private void SetSprite(Sprite targetSprite)
    {
        if (targetSprite == null) return; // 설정된 스프라이트가 없으면 아무것도 하지 않음

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = targetSprite;
        }
    }
}