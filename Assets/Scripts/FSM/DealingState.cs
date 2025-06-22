using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealingState : IGameState
{
    public void Enter()
    {
        GameManager.Instance.ChangeState(new PlayerTurnState());
    }

    public void Exit()
    {
    }

    public void Update()
    {
    }
}
