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
    public CharacterManager characterManager;

    public GameObject cardPrefab;
    public Transform deckPosition;
    public Transform playerHandPosition;
    public Transform dealerHandPosition;

    public float cardOffsetX = 0.2f;
    public float cardOffsetY = 0.01f;

    public ChipFactory chipFactory;

    public bool PlayerJoined { get; set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
        stateMachine = new GameStateMachine();
        deckManager = new DeckManager();
        characterManager = new CharacterManager();

        uiManager.SetPlayerInfos();
        uiManager.SetCardValues();
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

    public void InstancingCardToPlayer(Card card, Hand hand)
    {
        GameObject cardObj = Instantiate(cardPrefab, deckPosition.position, deckPosition.rotation);
        CardView view = cardObj.GetComponent<CardView>();

        view.SetCard(card);

        int cardObjCount = hand.cardObjects.Count;
        hand.cardObjects.Add(cardObj);

        MoveCardToHand(view, playerHandPosition.position + new Vector3(cardObjCount * cardOffsetX, cardObjCount * cardOffsetY, 0));
    }

    private void MoveCardToHand(CardView cardView, Vector3 handPosition)
    {
        cardView.transform.DOMove(handPosition, 1f);
    }

    public void InstancingCardToDealer(Card card, Hand hand, bool hidden = false)
    {
        GameObject cardObj = Instantiate(cardPrefab, deckPosition.position, deckPosition.rotation);
        CardView view = cardObj.GetComponent<CardView>();

        view.SetCard(card, hidden);

        int cardObjCount = hand.cardObjects.Count;
        hand.cardObjects.Add(cardObj);

        MoveCardToHand(view, dealerHandPosition.position + new Vector3(cardObjCount * cardOffsetX, cardObjCount * cardOffsetY, 0));
    }

    public void RevealHoleCard()
    {
        foreach (GameObject cardObj in characterManager.dealer.Hand.cardObjects)
        {
            CardView view = cardObj.GetComponent<CardView>();
            view.UpdateVisual(false);
        }
    }
}
