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
        // 딜러의 히든 카드 오픈
        GameManager.Instance.RevealHoleCard();

        if (GameManager.Instance.characterManager.dealer.Hand.IsBlackjack())
        {
            GameManager.Instance.uiManager.CardValueDealerSetText("Blackjack");
        }
        else
        {
            GameManager.Instance.uiManager.CardValueDealerSetText(GameManager.Instance.characterManager.dealer.Hand.GetValue().ToString());
        }

        yield return new WaitForSeconds(1f);

        while (GameManager.Instance.characterManager.dealer.ShouldHit())
        {
            Card card = GameManager.Instance.deckManager.DrawCard();
            GameManager.Instance.characterManager.dealer.Hand.AddCard(card);
            GameManager.Instance.InstancingCardToDealer(card, GameManager.Instance.characterManager.dealer.Hand);

            yield return new WaitForSeconds(1f);

            GameManager.Instance.uiManager.CardValueDealerSetText(GameManager.Instance.characterManager.dealer.Hand.GetValue().ToString());
        }

        GameManager.Instance.ChangeState(new ResultState());
    }
}
