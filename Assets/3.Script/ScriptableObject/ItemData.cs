using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("--- 기본 정보 ---")]
    [SerializeField]
    private string itemName;
    public string ItemName => itemName;

    [SerializeField]
    [TextArea(3, 5)]
    private string itemDescription;
    public string ItemDescription => itemDescription;

    [Header("--- 가격 정보 ---")]
    [SerializeField]
    private int buyPrice;
    public int BuyPrice => buyPrice;

    [SerializeField]
    private int sellPrice;
    public int SellPrice => sellPrice;

    [Header("--- 효과 정보 ---")]
    [SerializeField]
    private ItemType type;
    public ItemType Type => type;

    [SerializeField]
    private int effectValue;
    public int EffectValue => effectValue;

    public enum ItemType
    {
        None,
        Heal_HP,
        Heal_MAXHP,
        Attack_UP,
        Defense_UP
    }

    public void UseItemEffect()
    {
        if (GameManager.Instance == null) return;

        switch (type)
        {
            case ItemType.Heal_HP:

                GameManager.Instance.PlayerCurrentHP += effectValue;

                if (GameManager.Instance.PlayerCurrentHP > GameManager.Instance.PlayerMaxHP)
                {
                    GameManager.Instance.PlayerCurrentHP = GameManager.Instance.PlayerMaxHP;
                }
                break;
            case ItemType.Attack_UP:

                GameManager.Instance.PlayerAttack += effectValue;
                break;
            case ItemType.Defense_UP:

                GameManager.Instance.PlayerDefense += effectValue;
                break;

        }


        //GameManager.Instance.playerStatsUpdate?.Invoke();
    }
}