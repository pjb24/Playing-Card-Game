using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerHiddenCardDealtCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnDealerHiddenCardDealtDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerHiddenCardDealtDTO>(payload);

        Debug.Log("OnDealerHiddenCardDealt, " + "딜러에게 숨겨진 카드를 분배합니다.");

        Dealer dealer = GameManager.Instance.characterManager.dealer;

        // Dealer Second Card - Hidden Card
        Card dealerCard = new Card(E_CardSuit.Back, E_CardRank.Back);
        dealer.Hand.AddCard(dealerCard);

        WorkForUI(dealerCard, dealer);

        yield return null;
    }

    private void WorkForUI(Card dealerCard, Dealer dealer)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.InstancingCardToDealer(dealerCard, dealer.Hand, true);
        });
    }
}
