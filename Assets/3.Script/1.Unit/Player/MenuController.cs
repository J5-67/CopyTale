using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("--- UI 참조 ---")]
    [SerializeField] private GameObject inventoryPanel; // 인벤토리 전체 패널 (켜고 끄기용)
    [SerializeField] private TMP_Text descriptionText;  // 아이템 설명을 표시할 TextMeshPro
    [SerializeField] private TMP_Text statsText;        // 플레이어 스탯을 표시할 TextMeshPro (LV, HP 등)
    [SerializeField] private GameObject heartCursor;    // 선택 하트 GameObject

    [Header("--- 목록 동적 생성 ---")]
    [SerializeField] private GameObject itemSlotPrefab; // 1단계에서 만든 InventoryItemSlot이 붙은 프리팹
    [SerializeField] private Transform itemSlotParent;  // 아이템 슬롯들이 배치될 부모 Transform

    private bool isMenuOpen = false;
    private int currentSelectedIndex = 0; // 현재 선택된 아이템의 인덱스

    // 현재 인벤토리에 표시되고 있는 아이템 목록 (선택 처리를 위해 ItemData만 저장)
    private List<ItemData> displayedItems = new List<ItemData>();
    // 동적으로 생성된 슬롯 GameObject 목록
    private List<GameObject> activeSlots = new List<GameObject>();

    // GameManager와 InventoryManager는 미리 참조해 둡니다.
    private GameManager gameManager;
    private InventoryManager inventoryManager;

    private void Start()
    {
        // 참조 초기화
        gameManager = GameManager.Instance;
        inventoryManager = InventoryManager.Instance;

        inventoryPanel.SetActive(false); // 시작 시 인벤토리는 꺼져 있어야 해.

        // 스탯 업데이트 이벤트 구독 (스탯창과 공유)
        if (gameManager != null)
        {
            gameManager.playerStatsUpdate += UpdatePlayerStatsDisplay;
        }
    }

    private void OnDestroy()
    {
        // 구독 해지! 잊으면 안 돼, 오빠!
        if (gameManager != null)
        {
            gameManager.playerStatsUpdate -= UpdatePlayerStatsDisplay;
        }
    }

    private void Update()
    {
        // C 키를 누르면 인벤토리 메뉴를 켜거나 끕니다.
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleInventoryMenu();
        }

        // 메뉴가 열려 있을 때만 입력 처리
        if (isMenuOpen)
        {
            HandleMovementInput();
            HandleConfirmInput();
        }
    }

    private void ToggleInventoryMenu()
    {
        isMenuOpen = !isMenuOpen;
        inventoryPanel.SetActive(isMenuOpen);

        if (isMenuOpen)
        {
            // 메뉴를 열 때, 최신 아이템 목록과 스탯을 업데이트합니다.
            currentSelectedIndex = 0; // 인덱스 초기화
            PopulateItemList(); // 아이템 목록 동적 생성
            UpdatePlayerStatsDisplay(); // 스탯 업데이트
            UpdateSelectionDisplay(); // 선택 상태 UI 업데이트
        }
        else
        {
            ClearItemList(); // 메뉴를 닫을 때 메모리 관리를 위해 슬롯을 제거합니다.
        }
    }

    /// <summary>
    /// InventoryManager에서 데이터를 가져와 아이템 목록 UI를 동적으로 생성합니다.
    /// </summary>
    private void PopulateItemList()
    {
        ClearItemList(); // 기존 목록이 있다면 먼저 정리

        if (inventoryManager == null) return;

        // 소지한 아이템 딕셔너리를 가져옵니다.
        Dictionary<ItemData, int> items = inventoryManager.GetCurrentInventory();

        // 딕셔너리에서 ItemData만 추출하여 displayedItems 리스트에 저장합니다.
        displayedItems = items.Keys.ToList();

        // 아이템 목록이 없으면 종료합니다.
        if (displayedItems.Count == 0)
        {
            descriptionText.text = "아이템이 없습니다...";
            heartCursor.SetActive(false);
            return;
        }

        // 아이템 목록이 있다면 하트를 활성화합니다.
        heartCursor.SetActive(true);

        // 아이템 슬롯들을 동적으로 생성합니다.
        for (int i = 0; i < displayedItems.Count; i++)
        {
            ItemData data = displayedItems[i];
            int quantity = items[data];

            GameObject newSlot = Instantiate(itemSlotPrefab, itemSlotParent);

            // 슬롯의 위치를 조정해야 한다면 여기서 처리 (예: newSlot.transform.localPosition = new Vector3(0, -30 * i, 0);)

            if (newSlot.TryGetComponent(out InventoryItemSlot slotController))
            {
                slotController.SetSlotData(data, quantity);
            }

            activeSlots.Add(newSlot); // 생성된 슬롯 저장
        }
    }

    /// <summary>
    /// 동적으로 생성된 아이템 슬롯들을 파괴하고 목록을 초기화합니다.
    /// </summary>
    private void ClearItemList()
    {
        foreach (GameObject slot in activeSlots)
        {
            Destroy(slot); // 생성된 모든 슬롯 GameObject 파괴
        }
        activeSlots.Clear();
        displayedItems.Clear();
    }

    /// <summary>
    /// 플레이어의 스탯 정보를 TextMeshPro에 업데이트합니다.
    /// </summary>
    private void UpdatePlayerStatsDisplay()
    {
        if (gameManager == null || statsText == null) return;

        // GameManager에서 최신 스탯을 가져와 포맷팅합니다.
        string stats = $"LV {gameManager.PlayerLevel}\n" +
                       $"HP {gameManager.PlayerCurrentHP}/{gameManager.PlayerMaxHP}\n" +
                       $"ATK {gameManager.PlayerAttack}\n" +
                       $"DEF {gameManager.PlayerDefense}\n" +
                       $"GOLD {gameManager.PlayerCurrentGold}";

        statsText.text = stats;
    }

    /// <summary>
    /// 메뉴 이동(상/하 방향키)을 처리합니다.
    /// </summary>
    private void HandleMovementInput()
    {
        if (displayedItems.Count == 0) return;

        int direction = 0;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = -1;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = 1;
        }

        if (direction != 0)
        {
            // 인덱스를 순환하며 이동 (배틀 매니저의 GetNextIndex 로직과 유사)
            currentSelectedIndex += direction;
            if (currentSelectedIndex < 0)
            {
                currentSelectedIndex = displayedItems.Count - 1; // 맨 아래로
            }
            else if (currentSelectedIndex >= displayedItems.Count)
            {
                currentSelectedIndex = 0; // 맨 위로
            }

            UpdateSelectionDisplay();
        }
    }

    /// <summary>
    /// 현재 선택된 인덱스에 맞춰 하트 위치와 아이템 설명을 업데이트합니다.
    /// </summary>
    private void UpdateSelectionDisplay()
    {
        if (activeSlots.Count == 0) return;

        // 1. 하트 커서 위치 이동: 현재 선택된 슬롯의 위치로 하트 이동
        Transform selectedSlotTransform = activeSlots[currentSelectedIndex].transform;

        // 오빠, 하트 커서의 최종 위치는 선택된 슬롯의 위치를 기준으로 조금 왼쪽으로 조정해 줘야 해!
        // 이 위치는 오빠의 UI 디자인에 맞춰서 조정해야 해.
        heartCursor.transform.position = selectedSlotTransform.position + new Vector3(-100f, 0f, 0f);

        // 2. 아이템 설명 업데이트: 현재 선택된 아이템의 설명으로 텍스트 변경
        ItemData selectedItem = displayedItems[currentSelectedIndex];
        descriptionText.text = selectedItem.ItemDescription;
    }

    /// <summary>
    /// Z 키 (선택) 입력을 처리합니다. (아이템 사용 로직)
    /// </summary>
    private void HandleConfirmInput()
    {
        if (Input.GetKeyDown(KeyCode.Z) && displayedItems.Count > 0)
        {
            // 현재 선택된 아이템을 가져옵니다.
            ItemData selectedItem = displayedItems[currentSelectedIndex];

            // 여기에 "사용하시겠습니까?" 팝업을 띄우는 로직을 추가해야 해.
            // 지금은 바로 사용한다고 가정하고 처리할게!

            UseSelectedItem(selectedItem);
        }
    }

    /// <summary>
    /// 인벤토리에서 아이템을 사용하고 수량을 업데이트합니다.
    /// </summary>
    private void UseSelectedItem(ItemData item)
    {
        if (inventoryManager.RemoveItem(item)) // 인벤토리에서 아이템을 성공적으로 제거했다면
        {
            item.UseItemEffect(); // ItemData 스크립트에 정의된 효과를 실행 (HP 회복 등)

            // 사용 후 인벤토리 목록을 갱신합니다.
            PopulateItemList();

            // 선택 인덱스가 0보다 크다면, 목록이 갱신된 후 인덱스를 조정합니다.
            if (currentSelectedIndex >= displayedItems.Count && displayedItems.Count > 0)
            {
                currentSelectedIndex = displayedItems.Count - 1;
            }

            UpdateSelectionDisplay(); // 선택 상태 UI 업데이트
        }
        else
        {
            // 사용 실패 (이럴 일은 없겠지만, 디버깅용)
            Debug.LogWarning("[Inventory] 아이템 사용에 실패했습니다.");
        }
    }
}
