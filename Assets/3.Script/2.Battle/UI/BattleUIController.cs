using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BattleUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text statsText;

    private void Start()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.playerStatsUpdate += UpdateStatsDisplay;
            UpdateStatsDisplay();
        }

        UpdateStatsDisplay();
    }
    private void OnDestroy()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.playerStatsUpdate -= UpdateStatsDisplay;
        }
    }

    public void UpdateStatsDisplay()
    {
        if(GameManager.Instance == null)
        {
            return;
        }

        string playerName = GameManager.Instance.PlayerName;
        int currentLV = GameManager.Instance.PlayerLevel;
        int currentHP = GameManager.Instance.PlayerCurrentHP;
        int maxHP = GameManager.Instance.PlayerMaxHP;

        string newText = $"{playerName} LV {currentLV} HP {currentHP} / {maxHP}";

        if(statsText != null)
        {
            statsText.text = newText;
        }
    }
}
