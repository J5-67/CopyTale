using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private Dictionary<ItemData, int> inventoryItems = new Dictionary<ItemData, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 테스트용 아이템 추가 (아이템 데이터 에셋 파일을 Project 폴더에 만들어야 작동해요!)
        ItemData testPotion = Resources.Load<ItemData>("ItemData/HP_Potion"); // Resource 폴더에 있다면

        if (InventoryManager.Instance != null && testPotion != null)
        {
            InventoryManager.Instance.AddItem(testPotion, 3); // 포션 3개 추가
        }
    }

    public void AddItem(ItemData item, int quantity = 1)
    {
        if (item == null) return;

        if (inventoryItems.ContainsKey(item))
        {
            inventoryItems[item] += quantity;
        }
        else
        {
            inventoryItems.Add(item, quantity);
        }

        Debug.Log($"[Inventory] {item.ItemName} {quantity}개 추가됨. 현재 수량: {inventoryItems[item]}");
    }

    public bool RemoveItem(ItemData item, int quantity = 1)
    {
        if (item == null || !inventoryItems.ContainsKey(item) || inventoryItems[item] < quantity)
        {
            return false;
        }

        inventoryItems[item] -= quantity;

        if (inventoryItems[item] <= 0)
        {
            inventoryItems.Remove(item);
        }

        Debug.Log($"[Inventory] {item.ItemName} {quantity}개 제거됨.");
        return true;
    }

    public Dictionary<ItemData, int> GetCurrentInventory()
    {
        // 딕셔너리를 복사해서 반환하여 외부에서 원본을 임의로 변경하지 못하게 보호
        return new Dictionary<ItemData, int>(inventoryItems);
    }
}