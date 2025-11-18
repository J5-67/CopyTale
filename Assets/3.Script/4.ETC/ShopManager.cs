using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("--- UI 참조 ---")]
    [SerializeField] private GameObject shopUI; // 상점 전체 패널
    [SerializeField] private TMP_Text dialogueText; // 상인 대화 텍스트
    [SerializeField] private GameObject menuHeart; // 선택 하트 GameObject

    [Header("--- 메인 메뉴 ---")]
    [SerializeField] private Transform[] mainMenuPositions; // 구매, 판매, 대화, 나가기 위치
    [SerializeField] private GameObject buyMenu; // 구매 메뉴 패널
    [SerializeField] private GameObject sellMenu; // 판매 메뉴 패널

    [Header("--- 아이템 목록 ---")]
    [SerializeField] private List<ItemData> itemsToSell = new List<ItemData>(); // 이 상점에서 판매할 아이템 목록

    // 상인 대화 목록
    private string[] shopkeeperDialogues = {
        "안녕! 물건 좀 구경할래?",
        "오늘 날씨가 참 좋지?",
        "언제든지 다시 와!"
    };

    private enum ShopState { MainMenu, BuyMenu, SellMenu, Talking }
    private ShopState currentState = ShopState.MainMenu;
    private int currentMenuIndex = 0; // 메인 메뉴 인덱스 (0:구매, 1:판매, 2:대화, 3:나가기)
    private const int MAIN_MENU_COUNT = 4;

    private void Start()
    {
        // 초기화 시 상점 UI를 비활성화 상태로 둡니다.
        shopUI.SetActive(false);
    }

    private void Update()
    {
        // 상점 UI가 켜져 있을 때만 입력 처리
        if (!shopUI.activeSelf) return;

        // Z 키 (확인/선택) 처리
        if (Input.GetKeyDown(KeyCode.Z))
        {
            HandleConfirm();
        }

        // X 키 (취소/뒤로가기) 처리
        if (Input.GetKeyDown(KeyCode.X))
        {
            HandleCancel();
        }

        // 방향키 입력 처리 (메뉴 이동)
        HandleMovementInput();
    }

    public void OpenShop()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[ShopManager] GameManager가 없습니다!");
            return;
        }

        shopUI.SetActive(true); // 상점 UI 활성화
        dialogueText.text = "어서 와! 뭘 도와줄까?"; // 초기 대화 설정
        SetState(ShopState.MainMenu); // 메인 메뉴로 시작
    }

    private void SetState(ShopState newState)
    {
        currentState = newState;
        buyMenu.SetActive(false);
        sellMenu.SetActive(false);
        menuHeart.SetActive(false); // 일단 메인 메뉴가 아니면 하트 숨기기

        switch (newState)
        {
            case ShopState.MainMenu:
                // 메인 메뉴 UI를 보여줍니다.
                buyMenu.SetActive(false);
                sellMenu.SetActive(false);
                menuHeart.SetActive(true);
                currentMenuIndex = 0; // 인덱스 초기화
                MoveHeartToMenu(currentMenuIndex); // 하트 위치 초기화
                break;
            case ShopState.BuyMenu:
                // 구매 메뉴 UI를 활성화하고 목록을 보여줍니다.
                buyMenu.SetActive(true);
                // 여기에 구매 메뉴 목록 생성 및 초기화 로직이 필요합니다.
                break;
            case ShopState.SellMenu:
                // 판매 메뉴 UI를 활성화하고 인벤토리 목록을 보여줍니다.
                sellMenu.SetActive(true);
                //
                //여기에 판매 메뉴 목록 생성 및 초기화 로직이 필요합니다.
                break;
            case ShopState.Talking:
                // 대화 상태에서는 메뉴 이동을 막고 대화만 표시합니다.
                menuHeart.SetActive(false);
                break;
        }
    }

    private void HandleMovementInput()
    {
        if (currentState != ShopState.MainMenu) return; // 메인 메뉴에서만 이동 가능

        int direction = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = -1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = 1;
        }

        if (direction != 0)
        {
            // 인덱스를 순환하며 이동
            currentMenuIndex = (currentMenuIndex + direction + MAIN_MENU_COUNT) % MAIN_MENU_COUNT;
            MoveHeartToMenu(currentMenuIndex);
        }
    }

    private void MoveHeartToMenu(int index)
    {
        if (index >= 0 && index < mainMenuPositions.Length)
        {
            menuHeart.transform.position = mainMenuPositions[index].position;
        }
    }

    private void HandleConfirm()
    {
        switch (currentState)
        {
            case ShopState.MainMenu:
                switch (currentMenuIndex)
                {
                    case 0: // 구매
                        dialogueText.text = "뭘 사려고 해? ";
                        SetState(ShopState.BuyMenu);
                        break;
                    case 1: // 판매
                        dialogueText.text = "내가 가진 물건을 보여줘. ";
                        SetState(ShopState.SellMenu);
                        break;
                    case 2: // 대화하기
                        // 상인 대화 목록 중 랜덤하게 하나 선택
                        string randomDialogue = shopkeeperDialogues[Random.Range(0, shopkeeperDialogues.Length)];
                        dialogueText.text = randomDialogue + " ";
                        SetState(ShopState.Talking);
                        break;
                    case 3: // 나가기
                        dialogueText.text = "안녕히 가렴! ";
                        CloseShop();
                        break;
                }
                break;
            case ShopState.BuyMenu:
                // BuyItem(selectedItem);
                break;
            case ShopState.SellMenu:
                // SellItem(selectedItem);
                break;
            case ShopState.Talking:
                // 대화 모드에서 Z를 누르면 메인 메뉴로 돌아갑니다.
                dialogueText.text = "뭘 도와줄까?";
                SetState(ShopState.MainMenu);
                break;
        }
    }

    private void HandleCancel()
    {
        // X 키는 항상 메인 메뉴로 돌아가는 역할
        if (currentState != ShopState.MainMenu)
        {
            dialogueText.text = "뭘 도와줄까?";
            SetState(ShopState.MainMenu);
        }
    }

    private void CloseShop()
    {
        // 1초 뒤에 UI를 끄고 플레이어를 상점에서 나가게 합니다.
        StartCoroutine(CloseShopAfterDelay(1f));
    }

    private IEnumerator CloseShopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        shopUI.SetActive(false);
        
        // 예: PlayerController.Instance.SetCanMove(true);
    }

}