using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : IGameState
{
    private Player currentPlayer;
    private PlayerHand currentHand;

    public void Enter()
    {
        currentPlayer = GameManager.Instance.characterManager.GetNextActivePlayer();
        if (currentPlayer == null)
        {
            // 모든 플레이어의 행동 완료 딜러 턴으로 전이
            GameManager.Instance.ChangeState(new DealerTurnState());

            return;
        }

        currentHand = currentPlayer.GetActiveHand();
        if (currentHand == null)
        {
            // 현재 플레이어의 행동 완료 플래그 설정
            GameManager.Instance.characterManager.MarkCurrentPlayerDone();
            // 현재 플레이어의 모든 핸드에 대한 행동 완료, 다음 플레이어 턴으로 전이
            GameManager.Instance.ChangeState(new PlayerTurnState());

            return;
        }

        // 핸드의 행동을 시작할 때 카드가 2장 보다 적으면 카드를 받음
        if (currentHand.Cards.Count < 2)
        {
            Card currentHandCard = GameManager.Instance.deckManager.DrawCard();
            currentHand.AddCard(currentHandCard);
            GameManager.Instance.InstancingCardToPlayer(currentHandCard, currentHand);
        }

        GameManager.Instance.uiManager.ChangeToPlayerActionPanel();

        // Button subscribe function
        GameManager.Instance.uiManager.button_Hit.clicked += HandleHit;
        GameManager.Instance.uiManager.button_Stand.clicked += HandleStand;
        GameManager.Instance.uiManager.button_Split.clicked += HandleSplit;
        GameManager.Instance.uiManager.button_DoubleDown.clicked += HandleDoubleDown;
    }

    public void Exit()
    {
        // Unsubscribe function
        GameManager.Instance.uiManager.button_Hit.clicked -= HandleHit;
        GameManager.Instance.uiManager.button_Stand.clicked -= HandleStand;
        GameManager.Instance.uiManager.button_Split.clicked -= HandleSplit;
        GameManager.Instance.uiManager.button_DoubleDown.clicked -= HandleDoubleDown;
    }

    public void Update()
    {
    }

    public void HandleHit()
    {
        Card card = GameManager.Instance.deckManager.DrawCard();
        currentHand.AddCard(card);
        GameManager.Instance.InstancingCardToPlayer(card, currentHand);

        GameManager.Instance.uiManager.label_CardValue_Player_01.text = currentHand.GetValue().ToString();

        if (currentHand.IsBust())
        {
            NextHand();
        }
    }

    public void HandleStand()
    {
        NextHand();
    }

    public void HandleSplit()
    {
        // 베팅에 사용한 칩과 동일한 양의 칩이 필요
        if (currentPlayer.Chips < currentHand.BetAmount)
        {
            return;
        }

        // 핸드에 카드가 2장, 카드의 숫자 또는 문자가 같아야 함
        if (!currentHand.CanSplit())
        {
            //return;
        }

        // 새로운 핸드를 현재 핸드의 오른편에 추가
        PlayerHand newHand = currentPlayer.InsertHand(currentPlayer.Hands.IndexOf(currentHand) + 1);

        // 새로운 핸드에 베팅 입력
        currentPlayer.PlaceBet(newHand, currentHand.BetAmount);

        // 현재 핸드의 2번째 카드를 새로운 핸드로 나눔
        Card splitCard = currentHand.Cards[1];
        currentHand.RemoveCard(splitCard);
        newHand.AddCard(splitCard);

        // 카드 오브젝트 나눔
        GameObject splitCardObj = currentHand.cardObjects[1];
        currentHand.cardObjects.Remove(splitCardObj);
        newHand.cardObjects.Add(splitCardObj);

        // 모든 카드 위치 갱신

        // 새 핸드에 칩 생성

        // 모든 칩의 위치를 갱신

        // UI 업데이트. Player Info, Card Value

        // PlayerTurnState 다시 진입
        GameManager.Instance.ChangeState(new PlayerTurnState());
    }

    public void HandleDoubleDown()
    {
        // Check player chip
        if (currentPlayer.Chips < currentHand.BetAmount)
        {
            return;
        }

        // Increase betting chips
        currentPlayer.DoubleDown(currentHand);

        // Animate chip
        GameManager.Instance.chipFactory.ResetChips();

        int tempChips = currentHand.BetAmount;
        int countType5 = tempChips / (int)E_ChipValue.BetMax;
        tempChips -= countType5 * (int)E_ChipValue.BetMax;
        int countType4 = tempChips / (int)E_ChipValue.Bet4;
        tempChips -= countType4 * (int)E_ChipValue.Bet4;
        int countType3 = tempChips / (int)E_ChipValue.Bet3;
        tempChips -= countType3 * (int)E_ChipValue.Bet3;
        int countType2 = tempChips / (int)E_ChipValue.Bet2;
        tempChips -= countType2 * (int)E_ChipValue.Bet2;
        int countType1 = tempChips / (int)E_ChipValue.Bet1;

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
        for (int i = 0; i < countType5; i++)
        {
            GameManager.Instance.chipFactory.CreateChipType5();
        }

        GameManager.Instance.chipFactory.UpdateChipPosition();

        UpdateUIChips();

        // Draw card
        Card card = GameManager.Instance.deckManager.DrawCard();
        currentHand.AddCard(card);
        GameManager.Instance.InstancingCardToPlayer(card, currentHand);

        GameManager.Instance.uiManager.label_CardValue_Player_01.text = currentHand.GetValue().ToString();

        // Move to next hand
        NextHand();
    }

    private void NextHand()
    {
        currentPlayer.Stand(currentHand);
        GameManager.Instance.ChangeState(new PlayerTurnState());
    }

    public void UpdateUIChips()
    {
        GameManager.Instance.uiManager.label_BetAmount.text = currentHand.BetAmount.ToString("N0");
        GameManager.Instance.uiManager.label_PlayerChip.text = currentPlayer.Chips.ToString("N0");
    }
}
