using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleTrigger : MonoBehaviour
{
    [SerializeField] private GameObject Player;
    public EnemyData targetEnemyData;
    private bool isInTrigger = false;

    private void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.Z))
        {
            if (targetEnemyData == null)
            {
                Debug.LogError("targetEnemyData가 Null입니다! 적의 충돌 영역에 들어갔을 때 데이터 할당이 안 된 것 같아요.");
                return; // Null이면 여기서 멈춥니다.
            }
            Debug.Log("전투 시작" + targetEnemyData.EnemyName);
            StartBattle();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyDataHolder dataHolder = collision.GetComponent<EnemyDataHolder>();

        isInTrigger = true;

        targetEnemyData = dataHolder.enemyDataAsset;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInTrigger = false;

        targetEnemyData = null;
    }

    private void StartBattle()
    {
        GameManager.Instance.enemyData = targetEnemyData;

        SceneManager.LoadScene("Battle");
    }
}
