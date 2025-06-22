using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager
{
    private List<Card> shoe;
    private int numberOfDecks;
    private System.Random rng = new System.Random();

    // 남은 카드가 18% 이하일 경우 셔플
    private float reshuffleThreshold = 0.18f;

    // shoe에 남은 카드 개수 확인
    public int RemainingCardCount => shoe.Count;
    public float RemainingRatio => (float)RemainingCardCount / (numberOfDecks * 52);
    public bool ShouldReshuffle => RemainingRatio < reshuffleThreshold;


    // 다음 라운드 시작 전 셔플 확인
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

    // 구성된 덱을 섞는다.
    // Fisher-Yates 방식
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

    // 맨 위 카드를 반환하고 shoe에서 제거한다.
    public Card DrawCard()
    {
        if (shoe.Count == 0)
        {
            // 자동 리셋
            // 보통은 ReshuffleCardCount 이하로 남으면 셔플
            InitializeShoe();
        }

        Card card = shoe[0];
        shoe.RemoveAt(0);

        return card;
    }
}
