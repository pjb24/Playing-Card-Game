using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 카드 문양
public enum E_CardSuit
{
    Spades,
    Hearts,
    Diamonds,
    Clovers,
    Back,
}

// 카드 숫자
public enum E_CardRank
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
    private E_CardSuit _suit;
    public E_CardSuit Suit => _suit;
    private E_CardRank _rank;
    public E_CardRank Rank => _rank;

	public Card(E_CardSuit suit, E_CardRank rank)
    {
        _suit = suit;
        _rank = rank;
    }

	public int GetValue()
	{
		int result = (int)_rank;

		if (_rank >= E_CardRank.Ten)
		{
			result = 10;
		}
		if (_rank == E_CardRank.Ace)
		{
			result = 11;
		}

		return result;
	}

    public override string ToString()
    {
        return $"{_rank} of {_suit}";
    }

    public void SetRank(E_CardRank rank)
    {
        _rank = rank;
    }

    public void SetSuit(E_CardSuit suit)
    {
        _suit = suit;
    }
}
