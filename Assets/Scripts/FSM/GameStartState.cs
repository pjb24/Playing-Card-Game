using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartState : IGameState
{
    private GameManager gameManager;

    public GameStartState(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }

    public void Enter()
    {
        throw new System.NotImplementedException();
    }

    public void Exit()
    {
        throw new System.NotImplementedException();
    }

    public void Update()
    {
        throw new System.NotImplementedException();
    }
}
