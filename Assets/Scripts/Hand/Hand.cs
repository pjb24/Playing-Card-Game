using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    protected List<Card> cards = new();
    public IReadOnlyList<Card> Cards => cards;
    public List<GameObject> cardObjects = new();

    public void AddCard(Card card)
    {
        cards.Add(card);
    }

    public void RemoveCard(Card card)
    {
        cards.Remove(card);
    }

    public void Clear()
    {
        cards.Clear();

        foreach (var cardObj in cardObjects)
        {
            Object.Destroy(cardObj);
        }
        cardObjects.Clear();
    }

    // 핸드의 점수 계산
    public int GetValue()
    {
        int total = 0;
        int aceCount = 0;

        foreach (Card card in cards)
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

    public bool IsBlackjack() => cards.Count == 2 && GetValue() == 21;

    public bool IsBust() => GetValue() > 21;
}
