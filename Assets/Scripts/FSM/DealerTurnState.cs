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
        // µô·¯ÀÇ È÷µç Ä«µå ¿ÀÇÂ
        GameManager.Instance.RevealHoleCard();

        yield return new WaitForSeconds(1f);

        while (GameManager.Instance.characterManager.dealer.ShouldHit())
        {
            Card card = GameManager.Instance.deckManager.DrawCard();
            GameManager.Instance.characterManager.dealer.Hand.AddCard(card);
            GameManager.Instance.InstancingCardToDealer(card, GameManager.Instance.characterManager.dealer.Hand);

            yield return new WaitForSeconds(1f);
        }

        GameManager.Instance.ChangeState(new ResultState());
    }
}
