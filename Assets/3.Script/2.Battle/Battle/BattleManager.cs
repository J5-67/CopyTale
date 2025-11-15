using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("플레이어 하트")]
    [SerializeField] private GameObject Fight_Heart;
    [SerializeField] private GameObject Act_Heart;
    [SerializeField] private ButtonChoice buttonChoice;

    [Header("버튼 위치")]
    [SerializeField] private Transform[] MainButtonPosition;
    [SerializeField] private Transform[] SubButtonPosition;

    [Header("공격 기능")]
    [SerializeField] private GameObject AttackBg;
    [SerializeField] private GameObject AttackBar;
    [SerializeField] private BattleDamageCal battleDamageCal;

    [Header("UI 관련")]
    [SerializeField] private GameObject[] BattleTextBorder;
    [SerializeField] private GameObject[] BattleBorder;
    [SerializeField] private TMP_Text EnemyNameText;
    [SerializeField] private TMP_Text DialogueText;

    [Header("동적 생성 메뉴 항목")]
    [SerializeField] private GameObject MenuTextPrefab;
    [SerializeField] private Transform MenuTextParent;

    [Header("적 데이터")]
    [SerializeField] private EnemyController enemyController;

    private List<GameObject> activeMenuTexts = new List<GameObject>();
    private List<GameObject> activeEnemies = new List<GameObject>();
    private EnemyData currentEnemyData;
    private PatternManager patternManager;
    private const int FIGHT_INDEX = 0;
    private const int ACT_INDEX = 1;
    private const int ITEM_INDEX = 2;
    private const int MERCY_INDEX = 3;
    public int ActiveSubButtonCount { get; private set; } = 0;

    void Start()
    {
        TryGetComponent(out patternManager);

        if (GameManager.Instance.enemyData != null)
        {
            currentEnemyData = GameManager.Instance.enemyData;
        }

        if (DialogueText != null)
        {
            DialogueText.text = currentEnemyData.EncounterDialogue;
        }

        if (MainButtonPosition.Length > 0)
        {
            MoveHeart(MainButtonPosition, buttonChoice.CurrentMainButtonIndex);
        }

        ClearSubButtons();

        //일단 하나만
        GameObject mainEnemy = GameObject.FindGameObjectWithTag("Flowey");

        activeEnemies.Add(mainEnemy);
    }

    private void ClearSubButtons()
    {
        foreach (GameObject textObj in activeMenuTexts)
        {
            if (textObj != null)
            {
                Destroy(textObj);
            }
        }
        activeMenuTexts.Clear();
        ActiveSubButtonCount = 0;
    }

    public void OpenSubMenu(int mainIndex)
    {
        if (EnemyNameText != null)
        {
            EnemyNameText.gameObject.SetActive(false);
        }

        switch (mainIndex)
        {
            case FIGHT_INDEX:
                DisplayFightSubMenu();
                break;

            case ACT_INDEX:
                DisplayActSubMenu();
                break;

                // TODO: ITEM_INDEX, MERCY_INDEX 서브 메뉴 추가
        }
    }

    private void DisplayFightSubMenu()
    {
        ClearSubButtons();

        int enemyCount = activeEnemies.Count;

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyName = Instantiate(MenuTextPrefab, MenuTextParent);

            enemyName.SetActive(true);

            activeMenuTexts.Add(enemyName);

            TMP_Text subMenuText = enemyName.GetComponentInChildren<TMP_Text>();

            if (subMenuText != null)
            {
                subMenuText.text = currentEnemyData.EnemyName;
            }
        }

        ActiveSubButtonCount = activeMenuTexts.Count;
    }

    private void DisplayActSubMenu()
    {
        if (currentEnemyData == null || currentEnemyData.Reacts == null)
        {
            return;
        }

        ClearSubButtons();

        int reactsCount = currentEnemyData.Reacts.Count;

        for (int i = 0; i < reactsCount; i++)
        {
            GameObject enemyReact = Instantiate(MenuTextPrefab, MenuTextParent);

            enemyReact.SetActive(true);

            activeMenuTexts.Add(enemyReact);

            TMP_Text subMenuText = enemyReact.GetComponentInChildren<TMP_Text>();

            if (subMenuText != null)
            {
                subMenuText.text = currentEnemyData.Reacts[i];
            }
        }

        ActiveSubButtonCount = activeMenuTexts.Count;
    }


    public void MoveHeart(Transform[] positions, int index)
    {
        if (Act_Heart != null && index >= 0 && index < positions.Length)
        {
            Act_Heart.transform.position = positions[index].position;
        }
    }

    public void StartSubAction(int mainIndex, int subIndex)
    {
        switch (mainIndex)
        {
            case FIGHT_INDEX:
                if (subIndex >= 0 && subIndex < activeEnemies.Count)
                {
                    AttackBg.SetActive(true);
                    AttackBar.SetActive(true);
                    Act_Heart.SetActive(false);
                    buttonChoice.enabled = false;

                    ClearSubButtons();

                    battleDamageCal.enabled = true;
                }
                break;

            case ACT_INDEX:
                HandleActSubAction(subIndex);
                break;

                // TODO: ITEM_INDEX, MERCY_INDEX StartSubAction 로직 추가
        }
    }

    private void HandleActSubAction(int index)
    {
        if (currentEnemyData == null) return;

        if (index >= 0 && index < currentEnemyData.Reacts.Count)
        {
            string actName = currentEnemyData.Reacts[index];
            string dialogue = currentEnemyData.ReactsDialogues[index];

            if (EnemyNameText != null)
            {
                EnemyNameText.gameObject.SetActive(true);
                EnemyNameText.text = $"{currentEnemyData.EnemyName} - {actName}";
            }

            if (DialogueText != null)
            {
                DialogueText.text = dialogue;
            }

            ClearSubButtons();
            Act_Heart.SetActive(false);

            // TODO: 행동 실행 후 턴 종료 (EndTurn) 로직 호출
        }
    }

    // TODO: HandleItemSubAction, HandleMercySubAction 추가

    public void HandleEndAttackTurn()
    {
        AttackBg.SetActive(false);
        AttackBar.SetActive(false);

        foreach (GameObject TextBorder in BattleTextBorder)
        {
            TextBorder.SetActive(false);
        }

        foreach (GameObject Border in BattleBorder)
        {
            Border.SetActive(true);
        }

        Fight_Heart.transform.position = new Vector3(0f, -1.5f, 0f);
        Fight_Heart.SetActive(true);

        float rndPos = UnityEngine.Random.Range(-2f, 2);

        patternManager.StartPattern(currentEnemyData, new Vector3(rndPos, 1.3f, 0));
    }

    public void EndEnemyTurn()
    {
        if (patternManager != null)
        {
            patternManager.StopMonsterAttack();
        }

        EndTurn();
    }

    public void EndTurn()
    {
        buttonChoice.enabled = true;

        buttonChoice.ResetChoice();

        Fight_Heart.SetActive(false);

        Act_Heart.SetActive(true);

        foreach (GameObject Border in BattleBorder)
        {
            Border.SetActive(false);
        }

        foreach (GameObject TextBorder in BattleTextBorder)
        {
            TextBorder.SetActive(true);
        }

        if (MainButtonPosition.Length > 0)
        {
            MoveHeart(MainButtonPosition, buttonChoice.CurrentMainButtonIndex);
        }
    }

    public void CloseSubMenu()
    {
        ClearSubButtons();

        // TODO: 서브 메뉴 패널 배경 등을 닫는 로직도 여기에 추가하면 돼!
    }
}