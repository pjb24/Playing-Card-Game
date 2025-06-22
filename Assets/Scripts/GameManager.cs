using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameStateMachine stateMachine;

    public UIManager uiManager;
    public DeckManager deckManager;

    public GameObject cardPrefab;
    public Transform deckPosition;
    public Transform playerHandPosition;

    public ChipFactory chipFactory;
    
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

    public void InstancingCardToPlayer(Card card)
    {
        GameObject cardObj = Instantiate(cardPrefab, deckPosition.position, deckPosition.rotation);
        CardView view = cardObj.GetComponent<CardView>();

        view.SetCard(card);

        MoveCardToHand(view, playerHandPosition.position);
    }

    private void MoveCardToHand(CardView cardView, Vector3 handPosition)
    {
        cardView.transform.DOMove(handPosition, 1f);
    }
}
