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

        GameManager.Instance.uiManager.button_Join.clicked += TestCardInstancingAndMove;
    }

    public void Exit()
    {
        GameManager.Instance.uiManager.button_Join.clicked -= TestCardInstancingAndMove;
    }

    public void Update()
    {
    }

    private void HandleJoin()
    {
        // Player Add

        // Change state to BettingState
        GameManager.Instance.ChangeState(new BettingState());
    }

    private void TestCardInstancingAndMove()
    {
        GameManager.Instance.InstancingCardToPlayer(GameManager.Instance.deckManager.DrawCard());
    }
}
