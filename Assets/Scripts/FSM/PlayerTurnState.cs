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
            // ��� �÷��̾��� �ൿ �Ϸ� ���� ������ ����
            GameManager.Instance.ChangeState(new DealerTurnState());

            return;
        }

        currentHand = currentPlayer.GetActiveHand();
        if (currentHand == null)
        {
            // ���� �÷��̾��� �ൿ �Ϸ� �÷��� ����
            GameManager.Instance.characterManager.MarkCurrentPlayerDone();
            // ���� �÷��̾��� ��� �ڵ忡 ���� �ൿ �Ϸ�, ���� �÷��̾� ������ ����
            GameManager.Instance.ChangeState(new PlayerTurnState());

            return;
        }

        // �ڵ��� �ൿ�� ������ �� ī�尡 2�� ���� ������ ī�带 ����
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
        // ���ÿ� ����� Ĩ�� ������ ���� Ĩ�� �ʿ�
        if (currentPlayer.Chips < currentHand.BetAmount)
        {
            return;
        }

        // �ڵ忡 ī�尡 2��, ī���� ���� �Ǵ� ���ڰ� ���ƾ� ��
        if (!currentHand.CanSplit())
        {
            //return;
        }

        // ���ο� �ڵ带 ���� �ڵ��� ������ �߰�
        PlayerHand newHand = currentPlayer.InsertHand(currentPlayer.Hands.IndexOf(currentHand) + 1);

        // ���ο� �ڵ忡 ���� �Է�
        currentPlayer.PlaceBet(newHand, currentHand.BetAmount);

        // ���� �ڵ��� 2��° ī�带 ���ο� �ڵ�� ����
        Card splitCard = currentHand.Cards[1];
        currentHand.RemoveCard(splitCard);
        newHand.AddCard(splitCard);

        // ī�� ������Ʈ ����
        GameObject splitCardObj = currentHand.cardObjects[1];
        currentHand.cardObjects.Remove(splitCardObj);
        newHand.cardObjects.Add(splitCardObj);

        // ��� ī�� ��ġ ����

        // �� �ڵ忡 Ĩ ����

        // ��� Ĩ�� ��ġ�� ����

        // UI ������Ʈ. Player Info, Card Value

        // PlayerTurnState �ٽ� ����
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
