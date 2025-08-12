using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager
{
    private List<Player> _players = new();
    public IReadOnlyList<Player> Players => _players;

    private int currentPlayerIndex = 0;

    private Dealer _dealer = new();
    public Dealer Dealer => _dealer;

    private Player _clientPlayer;
    public Player ClientPlayer => _clientPlayer;

    public void SetClientPlayer(Player player)
    {
        _clientPlayer = player;
    }

    public Player GetPlayerByGuid(string guid)
    {
        return _players.Find(p => p.Id == guid);
    }

    public PlayerHand GetHandFromIndex(int index)
    {
        PlayerHand hand = null;

        int tempIndex = 0;

        foreach (Player p in _players)
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

        foreach (Player p in _players)
        {
            int result = p.Hands.FindIndex((PlayerHand h) => { return h == hand; });
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
        foreach (Player p in _players)
        {
            count += p.Hands.Count;
        }

        return count;
    }

    public void AddPlayer(Player player)
    {
        if (!_players.Contains(player))
        {
            _players.Add(player);
        }
    }

    public void RemovePlayer(Player player)
    {
        _players.Remove(player);
    }

    public void ResetAllPlayers()
    {
        foreach (var player in _players)
        {
            player.ResetForNextRound();
        }
        currentPlayerIndex = 0;
    }

    public Player GetNextBettingPlayer()
    {
        for (int i = currentPlayerIndex; i < _players.Count; i++)
        {
            if (!_players[i].IsFinishedBetting)
            {
                currentPlayerIndex = i;
                return _players[i];
            }
        }

        return null;
    }

    public Player GetNextActivePlayer()
    {
        for (int i = currentPlayerIndex; i < _players.Count; i++)
        {
            if (!_players[i].IsFinishedTurn)
            {
                currentPlayerIndex = i;
                return _players[i];
            }
        }

        return null;
    }

    public void MarkCurrentPlayerDone()
    {
        _players[currentPlayerIndex].SetIsFinishedTurn();
    }

    public void MarkCurrentPlayerDoneBetting()
    {
        _players[currentPlayerIndex].SetIsFinishedBetting();
    }

    public void ResetTurnOrder()
    {
        currentPlayerIndex = 0;
    }
}
