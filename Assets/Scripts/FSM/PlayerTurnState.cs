using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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

            int handIndex = GameManager.Instance.characterManager.GetHandIndex(currentHand);
            GameManager.Instance.uiManager.CardValuePlayerSetText(currentHand.GetValue().ToString(), handIndex);
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

        int handIndex = GameManager.Instance.characterManager.GetHandIndex(currentHand);
        GameManager.Instance.uiManager.CardValuePlayerSetText(currentHand.GetValue().ToString(), handIndex);

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
        GameManager.Instance.UpdateAllPlayerHandPositions();

        // �� �ڵ忡 Ĩ ����
        GameManager.Instance.chipFactory.CreateChipsToFitValue(newHand.BetAmount, newHand);

        // ��� Ĩ�� ��ġ�� ����
        GameManager.Instance.chipFactory.UpdateAllChipsPosition();

        // UI ������Ʈ. Player Info, Card Value
        UpdateUICardValue();

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
        GameManager.Instance.chipFactory.ResetChips(currentHand);

        GameManager.Instance.chipFactory.CreateChipsToFitValue(currentHand.BetAmount, currentHand);

        GameManager.Instance.chipFactory.UpdateHandChipPosition(currentHand);

        UpdateUIChips();

        // Draw card
        Card card = GameManager.Instance.deckManager.DrawCard();
        currentHand.AddCard(card);
        GameManager.Instance.InstancingCardToPlayer(card, currentHand);

        int handIndex = GameManager.Instance.characterManager.GetHandIndex(currentHand);
        GameManager.Instance.uiManager.CardValuePlayerSetText(currentHand.GetValue().ToString(), handIndex);

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

    private void UpdateUICardValue()
    {
        foreach (var player in GameManager.Instance.characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                UpdateUICardValue(hand);
            }
        }
    }

    private void UpdateUICardValue(PlayerHand hand)
    {
        int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);
        GameManager.Instance.uiManager.CardValuePlayerVisible(handIndex);

        Vector3 targetPosition = GameManager.Instance.GetHandPosition(hand);
        GameManager.Instance.uiManager.RequestCardValueUIPositionUpdate(targetPosition, handIndex);

        GameManager.Instance.uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
    }
}
