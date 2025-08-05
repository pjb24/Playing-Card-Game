using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using DG.Tweening;

public class OnDealerCardDealtCommand : IGameCommand
{
    private ConcurrentQueue<Card> _queue = new();
    public IReadOnlyCollection<Card> Queue => _queue;
    private readonly object _lock = new();
    private volatile bool _isCoroutineRunning = false;

    public IEnumerator Execute(string payload)
    {
        OnDealerCardDealtDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnDealerCardDealtDTO>(payload);

        Debug.Log("OnDealerCardDealt, " + "딜러에게 카드: " + dto.cardRank + " of " + dto.cardSuit + "을/를 분배합니다.");

        Dealer dealer = GameManager.Instance.characterManager.dealer;

        if (dealer.Hand.Cards.Count == 0)
        {
            WorkForUIAtFirst(dealer, dto.cardRank, dto.cardSuit);
        }
        else
        {
            WorkForUI(dealer, dto.cardRank, dto.cardSuit);
        }

        yield return null;
    }

    private void WorkForUIAtFirst(Dealer dealer, E_CardRank cardRank, E_CardSuit cardSuit)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Card dealerCard = new Card(cardSuit, cardRank);
            dealer.Hand.AddCard(dealerCard);

            GameManager.Instance.InstancingCardToDealer(dealerCard, dealer.Hand);

            GameManager.Instance.uiManager.CreateLabelCardValueDealer();
            GameManager.Instance.uiManager.RequestUpdateCardValueDealerPosition();
            GameManager.Instance.uiManager.CardValueDealerSetText(dealer.Hand.GetValue().ToString());
        });
    }

    private void WorkForUI(Dealer dealer, E_CardRank cardRank, E_CardSuit cardSuit)
    {
        Card dealerCard = new Card(cardSuit, cardRank);

        _queue.Enqueue(dealerCard);

        lock (_lock)
        {
            if (!_isCoroutineRunning)
            {
                _isCoroutineRunning = true;
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    GameManager.Instance.StartCoroutine(ExecuteCoroutine());
                });
            }
        }
    }

    private IEnumerator ExecuteCoroutine()
    {
        while (_queue.TryDequeue(out Card dealerCard))
        {
            yield return GameManager.Instance.StartCoroutine(WorkCoroutine(dealerCard));
        }

        _isCoroutineRunning = false;
    }

    private IEnumerator WorkCoroutine(Card dealerCard)
    {
        Tween tween = GameManager.Instance.InstancingCardToDealer(dealerCard, GameManager.Instance.characterManager.dealer.Hand);

        yield return tween.WaitForCompletion();
        GameManager.Instance.characterManager.dealer.Hand.AddCard(dealerCard);
        GameManager.Instance.uiManager.CardValueDealerSetText(GameManager.Instance.characterManager.dealer.Hand.GetValue().ToString());

        yield return null;
    }
}
