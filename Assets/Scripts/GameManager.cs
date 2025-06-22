using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameStateMachine stateMachine;

    public DeckManager deckManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        stateMachine = new GameStateMachine();
        deckManager = new DeckManager();
    }

    private void Start()
    {
        stateMachine.ChangeState(new GameStartState());
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public void ChangeState(IGameState newState)
    {
        stateMachine.ChangeState(newState);
    }
}
