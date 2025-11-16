using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    private Dictionary<int, int> spareCount = new Dictionary<int, int>();
    private List<GameObject> activeMenuTexts = new List<GameObject>();
    private List<GameObject> activeEnemies = new List<GameObject>();
    private PatternManager patternManager;
    private EnemyData currentEnemyData;
    private int currentSpareAction = 0;
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

            currentEnemyData.CalEnemyHP();
        }

        if (DialogueText != null)
        {
            DialogueText.text = currentEnemyData.EncounterDialogue;
        }

        if (MainButtonPosition.Length > 0)
        {
            MoveActHeart(MainButtonPosition, buttonChoice.CurrentMainButtonIndex);
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
            case ITEM_INDEX:
                DisplayItemSubMenu();
                break;
            case MERCY_INDEX:
                DisplayMercySubMenu();
                break;
        }
    }

    #region 서브 메뉴
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

    private void DisplayItemSubMenu()
    {

    }

    private void DisplayMercySubMenu()
    {
        ClearSubButtons();
        GameObject spareButton = Instantiate(MenuTextPrefab, MenuTextParent);
        activeMenuTexts.Add(spareButton);
        TMP_Text spareText = spareButton.GetComponentInChildren<TMP_Text>();
        if (spareText != null)
        {
            spareText.text = "살려주기";
            if (currentEnemyData.IsSpare)
            {
                spareText.color = Color.yellow;
            }
            else
            {
                spareText.color = Color.white;
            }
        }

        GameObject escapeButton = Instantiate(MenuTextPrefab, MenuTextParent);
        escapeButton.SetActive(true);
        activeMenuTexts.Add(escapeButton);
        TMP_Text escapeText = escapeButton.GetComponentInChildren<TMP_Text>();
        if (escapeText != null)
        {
            escapeText.text = "도망치기";
        }

        ActiveSubButtonCount = activeMenuTexts.Count;
    }

    #endregion

    public void StartSubAction(int mainIndex, int subIndex)
    {
        switch (mainIndex)
        {
            case FIGHT_INDEX:
                if (subIndex >= 0 && subIndex < activeEnemies.Count)
                {
                    Attack();
                }
                break;
            case ACT_INDEX:
                Act(subIndex);
                break;
            case ITEM_INDEX:
                break;
            case MERCY_INDEX:
                Mercy(subIndex);
                break;
        }
    }

    private void Attack()
    {
        AttackBg.SetActive(true);
        AttackBar.SetActive(true);
        Act_Heart.SetActive(false);
        buttonChoice.enabled = false;

        ClearSubButtons();

        battleDamageCal.enabled = true;
    }

    public void AttackDamage(float damage)
    {
        int intDamage = Mathf.RoundToInt(damage);

        currentEnemyData.TakeDamage(intDamage);

        Debug.Log($"적에게 {damage}의 피해를 입혔습니다. 적의 체력 : {currentEnemyData.CurrentHP}");

        if(currentEnemyData.Spare.type == SpareConditionType.Attack)
        {
            currentSpareAction++;
            CheckSpare();
        }
    }

    private void Act(int index)
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

            if(currentEnemyData.Spare.type == SpareConditionType.Act && currentEnemyData.Spare.index == index)
            {
                currentSpareAction++;
                CheckSpare();
            }

            StartEnemyTurn();
        }
    }

    private void Item()
    {

    }

    private void Mercy(int index)
    {
        if (index == 0)
        {
            if (currentEnemyData.IsSpare)
            {
                BattleEnd(false);
            }
            else
            {
                if(currentEnemyData.Spare.type == SpareConditionType.Mercy)
                {
                    currentSpareAction++;
                    CheckSpare();
                }
                StartEnemyTurn();
            }
        }
        else if (index == 1)
        {
            float escapeChance = 50f + (currentEnemyData.EscapeCount * 10f);
            float rnd = UnityEngine.Random.Range(0f, 100f);

            if(rnd <= escapeChance)
            {
                BattleEnd(true);
            }
            else
            {
                currentEnemyData.PlusEscapeCount();
                StartEnemyTurn();
            }
        }

        ClearSubButtons();
        if(Act_Heart.activeSelf == true)
        {
            Act_Heart.SetActive(false);
        }
    }

    public void StartEnemyTurn()
    {
        ClearSubButtons();

        if (buttonChoice.enabled == true)
        {
            buttonChoice.enabled = false;
        }

        if (battleDamageCal.enabled == true)
        {
            battleDamageCal.enabled = false;
        }

        if (Act_Heart.activeSelf == true)
        {
            Act_Heart.SetActive(false);
        }

        if (AttackBg.activeSelf && AttackBar.activeSelf)
        {
            AttackBg.SetActive(false);
            AttackBar.SetActive(false);
        }

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
            MoveActHeart(MainButtonPosition, buttonChoice.CurrentMainButtonIndex);
        }
    }

    private void CheckSpare()
    {
        if(currentEnemyData == null || currentEnemyData.IsSpare)
        {
            return;
        }

        Spare spare = currentEnemyData.Spare;

        if(spare.needCount <= 0)
        {
            return;
        }

        if (currentSpareAction >= spare.needCount)
        {
            currentEnemyData.SetSpare(true);
        }
    }

    public void BattleEnd(bool rebattle)
    {
        if(patternManager != null)
        {
            patternManager.StopMonsterAttack();
        }

        // 2. 필요한 데이터 저장 (재전투 가능 여부 등)

        SceneManager.LoadScene("MainGame");

        EndTurn();
        this.enabled = false;
    }

    public void MoveActHeart(Transform[] positions, int index)
    {
        if (Act_Heart != null && index >= 0 && index < positions.Length)
        {
            Act_Heart.transform.position = positions[index].position;
        }
    }

    public void CloseSubMenu()
    {
        ClearSubButtons();
    }
}