using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartState : IGameState
{
    public void Enter()
    {
        // Deck Shuffle

        // UI Reset

    }

    public void Exit()
    {
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
}
