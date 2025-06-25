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


        //GameManager.Instance.uiManager.button_Join.clicked += TestCardInstancingAndMove;
        //GameManager.Instance.uiManager.button_BetReset.clicked += TestResetChips;
        //GameManager.Instance.uiManager.button_Bet1.clicked += TestCreateChipType1;
        //GameManager.Instance.uiManager.button_Bet2.clicked += TestCreateChipType2;
        //GameManager.Instance.uiManager.button_Bet3.clicked += TestCreateChipType3;
        //GameManager.Instance.uiManager.button_Bet4.clicked += TestCreateChipType4;
        //GameManager.Instance.uiManager.button_BetMax.clicked += TestCreateChipType5;
    }

    public void Exit()
    {
        GameManager.Instance.uiManager.button_Join.clicked -= HandleJoin;

        //GameManager.Instance.uiManager.button_Join.clicked -= TestCardInstancingAndMove;
        //GameManager.Instance.uiManager.button_BetReset.clicked -= TestResetChips;
        //GameManager.Instance.uiManager.button_Bet1.clicked -= TestCreateChipType1;
        //GameManager.Instance.uiManager.button_Bet2.clicked -= TestCreateChipType2;
        //GameManager.Instance.uiManager.button_Bet3.clicked -= TestCreateChipType3;
        //GameManager.Instance.uiManager.button_Bet4.clicked -= TestCreateChipType4;
        //GameManager.Instance.uiManager.button_BetMax.clicked -= TestCreateChipType5;
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

    private void TestCardInstancingAndMove()
    {
        GameManager.Instance.InstancingCardToPlayer(GameManager.Instance.deckManager.DrawCard());
    }

    private void TestResetChips()
    {
        GameManager.Instance.chipFactory.ResetChips();
    }

    private void TestCreateChipType1()
    {
        GameManager.Instance.chipFactory.CreateChipType1();
        GameManager.Instance.chipFactory.UpdateChipPosition();
    }

    private void TestCreateChipType2()
    {
        GameManager.Instance.chipFactory.CreateChipType2();
        GameManager.Instance.chipFactory.UpdateChipPosition();
    }

    private void TestCreateChipType3()
    {
        GameManager.Instance.chipFactory.CreateChipType3();
        GameManager.Instance.chipFactory.UpdateChipPosition();
    }

    private void TestCreateChipType4()
    {
        GameManager.Instance.chipFactory.CreateChipType4();
        GameManager.Instance.chipFactory.UpdateChipPosition();
    }

    private void TestCreateChipType5()
    {
        GameManager.Instance.chipFactory.ResetChips();
        GameManager.Instance.chipFactory.CreateChipType5();
        GameManager.Instance.chipFactory.UpdateChipPosition();
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
