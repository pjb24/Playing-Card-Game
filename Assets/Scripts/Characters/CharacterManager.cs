using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager
{
    public List<Player> Players { get; private set; } = new();
    private int currentPlayerIndex = 0;
    public Dealer dealer = new();

    private Player _clientPlayer;
    public Player ClientPlayer => _clientPlayer;

    public void SetClientPlayer(Player player)
    {
        _clientPlayer = player;
    }

    public Player GetPlayerByGuid(string guid)
    {
        return Players.Find(p => p.Id == guid);
    }

    public PlayerHand GetHandFromIndex(int index)
    {
        PlayerHand hand = null;

        int tempIndex = 0;

        foreach (Player p in Players)
        {
            if (p.Hands.Count + tempIndex < index)
            {
                tempIndex += p.Hands.Count;
                continue;
            }

            foreach (PlayerHand h in p.Hands)
            {
                if (tempIndex == index)
                {
                    hand = h;
                    break;
                }

                tempIndex++;
            }

            break;
        }

        return hand;
    }

    public int GetHandIndex(PlayerHand hand)
    {
        int index = 0;

        foreach (Player p in Players)
        {
            int result = p.Hands.FindIndex((PlayerHand x) => { return x == hand; });
            if (result == -1)
            {
                index += p.Hands.Count;
            }
            else
            {
                index += result;
                break;
            }
        }

        return index;
    }

    public int GetHandCount()
    {
        int count = 0;
        foreach (Player p in Players)
        {
            count += p.Hands.Count;
        }

        return count;
    }

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
        currentPlayerIndex = 0;
    }

    public Player GetNextBettingPlayer()
    {
        for (int i = currentPlayerIndex; i < Players.Count; i++)
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
        for (int i = currentPlayerIndex; i < Players.Count; i++)
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
        currentPlayerIndex = 0;
    }
}
