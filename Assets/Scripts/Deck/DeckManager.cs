using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager
{
    private List<Card> shoe;
    private int numberOfDecks;
    private System.Random rng = new System.Random();

    // ���� ī�尡 18% ������ ��� ����
    private float reshuffleThreshold = 0.18f;

    // shoe�� ���� ī�� ���� Ȯ��
    public int RemainingCardCount => shoe.Count;
    public float RemainingRatio => (float)RemainingCardCount / (numberOfDecks * 52);
    public bool ShouldReshuffle => RemainingRatio < reshuffleThreshold;


    // ���� ���� ���� �� ���� Ȯ��
    public void PrepareNextRound()
    {
        if (ShouldReshuffle)
        {
            InitializeShoe();
        }
    }

    public DeckManager(int numberOfDecks = 6)
    {
        this.numberOfDecks = numberOfDecks;
        InitializeShoe();
    }

    private void InitializeShoe()
    {
        shoe = new List<Card>();
        for (int i = 0; i < numberOfDecks; i++)
        {
            shoe.AddRange(GenerateStandardDeck());
        }
        Shuffle();
    }

    private List<Card> GenerateStandardDeck()
    {
        var cards = new List<Card>();
        foreach (E_Suit suit in System.Enum.GetValues(typeof(E_Suit)))
        {
            foreach (E_Rank rank in System.Enum.GetValues(typeof(E_Rank)))
            {
                cards.Add(new Card(suit, rank));
            }
        }

        return cards;
    }

    // ������ ���� ���´�.
    // Fisher-Yates ���
    public void Shuffle()
    {
        int n = shoe.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            Card temp = shoe[i];
            shoe[i] = shoe[j];
            shoe[j] = temp;
        }
    }

    // �� �� ī�带 ��ȯ�ϰ� shoe���� �����Ѵ�.
    public Card DrawCard()
    {
        if (shoe.Count == 0)
        {
            // �ڵ� ����
            // ������ ReshuffleCardCount ���Ϸ� ������ ����
            InitializeShoe();
        }

        Card card = shoe[0];
        shoe.RemoveAt(0);

        return card;
    }
}
