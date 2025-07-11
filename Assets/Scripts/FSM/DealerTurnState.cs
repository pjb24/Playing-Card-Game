using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerTurnState : IGameState
{
    public void Enter()
    {
        GameManager.Instance.StartCoroutine(DealerBehaviorCoroutine());
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }

    private IEnumerator DealerBehaviorCoroutine()
    {
        // ������ ���� ī�� ����
        GameManager.Instance.RevealHoleCard();

        if (GameManager.Instance.characterManager.dealer.Hand.IsBlackjack())
        {
            GameManager.Instance.uiManager.label_CardValue_Dealer.text = "Blackjack";
        }
        else
        {
            GameManager.Instance.uiManager.label_CardValue_Dealer.text = GameManager.Instance.characterManager.dealer.Hand.GetValue().ToString();
        }

        yield return new WaitForSeconds(1f);

        while (GameManager.Instance.characterManager.dealer.ShouldHit())
        {
            Card card = GameManager.Instance.deckManager.DrawCard();
            GameManager.Instance.characterManager.dealer.Hand.AddCard(card);
            GameManager.Instance.InstancingCardToDealer(card, GameManager.Instance.characterManager.dealer.Hand);

            yield return new WaitForSeconds(1f);

            GameManager.Instance.uiManager.label_CardValue_Dealer.text = GameManager.Instance.characterManager.dealer.Hand.GetValue().ToString();
        }

        GameManager.Instance.ChangeState(new ResultState());
    }
}
