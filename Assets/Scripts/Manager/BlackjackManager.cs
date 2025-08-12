using DG.Tweening;
using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BlackjackManager : BaseSceneManager
    , IOnExistingPlayerListMessageHandler
    , IOnJoinSuccessMessageHandler
    , IOnUserJoinedMessageHandler
    , IOnPlayerRemainChipsMessageHandler
    , IOnGrantRoomMasterMessageHandler
    , IOnAddHandToPlayerMessageHandler
    , IOnTimeToBettingMessageHandler
    , IOnBetPlacedMessageHandler
    , IOnCardDealtMessageHandler
    , IOnDealerCardDealtMessageHandler
    , IOnDealerHiddenCardDealtMessageHandler
    , IOnTimeToActionMessageHandler
    , IOnPlayerBustedMessageHandler
    , IOnActionDoneMessageHandler
    , IOnHandSplitMessageHandler
    , IOnDealerHoleCardRevealedMessageHandler
    , IOnDealerCardDealtCompleteMessageHandler
    , IOnPayoutMessageHandler
    , IOnGameEndMessageHandler
{
    public BlackjackUIManager uiManager;
    public DeckManager deckManager;
    public CharacterManager characterManager;

    public GameObject cardPrefab;
    public Transform deckPosition;
    public Transform dealerHandPosition;
    [SerializeField] private Transform playerHandPositionRoot;
    public Transform PlayerHandPositionRoot => playerHandPositionRoot;
    [SerializeField] private float playerHandPositionSpacing = 3f;

    public float cardOffsetX = 0.2f;
    public float cardOffsetY = 0.01f;

    public ChipFactory chipFactory;

    public bool PlayerJoined { get; set; } = false;

    private int _betAmount = 0;

    private Player _player;
    private PlayerHand _hand;

    private ConcurrentQueue<Card> _queue = new();
    private readonly object _lock = new();
    private volatile bool _isCoroutineRunning = false;

    public override void InitManager()
    {
        Debug.Log("BlackjackManager가 초기화되었습니다.");
    }

    private void Start()
    {
        JoinRoomDTO joinRoomDTO = new JoinRoomDTO();
        joinRoomDTO.userId = GameManager.Instance.UserId;
        joinRoomDTO.roomName = GameManager.Instance.RoomName;
        string joinRoomJson = Newtonsoft.Json.JsonConvert.SerializeObject(joinRoomDTO);
        NetworkManager.Instance.SignalRClient.Execute("JoinRoom", joinRoomJson);
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

        Vector3 targetPosition = GetHandPosition(hand);

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

    private Tween MoveCardToHand(GameObject cardObj, Vector3 handPosition)
    {
        return cardObj.transform.DOMove(handPosition, 1f);
    }

    public Tween InstancingCardToDealer(Card card, Hand hand, bool hidden = false)
    {
        GameObject cardObj = Instantiate(cardPrefab, deckPosition.position, deckPosition.rotation);
        CardView view = cardObj.GetComponent<CardView>();

        view.SetCard(card, hidden);

        hand.cardObjects.Add(cardObj);
        int cardObjIndex = hand.cardObjects.IndexOf(cardObj);

        return MoveCardToHand(cardObj, dealerHandPosition.position + new Vector3(cardObjIndex * cardOffsetX, cardObjIndex * cardOffsetY, 0));
    }

    public void RevealHoleCard()
    {
        foreach (GameObject cardObj in characterManager.dealer.Hand.cardObjects)
        {
            CardView view = cardObj.GetComponent<CardView>();
            view.UpdateVisual(false);
        }
    }

    public Vector3 GetHandPosition(PlayerHand hand)
    {
        int handIndex = characterManager.GetHandIndex(hand);
        int handCount = characterManager.GetHandCount();

        float centerOffset = (handCount - 1) * playerHandPositionSpacing * 0.5f;

        float xPos = handIndex * playerHandPositionSpacing - centerOffset;

        Vector3 handPosition;
        handPosition.x = xPos;
        handPosition.y = playerHandPositionRoot.position.y;
        handPosition.z = playerHandPositionRoot.position.z;

        return handPosition;
    }

    public void OnJoinSuccess(OnJoinSuccessDTO dto)
    {
        Player player = new Player(dto.playerGuid, dto.userName);
        characterManager.AddPlayer(player);

        characterManager.SetClientPlayer(player);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            uiManager.ChangeToBetPanel();

            uiManager.button_Join.visible = false;
        });
    }

    public void OnExistingPlayerList(OnExistingPlayerListDTO dto)
    {
        foreach (var item in dto.players)
        {
            Player player = new Player(item.playerGuid, item.userName);
            characterManager.AddPlayer(player);
        }
    }

    public void OnUserJoined(OnUserJoinedDTO dto)
    {
        characterManager.AddPlayer(new Player(dto.playerGuid, dto.userName));
    }

    public void OnPlayerRemainChips(OnPlayerRemainChipsDTO dto)
    {
        Player player = characterManager.GetPlayerByGuid(dto.playerGuid);

        player.SetPlayerChips(int.Parse(dto.chips));

        foreach (var hand in player.Hands)
        {
            int handIndex = characterManager.GetHandIndex(hand);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                uiManager.PlayerInfoChipSetText((player.Chips).ToString("N0"), handIndex);
            });
        }
    }

    public void OnGrantRoomMaster(OnGrantRoomMasterDTO dto)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            uiManager.button_Join.visible = true;
            uiManager.button_Join.clicked += HandleJoin;
        });
    }

    private void HandleJoin()
    {
        StartGameDTO startGameDTO = new StartGameDTO();
        string startGameJson = Newtonsoft.Json.JsonConvert.SerializeObject(startGameDTO);
        NetworkManager.Instance.SignalRClient.Execute("StartGame", startGameJson);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            uiManager.button_Join.clicked -= HandleJoin;
        });
    }

    public void OnAddHandToPlayer(OnAddHandToPlayerDTO dto)
    {
        Player player = characterManager.GetPlayerByGuid(dto.playerGuid);
        player.AddHand(dto.handId);
    }

    public void OnTimeToBetting(OnTimeToBettingDTO dto)
    {
        _betAmount = 0;

        _player = characterManager.ClientPlayer;

        _hand = _player.GetNextBettingHand();

        EnterTimeToBetting();
    }

    private void EnterTimeToBetting()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            uiManager.ChangeToBetPanel();

            UpdateUI_PlayerInfos();
            UpdateUI_CardValues();

            // Hide join button
            uiManager.button_Join.visible = false;

            // Button subscribe function
            uiManager.button_BetReset.clicked += HandleBetReset;
            uiManager.button_Bet1.clicked += HandleBet1;
            uiManager.button_Bet2.clicked += HandleBet2;
            uiManager.button_Bet3.clicked += HandleBet3;
            uiManager.button_Bet4.clicked += HandleBet4;
            uiManager.button_BetMax.clicked += HandleBetMax;
            uiManager.button_BetConfirm.clicked += HandleBetConfirm;
        });
    }

    private void UpdateUI_PlayerInfos()
    {
        uiManager.RemoveAllPlayerInfos();

        foreach (var player in characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                int handIndex = characterManager.GetHandIndex(hand);
                uiManager.CreatePlayerInfo(handIndex);
                uiManager.PlayerInfoVisible(handIndex);

                uiManager.PlayerInfoBetAmountSetText(hand.BetAmount.ToString("N0"), handIndex);
                uiManager.PlayerInfoNameSetText(player.DisplayName, handIndex);
                uiManager.PlayerInfoChipSetText(player.Chips.ToString("N0"), handIndex);

                Vector3 targetPosition = GetHandPosition(hand);
                uiManager.RequestPlayerInfoPositionUpdate_Register(targetPosition, handIndex);
                uiManager.RequestPlayerInfoPositionUpdate_Y_Register(handIndex);
            }
        }
    }

    private void UpdateUI_CardValues()
    {
        uiManager.RemoveDealerCardValue();
        uiManager.RemoveAllLabelCardValue();
    }

    public void UpdateUI()
    {
        int handIndex = characterManager.GetHandIndex(_hand);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            uiManager.PlayerInfoBetAmountSetText(_betAmount.ToString("N0"), handIndex);
            uiManager.PlayerInfoChipSetText((_player.Chips - _betAmount).ToString("N0"), handIndex);
        });
    }

    public void HandleBetReset()
    {
        _betAmount = 0;
        chipFactory.ResetChips(_hand);
        UpdateUI();
    }

    public void HandleBet1()
    {
        if (_betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (_player.Chips - _betAmount < (int)E_ChipValue.Bet1)
        {
            return;
        }

        if (_betAmount + (int)E_ChipValue.Bet1 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        _betAmount += (int)E_ChipValue.Bet1;

        // Animate chips
        chipFactory.CreateChipType1(_hand);

        Vector3 handPosition = GetHandPosition(_hand);
        chipFactory.UpdateHandChipPosition(_hand, handPosition);

        UpdateUI();
    }

    public void HandleBet2()
    {
        if (_betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (_player.Chips - _betAmount < (int)E_ChipValue.Bet2)
        {
            return;
        }

        if (_betAmount + (int)E_ChipValue.Bet2 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        _betAmount += (int)E_ChipValue.Bet2;

        // Animate chips
        chipFactory.CreateChipType2(_hand);

        Vector3 handPosition = GetHandPosition(_hand);
        chipFactory.UpdateHandChipPosition(_hand, handPosition);

        UpdateUI();
    }

    public void HandleBet3()
    {
        if (_betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (_player.Chips - _betAmount < (int)E_ChipValue.Bet3)
        {
            return;
        }

        if (_betAmount + (int)E_ChipValue.Bet3 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        _betAmount += (int)E_ChipValue.Bet3;

        // Animate chips
        chipFactory.CreateChipType3(_hand);
        
        Vector3 handPosition = GetHandPosition(_hand);
        chipFactory.UpdateHandChipPosition(_hand, handPosition);

        UpdateUI();
    }

    public void HandleBet4()
    {
        if (_betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (_player.Chips - _betAmount < (int)E_ChipValue.Bet4)
        {
            return;
        }

        if (_betAmount + (int)E_ChipValue.Bet4 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        _betAmount += (int)E_ChipValue.Bet4;

        // Animate chips
        chipFactory.CreateChipType4(_hand);

        Vector3 handPosition = GetHandPosition(_hand);
        chipFactory.UpdateHandChipPosition(_hand, handPosition);

        UpdateUI();
    }

    public void HandleBetMax()
    {
        if (_betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        chipFactory.ResetChips(_hand);

        if (_player.Chips < (int)E_ChipValue.BetMax)
        {
            _betAmount = chipFactory.CreateChipsToFitValue(_player.Chips, _hand);
        }
        else
        {
            _betAmount = (int)E_ChipValue.BetMax;
            chipFactory.CreateChipType5(_hand);
        }

        Vector3 handPosition = GetHandPosition(_hand);
        // Animate chips
        chipFactory.UpdateHandChipPosition(_hand, handPosition);

        UpdateUI();
    }

    public void HandleBetConfirm()
    {
        _player.PlaceBet(_hand, _betAmount);

        PlaceBetDTO placeBetDTO = new PlaceBetDTO();
        placeBetDTO.amount = _betAmount;
        placeBetDTO.handId = _hand.Id;
        string placeBetJson = Newtonsoft.Json.JsonConvert.SerializeObject(placeBetDTO);
        NetworkManager.Instance.SignalRClient.Execute("PlaceBet", placeBetJson);

        _hand = _player.GetNextBettingHand();
        _betAmount = 0;

        if (_hand == null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                // Unsubscribe function
                uiManager.button_BetReset.clicked -= HandleBetReset;
                uiManager.button_Bet1.clicked -= HandleBet1;
                uiManager.button_Bet2.clicked -= HandleBet2;
                uiManager.button_Bet3.clicked -= HandleBet3;
                uiManager.button_Bet4.clicked -= HandleBet4;
                uiManager.button_BetMax.clicked -= HandleBetMax;
                uiManager.button_BetConfirm.clicked -= HandleBetConfirm;
            });
        }
    }

    public void OnBetPlaced(OnBetPlacedDTO dto)
    {
        Player player = characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        hand.Bet(dto.betAmount);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            chipFactory.CreateChipsToFitValue(hand.BetAmount, hand);

            // 모든 칩의 위치를 갱신
            foreach (var player in characterManager.Players)
            {
                foreach (var hand in player.Hands)
                {
                    Vector3 handPosition = GetHandPosition(hand);

                    chipFactory.UpdateHandChipPosition(hand, handPosition);
                }
            }

            // UI 업데이트. Player Info, Card Value
            UpdateUIChips();
        });
    }

    private void UpdateUIChips()
    {
        foreach (var player in characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                UpdateUiChips(hand, player);
            }
        }
    }

    private void UpdateUiChips(PlayerHand hand, Player player)
    {
        int handIndex = characterManager.GetHandIndex(hand);
        uiManager.PlayerInfoVisible(handIndex);

        Vector3 targetPosition = GetHandPosition(hand);

        uiManager.PlayerInfoBetAmountSetText(hand.BetAmount.ToString("N0"), handIndex);
        uiManager.PlayerInfoNameSetText(player.DisplayName, handIndex);
        uiManager.PlayerInfoChipSetText(player.Chips.ToString("N0"), handIndex);
    }

    public void OnCardDealt(OnCardDealtDTO dto)
    {
        Player player = characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        Card card = new Card(dto.cardSuit, dto.cardRank);
        hand.AddCard(card);

        if (hand.Cards.Count == 1)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                InstancingCardToPlayer(card, hand);

                int handIndex = characterManager.GetHandIndex(hand);
                uiManager.CreateLabelCardValuePlayer(handIndex);
                uiManager.CardValuePlayerVisible(handIndex);

                Vector3 targetPosition = GetHandPosition(hand);
                uiManager.RequestCardValueUIPositionUpdate_Register(targetPosition, handIndex);
                uiManager.RequestCardValueUIPositionUpdate_Y_Register(handIndex);

                uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
            });
        }
        else
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                InstancingCardToPlayer(card, hand);

                int handIndex = characterManager.GetHandIndex(hand);

                if (hand.IsBlackjack())
                {
                    uiManager.CardValuePlayerSetText("Blackjack", handIndex);
                }
                else
                {
                    uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
                }
            });
        }
    }

    public void OnDealerCardDealt(OnDealerCardDealtDTO dto)
    {
        Dealer dealer = characterManager.dealer;

        if (dealer.Hand.Cards.Count == 0)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Card dealerCard = new Card(dto.cardSuit, dto.cardRank);
                dealer.Hand.AddCard(dealerCard);

                InstancingCardToDealer(dealerCard, dealer.Hand);

                uiManager.CreateLabelCardValueDealer();
                uiManager.RequestUpdateCardValueDealerPosition();
                uiManager.CardValueDealerSetText(dealer.Hand.GetValue().ToString());
            });
        }
        else
        {
            Card dealerCard = new Card(dto.cardSuit, dto.cardRank);

            _queue.Enqueue(dealerCard);

            lock (_lock)
            {
                if (!_isCoroutineRunning)
                {
                    _isCoroutineRunning = true;
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        StartCoroutine(ExecuteCoroutine());
                    });
                }
            }
        }
    }

    private IEnumerator ExecuteCoroutine()
    {
        while (_queue.TryDequeue(out Card dealerCard))
        {
            yield return StartCoroutine(WorkCoroutine(dealerCard));
        }

        _isCoroutineRunning = false;
    }

    private IEnumerator WorkCoroutine(Card dealerCard)
    {
        Tween tween = InstancingCardToDealer(dealerCard, characterManager.dealer.Hand);

        yield return tween.WaitForCompletion();
        characterManager.dealer.Hand.AddCard(dealerCard);
        uiManager.CardValueDealerSetText(characterManager.dealer.Hand.GetValue().ToString());

        yield return null;
    }

    public void OnDealerHiddenCardDealt(OnDealerHiddenCardDealtDTO dto)
    {
        Dealer dealer = characterManager.dealer;

        // Dealer Second Card - Hidden Card
        Card dealerCard = new Card(E_CardSuit.Back, E_CardRank.Back);
        dealer.Hand.AddCard(dealerCard);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            InstancingCardToDealer(dealerCard, dealer.Hand, true);
        });
    }

    public void OnTimeToAction(OnTimeToActionDTO dto)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            uiManager.ChangeToPlayerActionPanel();
        });

        _player = characterManager.ClientPlayer;

        if (_player.Id == dto.playerGuid)
        {
            _hand = _player.GetHandByGuid(dto.handId);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                // Button subscribe function
                uiManager.button_Hit.clicked += HandleHit;
                uiManager.button_Stand.clicked += HandleStand;
                uiManager.button_Split.clicked += HandleSplit;
                uiManager.button_DoubleDown.clicked += HandleDoubleDown;
            });
        }
    }

    private void HandleHit()
    {
        HitDTO hitDTO = new HitDTO();
        hitDTO.handId = _hand.Id;
        string hitJson = Newtonsoft.Json.JsonConvert.SerializeObject(hitDTO);
        NetworkManager.Instance.SignalRClient.Execute("Hit", hitJson);
    }

    private void HandleStand()
    {
        StandDTO standDTO = new StandDTO();
        standDTO.handId = _hand.Id;
        string standJson = Newtonsoft.Json.JsonConvert.SerializeObject(standDTO);
        NetworkManager.Instance.SignalRClient.Execute("Stand", standJson);
    }

    private void HandleSplit()
    {
        // 베팅에 사용한 칩과 동일한 양의 칩이 필요
        if (_player.Chips < _hand.BetAmount)
        {
            return;
        }

        // 핸드에 카드가 2장, 카드의 숫자 또는 문자가 같아야 함
        if (!_hand.CanSplit())
        {
            //return;
        }

        SplitDTO splitDTO = new SplitDTO();
        splitDTO.handId = _hand.Id;
        string splitJson = Newtonsoft.Json.JsonConvert.SerializeObject(splitDTO);
        NetworkManager.Instance.SignalRClient.Execute("Split", splitJson);
    }

    private void HandleDoubleDown()
    {
        // Check player chip
        if (_player.Chips < _hand.BetAmount)
        {
            return;
        }

        if (!_hand.CanDoubleDown())
        {
            return;
        }

        DoubleDownDTO doubleDownDTO = new DoubleDownDTO();
        doubleDownDTO.handId = _hand.Id;
        string doubleDownJson = Newtonsoft.Json.JsonConvert.SerializeObject(doubleDownDTO);
        NetworkManager.Instance.SignalRClient.Execute("DoubleDown", doubleDownJson);
    }

    private void RemoveListeners()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            // Unsubscribe function
            uiManager.button_Hit.clicked -= HandleHit;
            uiManager.button_Stand.clicked -= HandleStand;
            uiManager.button_Split.clicked -= HandleSplit;
            uiManager.button_DoubleDown.clicked -= HandleDoubleDown;
        });
    }

    public void OnPlayerBusted(OnPlayerBustedDTO dto)
    {
        Player player = characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        int handIndex = characterManager.GetHandIndex(hand);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
        });
    }

    public void OnActionDone(OnActionDoneDTO dto)
    {
        RemoveListeners();
    }

    public void OnHandSplit(OnHandSplitDTO dto)
    {
        Player player = characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        // 새로운 핸드를 현재 핸드의 오른편에 추가
        PlayerHand newHand = player.InsertHand(player.Hands.IndexOf(hand) + 1, dto.newHandId);

        // 핸드에 맞는 UI Insert
        int newHandIndex = characterManager.GetHandIndex(hand);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            uiManager.CreateLabelCardValuePlayer(newHandIndex);
            uiManager.CreatePlayerInfo(newHandIndex);

            uiManager.PlayerInfoBetAmountSetText(hand.BetAmount.ToString("N0"), newHandIndex);
            uiManager.PlayerInfoNameSetText(player.DisplayName, newHandIndex);
            uiManager.PlayerInfoChipSetText(player.Chips.ToString("N0"), newHandIndex);

            // 모든 카드 위치 갱신
            UpdateAllPlayerHandPositions();

            Vector3 targetPosition = GetHandPosition(hand);
            uiManager.RequestCardValueUIPositionUpdate_Register(targetPosition, newHandIndex);
            uiManager.RequestPlayerInfoPositionUpdate_Register(targetPosition, newHandIndex);

            uiManager.RequestCardValueUIPositionUpdate_Y_Register(newHandIndex);
            uiManager.RequestPlayerInfoPositionUpdate_Y_Register(newHandIndex);

            UpdateUICardValue();
        });

        // 현재 핸드의 2번째 카드를 새로운 핸드로 나눔
        Card splitCard = hand.Cards[1];
        hand.RemoveCard(splitCard);
        newHand.AddCard(splitCard);

        // 카드 오브젝트 나눔
        GameObject splitCardObj = hand.cardObjects[1];
        hand.cardObjects.Remove(splitCardObj);
        newHand.cardObjects.Add(splitCardObj);
    }

    private void UpdateUICardValue()
    {
        foreach (var player in characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                UpdateUICardValue(hand);
            }
        }
    }

    private void UpdateUICardValue(PlayerHand hand)
    {
        int handIndex = characterManager.GetHandIndex(hand);
        uiManager.CardValuePlayerVisible(handIndex);

        Vector3 targetPosition = GetHandPosition(hand);
        uiManager.RequestCardValueUIPositionUpdate(targetPosition, handIndex);
        uiManager.RequestPlayerInfoPositionUpdate(targetPosition, handIndex);

        uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
    }

    public void OnDealerHoleCardRevealed(OnDealerHoleCardRevealedDTO dto)
    {
        // 딜러의 히든 카드 오픈
        Card hiddenCard = characterManager.dealer.Hand.Cards[1];

        hiddenCard.SetRank(dto.cardRank);
        hiddenCard.SetSuit(dto.cardSuit);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            RevealHoleCard();

            if (characterManager.dealer.Hand.IsBlackjack())
            {
                uiManager.CardValueDealerSetText("Blackjack");
            }
            else
            {
                uiManager.CardValueDealerSetText(characterManager.dealer.Hand.GetValue().ToString());
            }
        });
    }

    public void OnDealerCardDealtComplete(OnDealerCardDealtCompleteDTO dto)
    {
        StartCoroutine(OnDealerCardDealtCompleteCoroutine());
    }

    private IEnumerator OnDealerCardDealtCompleteCoroutine()
    {
        while (_queue.Count != 0)
        {
            yield return null;
        }

        DealerBehaviorDoneDTO dealerBehaviorDoneDTO = new DealerBehaviorDoneDTO();
        string dealerBehaviorDoneJson = Newtonsoft.Json.JsonConvert.SerializeObject(dealerBehaviorDoneDTO);
        NetworkManager.Instance.SignalRClient.Execute("DealerBehaviorDone", dealerBehaviorDoneJson);
    }

    public void OnPayout(OnPayoutDTO dto)
    {

    }

    public void OnGameEnd(OnGameEndDTO dto)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.StartCoroutine(WaitAndExecute(2f));
        });
    }

    private IEnumerator WaitAndExecute(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        foreach (var player in characterManager.Players)
        {
            player.ResetForNextRound_Network();
        }

        characterManager.dealer.ResetHand();

        UpdateUI_PlayerInfos();
        UpdateUI_CardValues();

        uiManager.ChangeToBetPanel();

        uiManager.button_Join.visible = false;

        ReadyToNextRoundDTO readyToNextRoundDTO = new();
        string readyToNextRoundJson = Newtonsoft.Json.JsonConvert.SerializeObject(readyToNextRoundDTO);
        NetworkManager.Instance.SignalRClient.Execute("ReadyToNextRound", readyToNextRoundJson);
    }
}