using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartState : IGameState
{
    public void Enter()
    {
        // Deck Shuffle
        GameManager.Instance.deckManager.PrepareNextRound();

        // UI Reset
        GameManager.Instance.uiManager.ChangeToBetPanel();

        // Players Bet, Turn Complete, Hand Reset, Chips Reset
        GameManager.Instance.characterManager.ResetAllPlayers();

        GameManager.Instance.characterManager.dealer.ResetHand();

        UpdateUI_PlayerInfos();
        UpdateUI_CardValues();

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
        string id = "ID_1";
        string name = "DisplayName_1";
        int chips = 100_000_000;
        GameManager.Instance.characterManager.AddPlayer(new Player(id, name, chips));
        UpdateUI_PlayerInfos();

        GameManager.Instance.PlayerJoined = true;

        // Change state to BettingState
        GameManager.Instance.ChangeState(new BettingState());
    }

    private void UpdateUI_PlayerInfos()
    {
        GameManager.Instance.uiManager.RemoveAllPlayerInfos();

        foreach (var player in GameManager.Instance.characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);
                GameManager.Instance.uiManager.CreatePlayerInfo(handIndex);
                GameManager.Instance.uiManager.PlayerInfoVisible(handIndex);

                GameManager.Instance.uiManager.PlayerInfoBetAmountSetText(hand.BetAmount.ToString("N0"), handIndex);
                GameManager.Instance.uiManager.PlayerInfoNameSetText(player.DisplayName, handIndex);
                GameManager.Instance.uiManager.PlayerInfoChipSetText(player.Chips.ToString("N0"), handIndex);

                Vector3 targetPosition = GameManager.Instance.GetHandPosition(hand);
                GameManager.Instance.uiManager.RequestPlayerInfoPositionUpdate_Register(targetPosition, handIndex);
                GameManager.Instance.uiManager.RequestPlayerInfoPositionUpdate_Y_Register(handIndex);
            }
        }
    }

    private void UpdateUI_CardValues()
    {
        GameManager.Instance.uiManager.RemoveDealerCardValue();
        GameManager.Instance.uiManager.RemoveAllLabelCardValue();
    }
}
