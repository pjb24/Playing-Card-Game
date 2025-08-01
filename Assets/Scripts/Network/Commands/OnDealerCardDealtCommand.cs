using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerCardDealtCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnDealerCardDealtDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerCardDealtDTO>(payload);

        Debug.Log("OnDealerCardDealt, " + "딜러에게 카드: " + dto.cardRank + " of " + dto.cardSuit + "을/를 분배합니다.");

        Dealer dealer = GameManager.Instance.characterManager.dealer;

        Card dealerCard = new Card(dto.cardSuit, dto.cardRank);
        dealer.Hand.AddCard(dealerCard);

        if (dealer.Hand.Cards.Count == 1)
        {
            WorkForUIAtFirst(dealerCard, dealer);
        }
        else
        {
            WorkForUI(dealerCard, dealer);
        }
    }

    private void WorkForUIAtFirst(Card dealerCard, Dealer dealer)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.InstancingCardToDealer(dealerCard, dealer.Hand);

            GameManager.Instance.uiManager.CreateLabelCardValueDealer();
            GameManager.Instance.uiManager.RequestUpdateCardValueDealerPosition();
            GameManager.Instance.uiManager.CardValueDealerSetText(dealer.Hand.GetValue().ToString());
        });
    }

    private void WorkForUI(Card dealerCard, Dealer dealer)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.InstancingCardToDealer(dealerCard, GameManager.Instance.characterManager.dealer.Hand);

            GameManager.Instance.uiManager.CardValueDealerSetText(GameManager.Instance.characterManager.dealer.Hand.GetValue().ToString());
        });
    }
}
