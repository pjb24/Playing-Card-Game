using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager
{
    public List<Player> Players { get; private set; } = new();
    private int currentPlayerIndex = -1;
    public Dealer dealer = new();

    public void AddPlayer(Player player)
    {
        if (!Players.Contains(player))
        {
            Players.Add(player);
        }
    }

    public void RemovePlayer(Player player)
    {
        Players.Remove(player);
    }

    public void ResetAllPlayers()
    {
        foreach (var player in Players)
        {
            player.ResetForNextRound();
        }
        currentPlayerIndex = -1;
    }

    public Player GetNextBettingPlayer()
    {
        for (int i = currentPlayerIndex + 1; i < Players.Count; i++)
        {
            if (!Players[i].IsFinishedBetting)
            {
                currentPlayerIndex = i;
                return Players[i];
            }
        }

        return null;
    }

    public Player GetNextActivePlayer()
    {
        for (int i = currentPlayerIndex + 1; i < Players.Count; i++)
        {
            if (!Players[i].IsFinishedTurn)
            {
                currentPlayerIndex = i;
                return Players[i];
            }
        }

        return null;
    }

    public void MarkCurrentPlayerDone()
    {
        Players[currentPlayerIndex].IsFinishedTurn = true;
    }

    public void MarkCurrentPlayerDoneBetting()
    {
        Players[currentPlayerIndex].IsFinishedBetting = true;
    }

    public void ResetTurnOrder()
    {
        currentPlayerIndex = -1;
    }
}
