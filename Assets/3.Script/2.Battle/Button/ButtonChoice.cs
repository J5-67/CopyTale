using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChoice : MonoBehaviour
{
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Transform[] MainButtonPosition;
    [SerializeField] private Transform[] SubButtonPosition;

    public int CurrentMainButtonIndex { get; private set; } = 0;
    public int CurrentSubButtonIndex { get; private set; } = 0;
    public bool IsSubMenuOpen { get; private set; } = false;

    private void Update()
    {
        HandleMovementInput();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            HandleZInput();
        }

        if (Input.GetKeyDown(KeyCode.X) && IsSubMenuOpen)
        {
            HandleXInput();
        }
    }

    public void ResetChoice()
    {
        IsSubMenuOpen = false;
        CurrentMainButtonIndex = 0;
        CurrentSubButtonIndex = 0;
    }

    private void HandleMovementInput()
    {
        int direction = 0;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = -1;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = 1;
        }

        if (!IsSubMenuOpen)
        {
            CurrentMainButtonIndex = GetNextIndex(CurrentMainButtonIndex, direction, MainButtonPosition.Length);
            
            battleManager.MoveActHeart(MainButtonPosition, CurrentMainButtonIndex);
        }
        else
        {
            int activeCount = battleManager.ActiveSubButtonCount;

            if (activeCount > 1)
            {
                CurrentSubButtonIndex = GetNextIndex(CurrentSubButtonIndex, direction, activeCount);

                battleManager.MoveActHeart(SubButtonPosition, CurrentSubButtonIndex);
            }
        }
    }

    private int GetNextIndex(int currentIndex, int direction, int arrayLength)
    {
        if (arrayLength == 0) return 0; // 배열 길이가 0이면 순환할 필요가 없어.

        currentIndex += direction;

        if (currentIndex < 0)
        {
            currentIndex = arrayLength - 1; // 맨 오른쪽 끝으로
        }
        else if (currentIndex >= arrayLength)
        {
            currentIndex = 0; // 맨 왼쪽 끝으로
        }

        return currentIndex;
    }

    private void HandleZInput()
    {
        if (!IsSubMenuOpen)
        {
            IsSubMenuOpen = true;

            CurrentSubButtonIndex = 0;

            battleManager.OpenSubMenu(CurrentMainButtonIndex);

            battleManager.MoveActHeart(SubButtonPosition, CurrentSubButtonIndex);
        }
        else
        {
            battleManager.StartSubAction(CurrentMainButtonIndex, CurrentSubButtonIndex);
        }
    }

    private void HandleXInput()
    {
        IsSubMenuOpen = false;

        CurrentSubButtonIndex = 0;

        battleManager.MoveActHeart(MainButtonPosition, CurrentMainButtonIndex);

        battleManager.CloseSubMenu();
    }

}

