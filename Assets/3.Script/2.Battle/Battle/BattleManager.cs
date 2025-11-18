using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public enum BattleOutcome
{
    Spare,
    Kill,
    Escape
}

public class BattleManager : MonoBehaviour
{
    [Header("플레이어 하트")]
    [SerializeField] private GameObject Fight_Heart;
    [SerializeField] private GameObject Act_Heart;
    [SerializeField] private GameObject Escape_Heart;
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
    private int selectedEnemyIndex = -1;
    public int ActiveSubButtonCount { get; private set; } = 0;


    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("error: no GameManager");
            SceneManager.LoadScene("MainGame");
            return;
        }

        TryGetComponent(out patternManager);

        if (GameManager.Instance.enemyDataForNextBattle != null)
        {
            currentEnemyData = GameManager.Instance.enemyDataForNextBattle;
            currentEnemyData.CalEnemyHP();
        }
        else
        {
            Debug.LogError("error no enemydata in gamemanager");
            SceneManager.LoadScene("MainGame");
            return;
        }

        GameManager.Instance.SetEnemyDataForBattle(null);

        if(Fight_Heart != null && Fight_Heart.TryGetComponent(out HeartController hc))
        {
            hc.SetBattleManager(this);
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

        if(mainEnemy != null)
        {
            activeEnemies.Add(mainEnemy);
        }
    }


    #region Sub Menu
    public void OpenSubMenu(int mainIndex)
    {
        if (EnemyNameText != null)
        {
            EnemyNameText.gameObject.SetActive(false);
        }

        selectedEnemyIndex = -1;

        switch (mainIndex)
        {
            case FIGHT_INDEX:
            case ACT_INDEX:
            case MERCY_INDEX:
                DisplayEnemySelect();
                break;
            case ITEM_INDEX:
                DisplayItemSubMenu();
                break;
        }
    }

    private void DisplayEnemySelect()
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

                if (currentEnemyData.IsSpare)
                {
                    subMenuText.color = Color.yellow;
                }
                else
                {
                    subMenuText.color = Color.white;
                }
            }
        }

        ActiveSubButtonCount = activeMenuTexts.Count;
    }

    //private void DisplayFightSubMenu()
    //{
    //    
    //}

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
        spareButton.SetActive(true);
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

    public void StartSubAction(int mainIndex, int subIndex)
    {
        if(selectedEnemyIndex == -1)
        {
            selectedEnemyIndex = subIndex;

            if(mainIndex == FIGHT_INDEX)
            {
                Attack();
                selectedEnemyIndex = -1;
                return;
            }
            else if(mainIndex == ACT_INDEX)
            {
                DisplayActSubMenu();
                buttonChoice.ResetSubChoice();
                return;
            }else if(mainIndex == MERCY_INDEX)
            {
                DisplayMercySubMenu();
                buttonChoice.ResetSubChoice();
                return;
            }
        }

        if(mainIndex == ACT_INDEX)
        {
            Act(subIndex);
        }
        else if(mainIndex == MERCY_INDEX)
        {
            Mercy(subIndex);
        }

        selectedEnemyIndex = -1;
    }

    #endregion


    #region Attack
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


        if (currentEnemyData.CurrentHP <= 0)
        {
            BattleEnd(BattleOutcome.Kill);
            return;
        }

        if (currentEnemyData.Spare.type == SpareConditionType.Attack)
        {
            currentSpareAction++;
            CheckSpare();
        }
    }
    #endregion


    #region Act
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
    #endregion


    #region Item
    private void Item()
    {

    }
    #endregion


    #region Mercy
    private void Mercy(int index)
    {
        if (index == 0)
        {
            if (currentEnemyData.IsSpare)
            {
                BattleEnd(BattleOutcome.Spare);
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
                Escape_Heart.transform.position = Act_Heart.transform.position;
                Act_Heart.SetActive(false);
                StartCoroutine(Escape_co());
                
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

    private IEnumerator Escape_co()
    {
        float escapeDuration = 2f;
        float t = 0f;

        while(t < escapeDuration)
        {
            t += Time.deltaTime;

            Vector3 newPos = Escape_Heart.transform.position;

            newPos.x += 3f * Time.deltaTime;

            Escape_Heart.transform.position = newPos;

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        BattleEnd(BattleOutcome.Escape);
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
    #endregion


    #region Turn
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

    public void BattleEnd(BattleOutcome outcome)
    {
        if(patternManager != null)
        {
            patternManager.StopMonsterAttack();
        }

        if(GameManager.Instance != null && currentEnemyData != null)
        {
            if (outcome == BattleOutcome.Kill || outcome == BattleOutcome.Spare)
            {
                if (GameManager.Instance != null && currentEnemyData != null)
                {
                    GameManager.Instance.RecordBattleEnd(currentEnemyData.EnemyName, outcome);
                }
            }

            if(outcome == BattleOutcome.Kill)
            {
                GameManager.Instance.AddEXP(currentEnemyData.ExpDrop);
                GameManager.Instance.AddGold(currentEnemyData.GoldDrop);
            }
            else if(outcome == BattleOutcome.Spare)
            {
                GameManager.Instance.AddGold(currentEnemyData.GoldDrop);
            }
        }

        SceneManager.LoadScene("MainGame");

        EndTurn();

        this.enabled = false;
    }
    #endregion


    #region ETC
    public void MoveActHeart(Transform[] positions, int index)
    {
        if (Act_Heart != null && index >= 0 && index < positions.Length)
        {
            Act_Heart.transform.position = positions[index].position;
        }
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
    public void CloseSubMenu()
    {
        ClearSubButtons();
    }
    #endregion
}