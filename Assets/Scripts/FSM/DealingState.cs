using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealingState : IGameState
{
    public void Enter()
    {
        // UI º¯°æ
        GameManager.Instance.uiManager.ChangeToPlayerActionPanel();

        GameManager.Instance.StartCoroutine(DealCardsCoroutine());
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }

    private IEnumerator DealCardsCoroutine()
    {
        var players = GameManager.Instance.characterManager.Players;
        var dealer = GameManager.Instance.characterManager.dealer;

        // Player First Card
        foreach (var player in players)
        {
            foreach (var hand in player.Hands)
            {
                Card card = GameManager.Instance.deckManager.DrawCard();
                hand.AddCard(card);
                GameManager.Instance.InstancingCardToPlayer(card, hand);

                int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);
                GameManager.Instance.uiManager.CreateLabelCardValuePlayer(handIndex);
                GameManager.Instance.uiManager.CardValuePlayerVisible(handIndex);

                Vector3 targetPosition = GameManager.Instance.GetHandPosition(hand);
                GameManager.Instance.uiManager.RequestCardValueUIPositionUpdate_Register(targetPosition, handIndex);
                GameManager.Instance.uiManager.RequestCardValueUIPositionUpdate_Y_Register(handIndex);

                GameManager.Instance.uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);

                yield return new WaitForSeconds(0.3f);
            }
        }

        // Dealer First Card
        Card dealerCard1 = GameManager.Instance.deckManager.DrawCard();
        dealer.Hand.AddCard(dealerCard1);
        GameManager.Instance.InstancingCardToDealer(dealerCard1, dealer.Hand);

        GameManager.Instance.uiManager.CreateLabelCardValueDealer();
        GameManager.Instance.uiManager.RequestUpdateCardValueDealerPosition();
        GameManager.Instance.uiManager.CardValueDealerSetText(dealer.Hand.GetValue().ToString());
        
        yield return new WaitForSeconds(0.3f);

        // Player Second Card
        foreach (var player in players)
        {
            foreach (var hand in player.Hands)
            {
                Card card = GameManager.Instance.deckManager.DrawCard();
                hand.AddCard(card);
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

                yield return new WaitForSeconds(0.3f);
            }
        }

        // Dealer Second Card - Hidden Card
        Card dealerCard2 = GameManager.Instance.deckManager.DrawCard();
        dealer.Hand.AddCard(dealerCard2);
        GameManager.Instance.InstancingCardToDealer(dealerCard2, dealer.Hand, true);

        yield return new WaitForSeconds(0.3f);

        GameManager.Instance.ChangeState(new PlayerTurnState());
    }
}
