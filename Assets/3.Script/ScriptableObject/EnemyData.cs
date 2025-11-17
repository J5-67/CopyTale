using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct EnemyPattern
{
    public string PatternID;            // 고유 식별자 (예: "BasicShot", "SpinningWave")
    public string PatternDialogue;      // 이 패턴 시작 시 표시할 대사
    public float Duration;

    [Header("--- 탄막 데이터 ---")]
    public GameObject BulletPrefab;
    public int BulletCount;
    public float FireRate; 
    public float Speed;
    public float AngleOffset;
}

[System.Serializable]
public struct Spare
{
    public SpareConditionType type;
    public int index;
    public int needCount;
}
public enum SpareConditionType
{
    None,
    Attack,
    Act,
    Mercy,
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("--- 기본 정보 ---")]

    [SerializeField]
    private string enemyName = "플라위";

    [SerializeField]
    private int maxHP = 100;
    private int currentHP;

    [SerializeField]
    private int realEnemyAttack = 5;

    [SerializeField]
    private int realEnemyDefense = 2;

    [SerializeField]
    private int fakeEnemyAttack = 5; 

    [SerializeField]
    private int fakeEnemyDefense = 2; 

    [SerializeField]
    private int expDrop = 100;

    [SerializeField]
    private int goldDrop = 100;

    [Header("--- 자비 조건 ---")]
    [SerializeField]
    private Spare spare;

    private bool isSpare = false;

    private int escapeCount = 0;

    [Header("--- 대사 정보 ---")]

    [SerializeField]
    [TextArea(2, 5)]
    private List<string> normalDialogues = new List<string> { "플라위가 심심해 보인다." };

    [SerializeField]
    [TextArea(2, 5)]
    private List<string> spareDialogues = new List<string> { "플라위: 고마워! 잘 가." };

    [SerializeField]
    [TextArea(2, 5)]
    private string encounterDialogue = "반가워! 내 이름은 플라위.";
                                                                                   
    [SerializeField]
    [TextArea(2, 5)]
    private List<string> reacts = new List<string> { "살펴보기" };

    [SerializeField]
    [TextArea(2, 5)]
    private List<string> reactsDialogues = new List<string> { "작은 꽃 플라위다" };

    [SerializeField]
    [TextArea(2, 5)]
    private List<string> battleDialogues = new List<string> { "내 친절 알갱이를 받아!" };

    [Header("--- 패턴 정보 ---")]

    [SerializeField]
    private List<EnemyPattern> enemyPatterns = new List<EnemyPattern>();

    public int RealEnemyAttack => realEnemyAttack;
    public int RealEnemyDefense => realEnemyDefense;
    public int FakeEnemyAttack => fakeEnemyAttack;
    public int FakeEnemyDefense => fakeEnemyDefense;
    public string EnemyName => enemyName;
    public int MaxHP => maxHP;
    public int CurrentHP
    {
        get => currentHP;
        set => currentHP = value;
    }
    public void CalEnemyHP()
    {
        CurrentHP = maxHP;
    }
    public bool IsSpare => isSpare;
    public int EscapeCount => escapeCount;
    public int ExpDrop => expDrop;
    public int GoldDrop => goldDrop;
    public string EncounterDialogue => encounterDialogue;
    public List<string> NormalDialogues => normalDialogues;
    public List<string> SpareDialogues => spareDialogues;
    public List<string> BattleDialogues => battleDialogues;
    public List<string> Reacts => reacts;
    public List<string> ReactsDialogues => reactsDialogues;
    public Spare Spare => spare;
    public List<EnemyPattern> EnemyPatterns => enemyPatterns;

    public void SetSpare(bool isSpare)
    {
        this.isSpare = isSpare;
    }
    public void PlusEscapeCount()
    {
        escapeCount++;
    }
    public void ResetBattle()
    {
        isSpare = false;
        escapeCount = 0;
    }
    public int TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage - realEnemyDefense);

        CurrentHP -= finalDamage;

        CurrentHP = Mathf.Max(0, CurrentHP);

        return finalDamage;
    }

    private void OnEnable()
    {
        isSpare = false;
        escapeCount = 0;
        currentHP = maxHP;
    }
}
