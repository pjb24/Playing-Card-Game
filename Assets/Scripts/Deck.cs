using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> cards;
    private System.Random random = new System.Random();

    public Deck(int numberOfDecks = 1)
    {
        InitializeDeck(numberOfDecks);
        Shuffle();
    }

    // �Է��� ������ ���� ����Ͽ� ���� �����Ѵ�.
    public void InitializeDeck(int numberOfDecks)
    {
        cards = new List<Card>();

        for (int i = 0; i < numberOfDecks; i++)
        {
            foreach (E_Suit suit in System.Enum.GetValues(typeof(E_Suit)))
            {
                foreach (E_Rank rank in System.Enum.GetValues(typeof(E_Rank)))
                {
                    cards.Add(new Card(suit, rank));
                }
            }
        }
    }

    // ������ ���� ���´�.
    // Fisher-Yates ���
    public void Shuffle()
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            Card temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
    }

    // �� �� ī�带 ��ȯ�ϰ� ������ �����Ѵ�.
    public Card DrawCard()
    {
        if (cards.Count == 0)
        {
            Debug.LogWarning("Deck is empty!");
            return null;
        }

        Card card = cards[0];
        cards.RemoveAt(0);

        return card;
    }

    // ���� ���� ī�� ���� Ȯ��
    public int CardsRemaining()
    {
        return cards.Count;
    }
}
