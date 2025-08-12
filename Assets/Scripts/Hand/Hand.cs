using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    protected List<Card> _cards = new();
    public IReadOnlyList<Card> Cards => _cards;

    private List<GameObject> _cardObjects = new();
    public IReadOnlyList<GameObject> CardObjects => _cardObjects;

    public void AddCard(Card card)
    {
        _cards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        _cards.Remove(card);
    }

    public void AddCardObject(GameObject gameObject)
    {
        _cardObjects.Add(gameObject);
    }

    public void RemoveCardObject(GameObject gameObject)
    {
        _cardObjects.Remove(gameObject);
    }

    public void Clear()
    {
        _cards.Clear();

        foreach (var cardObj in _cardObjects)
        {
            Object.Destroy(cardObj);
        }
        _cardObjects.Clear();
    }

    // 핸드의 점수 계산
    public int GetValue()
    {
        int total = 0;
        int aceCount = 0;

        foreach (Card card in _cards)
        {
            int value = card.GetValue();
            total += value;
            if (card.Rank == E_CardRank.Ace)
            {
                aceCount++;
            }
        }

        // Adjust for Ace (11 or 1)
        while (total > 21 && aceCount > 0)
        {
            total -= 10; // Ace from 11 -> 1
            aceCount--;
        }

        return total;
    }

    public bool IsBlackjack() => _cards.Count == 2 && GetValue() == 21;

    public bool IsBust() => GetValue() > 21;
}
