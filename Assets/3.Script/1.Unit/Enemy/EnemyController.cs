using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    
    [Header("적 이름")]
    [SerializeField]
    private string enemyName;

    private bool isInTrigger = false;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        bool shouldBeVisible = GameManager.Instance.IsEnemyLive(enemyName);
        gameObject.SetActive(shouldBeVisible);
    }

    private void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        EnemyData enemyData = GameManager.Instance.GetEnemyData(enemyName);

        if (enemyData == null) 
        {
            Debug.LogError($"[ERROR] ID '{enemyName}'에 해당하는 EnemyData를 찾지 못했습니다. GameManager의 All Enemy Data 설정을 확인하세요.");
            return; 
        }

        if (GameManager.Instance.CantReBattle(enemyName))
        {
            if (GameManager.Instance.IsEnemyLive(enemyName))
            {
                if (enemyData.SpareDialogues.Count > 0)
                {
                    Debug.Log($"[상호작용] 특별 대화: {enemyData.SpareDialogues[0]}");
                }
            }
        }
        else
        {
            if (enemyData.NormalDialogues.Count > 0)
            {
                // StartDialogue(enemyData.NormalInteractionDialogues); 
                Debug.Log($"[상호작용] 일반 대화: {enemyData.NormalDialogues[0]}");
            }
            Debug.Log($"[SUCCESS] {enemyData.EnemyName} 데이터를 GameManager에 설정하고 배틀 씬 로드.");
            GameManager.Instance.SetEnemyDataForBattle(enemyData);
            SceneManager.LoadScene("Battle");
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInTrigger = false;
        }
    }
}
