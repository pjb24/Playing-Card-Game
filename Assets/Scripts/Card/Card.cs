using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 카드 문양
public enum E_Suit
{
    Spades,
    Hearts,
    Diamonds,
    Clovers,
    Back,
}

// 카드 숫자
public enum E_Rank
{
    Back = 0,
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13,
}

public class Card
{
    public E_Suit Suit { get; private set; }
    public E_Rank Rank { get; private set; }

	public Card(E_Suit suit, E_Rank rank)
    {
        this.Suit = suit;
        this.Rank = rank;
    }

	public int GetValue()
	{
		int result = (int)Rank;

		if (Rank >= E_Rank.Ten)
		{
			result = 10;
		}
		if (Rank == E_Rank.Ace)
		{
			result = 11;
		}

		return result;
	}

    public override string ToString()
    {
        return $"{Rank} of {Suit}";
    }

    public void SetRank(E_Rank rank)
    {
        Rank = rank;
    }

    public void SetSuit(E_Suit suit)
    {
        Suit = suit;
    }
}
