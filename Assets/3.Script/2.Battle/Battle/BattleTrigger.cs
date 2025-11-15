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
            Debug.Log("전투 시작" + targetEnemyData.EnemyName);
            StartBattle();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyDataHolder dataHolder = collision.GetComponent<EnemyDataHolder>();

        isInTrigger = true;

        targetEnemyData = dataHolder.enemyDataAsset;

        //if (collision.CompareTag("Flowey"))
        //{
        //    Debug.Log("플라위 접촉");
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInTrigger = false;

        targetEnemyData = null;

        //if (collision.CompareTag("Flowey"))
        //{
        //    Debug.Log("플라위 접촉 해제");
        //}
    }

    private void StartBattle()
    {
        GameManager.Instance.enemyData = targetEnemyData;

        SceneManager.LoadScene("Battle");
    }
}
