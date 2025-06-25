using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartState : IGameState
{
    public void Enter()
    {
        // Deck Shuffle

        // UI Reset
        GameManager.Instance.uiManager.ChangeToBetPanel();

        // Players Bet, Turn Complete, Hand Reset
        GameManager.Instance.characterManager.ResetAllPlayers();

        UpdateUI_PlayerInfos();

        if (GameManager.Instance.PlayerJoined)
        {
            GameManager.Instance.uiManager.button_Join.visible = false;

            GameManager.Instance.ChangeState(new BettingState());

        }
        else
        {
            GameManager.Instance.uiManager.button_Join.visible = true;
            GameManager.Instance.uiManager.button_Join.clicked += HandleJoin;
        }
    }

    public void Exit()
    {
        GameManager.Instance.uiManager.button_Join.clicked -= HandleJoin;
    }

    public void Update()
    {
    }

    private void HandleJoin()
    {
        // Player Add
        GameManager.Instance.characterManager.AddPlayer(new Player("ID_1", "DisplayName_1", 1_000_000));
        UpdateUI_PlayerInfos();

        // Change state to BettingState
        GameManager.Instance.ChangeState(new BettingState());
    }

    private void UpdateUI_PlayerInfos()
    {
        if (GameManager.Instance.characterManager.Players.Count <= 0)
        {
            GameManager.Instance.uiManager.label_BetAmount.visible = false;
            GameManager.Instance.uiManager.label_PlayerName.visible = false;
            GameManager.Instance.uiManager.label_PlayerChip.visible = false;
        }
        else
        {
            GameManager.Instance.uiManager.label_BetAmount.visible = true;
            GameManager.Instance.uiManager.label_PlayerName.visible = true;
            GameManager.Instance.uiManager.label_PlayerChip.visible = true;
        }

        foreach (var player in GameManager.Instance.characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                GameManager.Instance.uiManager.label_BetAmount.text = hand.BetAmount.ToString("N0");
                GameManager.Instance.uiManager.label_PlayerName.text = player.DisplayName;
                GameManager.Instance.uiManager.label_PlayerChip.text = player.Chips.ToString("N0");
            }
        }
    }
}
