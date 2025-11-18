using UnityEngine;
using TMPro;

public class InventoryItemSlot : MonoBehaviour
{
    [SerializeField]
    private TMP_Text itemNameText; // 아이템 이름을 표시할 TextMeshPro

    [SerializeField]
    private TMP_Text itemQuantityText; // 아이템 수량을 표시할 TextMeshPro

    private ItemData currentItemData; // 이 슬롯에 할당된 ItemData 참조

    /// <summary>
    /// 슬롯의 정보를 설정하고 UI를 업데이트합니다.
    /// </summary>
    public void SetSlotData(ItemData data, int quantity)
    {
        currentItemData = data;

        // 아이템의 이름과 수량을 설정합니다.
        if (itemNameText != null)
        {
            itemNameText.text = data.ItemName;
        }
        if (itemQuantityText != null)
        {
            // 수량이 99를 넘어가면 '99+' 등으로 표시할 수 있도록 확장 가능
            itemQuantityText.text = $"x{quantity}";
        }
    }

    /// <summary>
    /// 현재 슬롯에 연결된 ItemData를 반환합니다.
    /// </summary>
    public ItemData GetItemData()
    {
        return currentItemData;
    }
}