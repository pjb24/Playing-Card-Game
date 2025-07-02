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
    public Transform dealerHandPosition;
    [SerializeField] private Transform playerHandPositionRoot;
    [SerializeField] private float playerHandPositionSpacing = 3f;

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

    public void UpdateAllPlayerHandPositions()
    {
        foreach (var player in characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                foreach (var cardObj in hand.cardObjects)
                {
                    MoveCardToPlayer(cardObj, hand);
                }
            }
        }
    }

    public void MoveCardToPlayer(GameObject cardObj, PlayerHand hand)
    {
        int cardObjIndex = hand.cardObjects.IndexOf(cardObj);

        int handIndex = characterManager.GetHandIndex(hand);
        int handCount = characterManager.GetHandCount();

        Vector3 targetPosition;
        targetPosition.x = GetHandPosition(handIndex, handCount, playerHandPositionSpacing);
        targetPosition.y = playerHandPositionRoot.position.y;
        targetPosition.z = playerHandPositionRoot.position.z;

        MoveCardToHand(cardObj, targetPosition + new Vector3(cardObjIndex * cardOffsetX, cardObjIndex * cardOffsetY, 0));
    }

    public void InstancingCardToPlayer(Card card, PlayerHand hand)
    {
        GameObject cardObj = Instantiate(cardPrefab, deckPosition.position, deckPosition.rotation);
        CardView view = cardObj.GetComponent<CardView>();

        view.SetCard(card);

        hand.cardObjects.Add(cardObj);

        MoveCardToPlayer(cardObj, hand);
    }

    private void MoveCardToHand(GameObject cardObj, Vector3 handPosition)
    {
        cardObj.transform.DOMove(handPosition, 1f);
    }

    public void InstancingCardToDealer(Card card, Hand hand, bool hidden = false)
    {
        GameObject cardObj = Instantiate(cardPrefab, deckPosition.position, deckPosition.rotation);
        CardView view = cardObj.GetComponent<CardView>();

        view.SetCard(card, hidden);

        hand.cardObjects.Add(cardObj);
        int cardObjIndex = hand.cardObjects.IndexOf(cardObj);

        MoveCardToHand(cardObj, dealerHandPosition.position + new Vector3(cardObjIndex * cardOffsetX, cardObjIndex * cardOffsetY, 0));
    }

    public void RevealHoleCard()
    {
        foreach (GameObject cardObj in characterManager.dealer.Hand.cardObjects)
        {
            CardView view = cardObj.GetComponent<CardView>();
            view.UpdateVisual(false);
        }
    }

    private float GetHandPosition(int index, int totalHands, float spacing = 3f)
    {
        float centerOffset = (totalHands - 1) * spacing * 0.5f;
        return index * spacing - centerOffset;
    }

    public float GetHandPosition(PlayerHand hand)
    {
        int handIndex = characterManager.GetHandIndex(hand);
        int handCount = characterManager.GetHandCount();

        float centerOffset = (handCount - 1) * playerHandPositionSpacing * 0.5f;
        return handIndex * playerHandPositionSpacing - centerOffset;
    }
}
