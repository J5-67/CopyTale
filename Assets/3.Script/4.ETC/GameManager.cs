using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;

    [Header("플레이어 기본 스탯")]
    public string PlayerName = "FRISK"; //이름 입력 추가 고민,,,
    public int PlayerLevel = 1;
    public float PlayerMaxHP = 20f;
    public float PlayerCurrentHP = 20f;
    public float PlayerAttack = 10f;
    public float PlayerDefense = 0f;
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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float damage)
    {
        PlayerCurrentHP -= damage;
        if (PlayerCurrentHP < 0)
        {
            PlayerCurrentHP = 0;
            // TODO: 게임 오버 로직 호출
        }
    }
}
