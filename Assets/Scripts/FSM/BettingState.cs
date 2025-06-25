using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BettingState : IGameState
{
    private Player currentPlayer;
    private PlayerHand currentHand;

    private int betAmount;

    public BettingState()
    {
        betAmount = 0;
    }

    public void Enter()
    {
        currentPlayer = GameManager.Instance.characterManager.GetNextBettingPlayer();
        if (currentPlayer == null)
        {
            GameManager.Instance.characterManager.ResetTurnOrder();
            // 모든 플레이어 베팅 완료, 카드 분배 페이즈로 전이
            GameManager.Instance.ChangeState(new DealingState());

            return;
        }

        currentHand = currentPlayer.GetNextBettingHand();
        if (currentHand == null)
        {
            GameManager.Instance.characterManager.MarkCurrentPlayerDoneBetting();
            // 현재 플레이어의 핸드들에 대한 모든 베팅이 완료, 다음 플레이어의 베팅 페이즈로 전이
            GameManager.Instance.ChangeState(new BettingState());

            return;
        }

        GameManager.Instance.uiManager.ChangeToBetPanel();

        // Hide join button
        GameManager.Instance.uiManager.button_Join.visible = false;

        // Button subscribe function
        GameManager.Instance.uiManager.button_BetReset.clicked += HandleBetReset;
        GameManager.Instance.uiManager.button_Bet1.clicked += HandleBet1;
        GameManager.Instance.uiManager.button_Bet2.clicked += HandleBet2;
        GameManager.Instance.uiManager.button_Bet3.clicked += HandleBet3;
        GameManager.Instance.uiManager.button_Bet4.clicked += HandleBet4;
        GameManager.Instance.uiManager.button_BetMax.clicked += HandleBetMax;
        GameManager.Instance.uiManager.button_BetConfirm.clicked += HandleBetConfirm;
    }

    public void Exit()
    {
        // Unsubscribe function
        GameManager.Instance.uiManager.button_BetReset.clicked -= HandleBetReset;
        GameManager.Instance.uiManager.button_Bet1.clicked -= HandleBet1;
        GameManager.Instance.uiManager.button_Bet2.clicked -= HandleBet2;
        GameManager.Instance.uiManager.button_Bet3.clicked -= HandleBet3;
        GameManager.Instance.uiManager.button_Bet4.clicked -= HandleBet4;
        GameManager.Instance.uiManager.button_BetMax.clicked -= HandleBetMax;
        GameManager.Instance.uiManager.button_BetConfirm.clicked -= HandleBetConfirm;
    }

    public void Update()
    {
    }

    public void UpdateUI()
    {
        GameManager.Instance.uiManager.label_BetAmount.text = betAmount.ToString("N0");
        GameManager.Instance.uiManager.label_PlayerChip.text = (currentPlayer.Chips - betAmount).ToString("N0");
    }

    public void HandleBetReset()
    {
        betAmount = 0;
        GameManager.Instance.chipFactory.ResetChips();
        UpdateUI();
    }

    public void HandleBet1()
    {
        if (betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (currentPlayer.Chips - betAmount < (int)E_ChipValue.Bet1)
        {
            return;
        }

        if (betAmount + (int)E_ChipValue.Bet1 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        betAmount += (int)E_ChipValue.Bet1;

        // Animate chips
        GameManager.Instance.chipFactory.CreateChipType1();
        GameManager.Instance.chipFactory.UpdateChipPosition();

        UpdateUI();
    }

    public void HandleBet2()
    {
        if (betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (currentPlayer.Chips - betAmount < (int)E_ChipValue.Bet2)
        {
            return;
        }

        if (betAmount + (int)E_ChipValue.Bet2 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        betAmount += (int)E_ChipValue.Bet2;

        // Animate chips
        GameManager.Instance.chipFactory.CreateChipType2();
        GameManager.Instance.chipFactory.UpdateChipPosition();

        UpdateUI();
    }

    public void HandleBet3()
    {
        if (betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (currentPlayer.Chips - betAmount < (int)E_ChipValue.Bet3)
        {
            return;
        }

        if (betAmount + (int)E_ChipValue.Bet3 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        betAmount += (int)E_ChipValue.Bet3;

        // Animate chips
        GameManager.Instance.chipFactory.CreateChipType3();
        GameManager.Instance.chipFactory.UpdateChipPosition();

        UpdateUI();
    }

    public void HandleBet4()
    {
        if (betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (currentPlayer.Chips - betAmount < (int)E_ChipValue.Bet4)
        {
            return;
        }

        if (betAmount + (int)E_ChipValue.Bet4 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        betAmount += (int)E_ChipValue.Bet4;

        // Animate chips
        GameManager.Instance.chipFactory.CreateChipType4();
        GameManager.Instance.chipFactory.UpdateChipPosition();

        UpdateUI();
    }

    public void HandleBetMax()
    {
        if (betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        GameManager.Instance.chipFactory.ResetChips();

        if (currentPlayer.Chips < (int)E_ChipValue.BetMax)
        {
            int tempChips = currentPlayer.Chips;
            int countType4 = tempChips / (int)E_ChipValue.Bet4;
            tempChips -= countType4 * (int)E_ChipValue.Bet4;
            int countType3 = tempChips / (int)E_ChipValue.Bet3;
            tempChips -= countType3 * (int)E_ChipValue.Bet3;
            int countType2 = tempChips / (int)E_ChipValue.Bet2;
            tempChips -= countType2 * (int)E_ChipValue.Bet2;
            int countType1 = tempChips / (int)E_ChipValue.Bet1;

            betAmount = countType4 * (int)E_ChipValue.Bet4
                + countType3 * (int)E_ChipValue.Bet3
                + countType2 * (int)E_ChipValue.Bet2
                + countType1 * (int)E_ChipValue.Bet1;

            for (int i = 0; i < countType1; i++)
            {
                GameManager.Instance.chipFactory.CreateChipType1();
            }
            for (int i = 0; i < countType2; i++)
            {
                GameManager.Instance.chipFactory.CreateChipType2();
            }
            for (int i = 0; i < countType3; i++)
            {
                GameManager.Instance.chipFactory.CreateChipType3();
            }
            for (int i = 0; i < countType4; i++)
            {
                GameManager.Instance.chipFactory.CreateChipType4();
            }
        }
        else
        {
            betAmount = (int)E_ChipValue.BetMax;
            GameManager.Instance.chipFactory.CreateChipType5();
        }

        // Animate chips
        GameManager.Instance.chipFactory.UpdateChipPosition();

        UpdateUI();
    }

    public void HandleBetConfirm()
    {
        currentPlayer.PlaceBet(currentHand, betAmount);

        // 다음 베팅 핸드로 이동
        GameManager.Instance.ChangeState(new BettingState());
    }
}
