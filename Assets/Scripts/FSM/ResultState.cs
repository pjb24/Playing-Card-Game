using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultState : IGameState
{
    public void Enter()
    {
        GameManager.Instance.ChangeState(new GameEndState());
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }
}
