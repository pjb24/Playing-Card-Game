using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    public string Id { get; private set; }
    public string DisplayName { get; private set; }

    // 소유한 Chip
    public int Chips { get; private set; }

    // 플레이어가 Split을 하면 여러 개의 핸드를 가질 수 있음
    public List<PlayerHand> Hands { get; private set; } = new();
    public bool IsFinishedTurn { get; set; } = false;
    public bool IsFinishedBetting { get; set; } = false;

    public Player(string id, string displayName, int initialChips)
    {
        Id = id;
        DisplayName = displayName;
        Chips = initialChips;

        Hands.Add(new PlayerHand());
    }

    public PlayerHand GetActiveHand()
    {
        return Hands.FirstOrDefault(h => !h.IsCompleted);
    }

    public PlayerHand GetNextBettingHand()
    {
        return Hands.FirstOrDefault(h => !h.IsBetConfirmed);
    }

    public void PlaceBet(PlayerHand hand, int amount)
    {
        if (amount > Chips)
        {
            throw new InvalidOperationException("Not enough chips to place bet.");
        }

        hand.IsBetConfirmed = true;
        hand.Bet(amount);

        Chips -= amount;
    }

    public void Stand(PlayerHand hand)
    {
        hand.IsCompleted = true;
    }

    public void ResetForNextRound()
    {
        IsFinishedTurn = false;
        IsFinishedBetting = false;
        foreach (var hand in Hands)
        {
            hand.ResetChipAll();
            hand.Clear();
        }
        Hands.Clear();
        Hands.Add(new PlayerHand());
    }

    public void Blackjack(PlayerHand hand)
    {
        int payout = (int)(hand.BetAmount * 2.5f);
        Chips += payout;
    }

    public void Win(PlayerHand hand)
    {
        int payout = (int)(hand.BetAmount * 2.0f);
        Chips += payout;
    }

    public void Push(PlayerHand hand)
    {
        int payout = hand.BetAmount;
        Chips += payout;
    }

    public void Lose(PlayerHand hand)
    {

    }

    public void DoubleDown(PlayerHand hand)
    {
        if (hand.BetAmount > Chips)
        {
            Debug.Log("Not enough chips to double down.");
            return;
        }

        Chips -= hand.BetAmount;

        hand.Bet(hand.BetAmount * 2);
    }

    public PlayerHand AppendHand()
    {
        PlayerHand hand = new PlayerHand();
        Hands.Add(hand);
        return hand;
    }

    public PlayerHand InsertHand(int index)
    {
        PlayerHand hand = new PlayerHand();
        Hands.Insert(index, hand);
        return hand;
    }
}
