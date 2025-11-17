using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleResult
{
    Spare,
    Kill
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [Header("적 데이터 중앙 창고")]
    [SerializeField]
    private List<EnemyData> allEnemyData = new List<EnemyData>();

    private Dictionary<string, EnemyData> enemyDataMap = new Dictionary<string, EnemyData>();
    private Dictionary<string, BattleResult> EnemyStatus = new Dictionary<string, BattleResult>();
    public EnemyData enemyDataForNextBattle { get; private set; }

    [Header("플레이어 기본 스탯")]
    public string PlayerName = "FRISK"; //이름 입력 추가 고민,,,
    public int PlayerLevel = 1;
    public int PlayerMaxHP = 20;
    public int PlayerCurrentHP = 20;
    public int PlayerCurrentGold = 0;
    public int PlayerNeedEXP = 50;
    public int PlayerCurrentEXP = 0;
    public int PlayerAttack = 10;
    public int PlayerDefense = 5;
    public int flag = 0;
    //public new Vector3(?,?,?); 만약 저장을 한다면,,,
    //public 아이템?

    public EnemyData enemyData;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeEnemyDataMap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeEnemyDataMap()
    {
        enemyDataMap.Clear();
        foreach (EnemyData data in allEnemyData)
        {
            if (!enemyDataMap.ContainsKey(data.EnemyName))
            {
                enemyDataMap.Add(data.EnemyName, data);
            }
        }
    }
    public EnemyData GetEnemyData(string enemyNameID)
    {
        if (enemyDataMap.TryGetValue(enemyNameID, out EnemyData data))
        {
            return data;
        }
        Debug.LogError($"[GameManager] 요청된 적 이름({enemyNameID})에 해당하는 EnemyData를 찾을 수 없습니다.");
        return null;
    }

    public void SetEnemyDataForBattle(EnemyData data)
    {
        enemyDataForNextBattle = data;
    }

    public void RecordBattleEnd(string enemyName, BattleOutcome outcome)
    {
        if(outcome == BattleOutcome.Kill)
        {
            EnemyStatus[enemyName] = BattleResult.Kill;
        }
        else if(outcome == BattleOutcome.Spare)
        {
            EnemyStatus[enemyName] = BattleResult.Spare;
        }
    }

    public bool IsEnemyLive(string enemyName)
    {
        if(!EnemyStatus.TryGetValue(enemyName, out var result))
        {
            return true;
        }

        return result == BattleResult.Spare;
    }

    public bool SpecialLine(string enemyName)
    {
        if (EnemyStatus.TryGetValue(enemyName, out var result))
        {
            return result == BattleResult.Spare;
        }
        return false;
    }

    public bool CantReBattle(string enemyName)
    {
        return EnemyStatus.ContainsKey(enemyName);
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage - PlayerDefense);

        PlayerCurrentHP -= finalDamage;

        if (PlayerCurrentHP < 0)
        {
            PlayerCurrentHP = 0;
            // TODO: 게임 오버 로직 호출
        }
    }

    public void AddGold(int amount)
    {
        PlayerCurrentGold += amount;
    }

    public void AddEXP(int amount)
    {
        PlayerCurrentEXP += amount;

        while(PlayerCurrentEXP >= PlayerNeedEXP)
        {
            PlayerCurrentEXP -= PlayerNeedEXP;
            PlayerLevel++;
            PlayerDefense += 5;
            PlayerNeedEXP += 50;
            PlayerAttack *= 2;
            PlayerMaxHP *= 2;
            PlayerCurrentHP = PlayerMaxHP;
        }
    }
}
