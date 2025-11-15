using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct EnemyPattern
{
    public string PatternID;            // 고유 식별자 (예: "BasicShot", "SpinningWave")
    public string PatternDialogue;      // 이 패턴 시작 시 표시할 대사
    public float Duration;           // 패턴이 지속될 총 시간 (초)

    [Header("--- 탄막 데이터 ---")]
    public GameObject BulletPrefab;     // 사용할 탄막 프리팹
    public int BulletCount;             // 한 번에 발사할 탄막의 개수 (샷 건, 원형 발사 등)
    public float FireRate;              // 탄막 생성 주기 (초)
    public float Speed;                 // 탄막의 속도
    public float AngleOffset;           // 탄막 발사 각도 조정
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("--- 기본 정보 ---")]

    [SerializeField]
    private string enemyName = "플라위"; // 적의 이름

    [SerializeField]
    private int maxHealth = 100; // 적의 최대 체력

    [SerializeField]
    private int realEnemyAttack = 5; // 적의 실제 공격력 

    [SerializeField]
    private int realEnemyDefense = 2; // 적의 실제 방어력

    [SerializeField]
    private int fakeEnemyAttack = 5; // 적의 보이는 공격력 

    [SerializeField]
    private int fakeEnemyDefense = 2; // 적의 보이는 방어력

    [SerializeField]
    private int experienceDrop = 100; // 처치 시 획득 경험치

    [SerializeField]
    private int goldDrop = 100; // 처치 시 획득 골드

    [SerializeField]
    public List<GameObject> DefaultProjectilePrefab = new List<GameObject>();

    [SerializeField]
    [TextArea(2, 5)]
    private string encounterDialogue = "반가워! 내 이름은 플라위."; // 조우 시 대사
                                                                                   
    [SerializeField]
    [TextArea(2, 5)] // 인스펙터에서 여러 줄의 긴 텍스트 입력을 쉽게 할 수 있도록 합니다.
    private List<string> reacts = new List<string> { "살펴보기" }; // 대응 목록

    [SerializeField]
    [TextArea(2, 5)] // 인스펙터에서 여러 줄의 긴 텍스트 입력을 쉽게 할 수 있도록 합니다.
    private List<string> reactsDialogues = new List<string> { "작은 꽃 플라위다" }; // 대응 행동 대사

    [SerializeField]
    [TextArea(2, 5)] // 인스펙터에서 여러 줄의 긴 텍스트 입력을 쉽게 할 수 있도록 합니다.
    private List<string> battleDialogues = new List<string> { "내 친절 알갱이를 받아!" }; // 전투 중 표시되는 대사 목록

    [SerializeField]
    private List<EnemyPattern> enemyPatterns = new List<EnemyPattern>(); // 적의 패턴 목록 (EnemyPattern 구조체 사용)

    public int RealEnemyAttack => realEnemyAttack;
    public int RealEnemyDefense => realEnemyDefense;
    public int FakeEnemyAttack => fakeEnemyAttack;
    public int FakeEnemyDefense => fakeEnemyDefense;
    public string EnemyName => enemyName;
    public int EnemyMaxHealth => maxHealth;
    public int ExperienceDrop => experienceDrop;
    public int GoldDrop => goldDrop;
    public string EncounterDialogue => encounterDialogue;
    public List<string> BattleDialogues => battleDialogues;
    public List<string> Reacts => reacts;
    public List<string> ReactsDialogues => reactsDialogues;
    public List<EnemyPattern> EnemyPatterns => enemyPatterns;
}
