using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : Hand
{
    public int BetAmount { get; private set; }
    public bool IsCompleted { get; set; } = false;
    public bool IsBetConfirmed { get; set; } = false;

    public void Bet(int amount)
    {
        BetAmount = amount;
    }

    public bool CanSplit()
    {
        if (Cards.Count != 2)
        {
            return false;
        }

        if (Cards[0].Rank != Cards[1].Rank)
        {
            return false;
        }

        return true;
    }
}
