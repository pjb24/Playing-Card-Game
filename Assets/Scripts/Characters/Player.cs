using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player
{
    private string _id;
    public string Id => _id;

    private string _displayName;
    public string DisplayName => _displayName;

    // 소유한 Chip
    private int _chips = 0;
    public int Chips => _chips;

    // 플레이어가 Split을 하면 여러 개의 핸드를 가질 수 있음
    private List<PlayerHand> _hands = new();
    public IReadOnlyList<PlayerHand> Hands => _hands;

    private bool _isFinishedTurn = false;
    public bool IsFinishedTurn => _isFinishedTurn;

    private bool _isFinishedBetting = false;
    public bool IsFinishedBetting => _isFinishedBetting;

    public Player(string id, string displayName)
    {
        _id = id;
        _displayName = displayName;
    }

    public PlayerHand GetHandByGuid(string guid)
    {
        return _hands.Find(h => h.Id == guid);
    }

    public void AddHand(string handId)
    {
        PlayerHand hand = new();
        hand.SetHandId(handId);
        _hands.Add(hand);
    }

    public void SetPlayerChips(int chips)
    {
        _chips = chips;
    }

    public PlayerHand GetActiveHand()
    {
        return _hands.FirstOrDefault(h => !h.IsCompleted);
    }

    public PlayerHand GetNextBettingHand()
    {
        return _hands.FirstOrDefault(h => !h.IsBetConfirmed);
    }

    public void PlaceBet(PlayerHand hand, int amount)
    {
        if (amount > _chips)
        {
            throw new InvalidOperationException("Not enough chips to place bet.");
        }

        hand.SetIsBetConfirmed();
        hand.Bet(amount);

        _chips -= amount;
    }

    public void Stand(PlayerHand hand)
    {
        hand.SetIsCompleted();
    }

    public void ResetForNextRound()
    {
        _isFinishedTurn = false;
        _isFinishedBetting = false;
        foreach (var hand in _hands)
        {
            hand.ResetChipAll();
            hand.Clear();
        }
        _hands.Clear();
        _hands.Add(new PlayerHand());
    }

    public void ResetForNextRound_Network()
    {
        _isFinishedTurn = false;
        _isFinishedBetting = false;
        foreach (var hand in _hands)
        {
            hand.ResetChipAll();
            hand.Clear();
        }
        _hands.Clear();
    }

    public void Blackjack(PlayerHand hand)
    {
        int payout = (int)(hand.BetAmount * 2.5f);
        _chips += payout;
    }

    public void Win(PlayerHand hand)
    {
        int payout = (int)(hand.BetAmount * 2.0f);
        _chips += payout;
    }

    public void Push(PlayerHand hand)
    {
        int payout = hand.BetAmount;
        _chips += payout;
    }

    public void Lose(PlayerHand hand)
    {

    }

    public void DoubleDown(PlayerHand hand)
    {
        if (hand.BetAmount > _chips)
        {
            Debug.Log("Not enough chips to double down.");
            return;
        }

        _chips -= hand.BetAmount;

        hand.Bet(hand.BetAmount * 2);
    }

    public PlayerHand AppendHand()
    {
        PlayerHand hand = new PlayerHand();
        _hands.Add(hand);
        return hand;
    }

    public PlayerHand InsertHand(int index)
    {
        PlayerHand hand = new PlayerHand();
        _hands.Insert(index, hand);
        return hand;
    }

    public PlayerHand InsertHand(int index, string handId)
    {
        PlayerHand hand = new PlayerHand();
        hand.SetHandId(handId);
        _hands.Insert(index, hand);
        return hand;
    }

    public void SetIsFinishedTurn()
    {
        _isFinishedTurn = true;
    }

    public void UnsetIsFinishedTurn()
    {
        _isFinishedTurn = false;
    }

    public void SetIsFinishedBetting()
    {
        _isFinishedBetting = true;
    }

    public void UnsetIsFinishedBetting()
    {
        _isFinishedBetting = false;
    }
}
