using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class OnCardDealtCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnCardDealtDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnCardDealtDTO>(payload);

        Debug.Log("OnCardDealt, " + "�÷��̾�: " + dto.playerName + "�� " + "�ڵ� ID: " + dto.handId + "�� " + "ī�� " + dto.cardRank + " of " + dto.cardSuit + "��/�� �й��մϴ�." + " �÷��̾� Guid: " + dto.playerGuid);

        Player player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        Card card = new Card(dto.cardSuit, dto.cardRank);
        hand.AddCard(card);

        if (hand.Cards.Count == 1)
        {
            WorkForUIAtFirst(card, hand);
        }
        else
        {
            WorkForUI(card, hand);
        }
    }

    private void WorkForUIAtFirst(Card card, PlayerHand hand)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.InstancingCardToPlayer(card, hand);

            int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);
            GameManager.Instance.uiManager.CreateLabelCardValuePlayer(handIndex);
            GameManager.Instance.uiManager.CardValuePlayerVisible(handIndex);

            Vector3 targetPosition = GameManager.Instance.GetHandPosition(hand);
            GameManager.Instance.uiManager.RequestCardValueUIPositionUpdate_Register(targetPosition, handIndex);
            GameManager.Instance.uiManager.RequestCardValueUIPositionUpdate_Y_Register(handIndex);

            GameManager.Instance.uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
        });
    }

    private void WorkForUI(Card card, PlayerHand hand)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.InstancingCardToPlayer(card, hand);

            int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);

            if (hand.IsBlackjack())
            {
                GameManager.Instance.uiManager.CardValuePlayerSetText("Blackjack", handIndex);
            }
            else
            {
                GameManager.Instance.uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
            }
        });
    }
}
