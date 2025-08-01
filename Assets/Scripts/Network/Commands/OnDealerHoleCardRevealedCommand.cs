using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDealerHoleCardRevealedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnDealerHoleCardRevealedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerHoleCardRevealedDTO>(payload);

        Debug.Log("OnDealerHoleCardRevealed, " + "������ ������ ī��: " + dto.cardRank + " of " + dto.cardSuit + "��/�� �����մϴ�.");

        // ������ ���� ī�� ����
        Card hiddenCard = GameManager.Instance.characterManager.dealer.Hand.Cards[1];

        hiddenCard.SetRank(dto.cardRank);
        hiddenCard.SetSuit(dto.cardSuit);

        WorkForUI();
    }

    private void WorkForUI()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.RevealHoleCard();

            if (GameManager.Instance.characterManager.dealer.Hand.IsBlackjack())
            {
                GameManager.Instance.uiManager.CardValueDealerSetText("Blackjack");
            }
            else
            {
                GameManager.Instance.uiManager.CardValueDealerSetText(GameManager.Instance.characterManager.dealer.Hand.GetValue().ToString());
            }
        });
    }
}
