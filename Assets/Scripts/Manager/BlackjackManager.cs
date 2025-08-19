using DG.Tweening;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    , IUserLeftMessageHandler
    , IOnAddCardToHandMessageHandler
    , IOnAddCardToDealerHandMessageHandler
{
    [SerializeField] private BlackjackUIManager _uiManager;
    private CharacterManager _characterManager = new();

    [SerializeField] private GameObject _cardPrefab;
    [SerializeField] private Transform _deckPosition;
    [SerializeField] private Transform _dealerHandPosition;
    [SerializeField] private Transform _playerHandPositionRoot;
    [SerializeField] private float _playerHandPositionSpacing = 3f;

    [SerializeField] private float _cardOffsetX = 0.2f;
    [SerializeField] private float _cardOffsetY = 0.01f;

    [SerializeField] private ChipFactory _chipFactory;

    private int _betAmount = 0;

    private Player _player;
    private PlayerHand _hand;

    private ConcurrentQueue<Card> _queue = new();
    private readonly object _lock = new();
    private volatile bool _isCoroutineRunning = false;

    private bool _flagLeaveBooked = false;

    private bool _flagPlayed = false;

    public override void InitManager()
    {
        Debug.Log("BlackjackManager가 초기화되었습니다.");
    }

    private void Start()
    {
        JoinRoomDTO joinRoomDTO = new();
        joinRoomDTO.userId = GameManager.Instance.UserId;
        joinRoomDTO.roomName = GameManager.Instance.RoomName;
        string joinRoomJson = Newtonsoft.Json.JsonConvert.SerializeObject(joinRoomDTO);
        NetworkManager.Instance.SignalRClient.Execute("JoinRoom", joinRoomJson);

        _uiManager.SubscribeButtonLeaveRoomClicked(HandleLeaveButtonClicked);
    }

    public void UpdateAllPlayerHandPositions()
    {
        foreach (var player in _characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                foreach (var cardObj in hand.CardObjects)
                {
                    MoveCardToPlayer(cardObj, hand);
                }
            }
        }
    }

    public void MoveCardToPlayer(GameObject cardObj, PlayerHand hand)
    {
        int cardObjIndex = hand.CardObjects.IndexOf(cardObj);

        Vector3 targetPosition = GetHandPosition(hand);

        MoveCardToHand(cardObj, targetPosition + new Vector3(cardObjIndex * _cardOffsetX, cardObjIndex * _cardOffsetY, 0));
    }

    public void InstancingCardToPlayer(Card card, PlayerHand hand)
    {
        GameObject cardObj = Instantiate(_cardPrefab, _deckPosition.position, _deckPosition.rotation);
        CardView view = cardObj.GetComponent<CardView>();

        view.SetCard(card);

        hand.AddCardObject(cardObj);

        MoveCardToPlayer(cardObj, hand);
    }

    private Tween MoveCardToHand(GameObject cardObj, Vector3 handPosition)
    {
        return cardObj.transform.DOMove(handPosition, 1f);
    }

    public Tween InstancingCardToDealer(Card card, Hand hand, bool hidden = false)
    {
        GameObject cardObj = Instantiate(_cardPrefab, _deckPosition.position, _deckPosition.rotation);
        CardView view = cardObj.GetComponent<CardView>();

        view.SetCard(card, hidden);

        hand.AddCardObject(cardObj);
        int cardObjIndex = hand.CardObjects.IndexOf(cardObj);

        return MoveCardToHand(cardObj, _dealerHandPosition.position + new Vector3(cardObjIndex * _cardOffsetX, cardObjIndex * _cardOffsetY, 0));
    }

    public void RevealHoleCard()
    {
        foreach (GameObject cardObj in _characterManager.Dealer.Hand.CardObjects)
        {
            CardView view = cardObj.GetComponent<CardView>();
            view.UpdateVisual(false);
        }
    }

    public Vector3 GetHandPosition(PlayerHand hand)
    {
        int handIndex = _characterManager.GetHandIndex(hand);
        int handCount = _characterManager.GetHandCount();

        float centerOffset = (handCount - 1) * _playerHandPositionSpacing * 0.5f;

        float xPos = handIndex * _playerHandPositionSpacing - centerOffset;

        Vector3 handPosition;
        handPosition.x = xPos;
        handPosition.y = _playerHandPositionRoot.position.y;
        handPosition.z = _playerHandPositionRoot.position.z;

        return handPosition;
    }

    public void OnJoinSuccess(OnJoinSuccessDTO dto)
    {
        Player player = new(dto.playerGuid, dto.userName);
        _characterManager.AddPlayer(player);

        _characterManager.SetClientPlayer(player);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _uiManager.ChangeToBetPanel();

            _uiManager.SetButtonJoinInvisible();
        });
    }

    public void OnExistingPlayerList(OnExistingPlayerListDTO dto)
    {
        foreach (var item in dto.players)
        {
            Player player = new(item.playerGuid, item.userName);
            _characterManager.AddPlayer(player);
        }
    }

    public void OnUserJoined(OnUserJoinedDTO dto)
    {
        _characterManager.AddPlayer(new Player(dto.playerGuid, dto.userName));

        UpdateUI_PlayerInfos();
    }

    public void OnPlayerRemainChips(OnPlayerRemainChipsDTO dto)
    {
        Player player = _characterManager.GetPlayerByGuid(dto.playerGuid);

        player.SetPlayerChips(dto.chips);

        foreach (var hand in player.Hands)
        {
            int handIndex = _characterManager.GetHandIndex(hand);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                _uiManager.PlayerInfoChipSetText((player.Chips).ToString("N0"), handIndex);

                Vector3 targetPosition = GetHandPosition(hand);
                _uiManager.RequestPlayerInfoPositionUpdate(targetPosition, handIndex);
            });
        }

        if (_characterManager.ClientPlayer.Id == player.Id)
        {
            GameManager.Instance.SetChips(dto.chips);
        }
    }

    public void OnGrantRoomMaster(OnGrantRoomMasterDTO dto)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _uiManager.SetButtonJoinVisible();
            _uiManager.SubscribeButtonJoinClicked(HandleJoin);
        });
    }

    private void HandleJoin()
    {
        StartGameDTO startGameDTO = new();
        string startGameJson = Newtonsoft.Json.JsonConvert.SerializeObject(startGameDTO);
        NetworkManager.Instance.SignalRClient.Execute("StartGame", startGameJson);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _uiManager.UnsubscribeButtonJoinClicked(HandleJoin);
        });
    }

    public void OnAddHandToPlayer(OnAddHandToPlayerDTO dto)
    {
        Player player = _characterManager.GetPlayerByGuid(dto.playerGuid);
        player.AddHand(dto.handId);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            UpdateUI_PlayerInfos();
        });
    }

    public void OnTimeToBetting(OnTimeToBettingDTO dto)
    {
        _flagPlayed = true;

        _betAmount = 0;

        _player = _characterManager.ClientPlayer;

        _hand = _player.GetNextBettingHand();

        EnterTimeToBetting();
    }

    private void EnterTimeToBetting()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _uiManager.ChangeToBetPanel();

            UpdateUI_PlayerInfos();
            UpdateUI_CardValues();

            // Hide join button
            _uiManager.SetButtonJoinInvisible();

            // Button subscribe function
            
            _uiManager.SubscribeButtonBetResetClicked(HandleBetReset);
            _uiManager.SubscribeButtonBet1Clicked(HandleBet1);
            _uiManager.SubscribeButtonBet2Clicked(HandleBet2);
            _uiManager.SubscribeButtonBet3Clicked(HandleBet3);
            _uiManager.SubscribeButtonBet4Clicked(HandleBet4);
            _uiManager.SubscribeButtonBetMaxClicked(HandleBetMax);
            _uiManager.SubscribeButtonBetConfirmClicked(HandleBetConfirm);
        });
    }

    private void UpdateUI_PlayerInfos()
    {
        _uiManager.RemoveAllPlayerInfos();

        foreach (var player in _characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                int handIndex = _characterManager.GetHandIndex(hand);
                _uiManager.CreatePlayerInfo(handIndex);
                _uiManager.PlayerInfoVisible(handIndex);

                _uiManager.PlayerInfoBetAmountSetText(hand.BetAmount.ToString("N0"), handIndex);
                _uiManager.PlayerInfoNameSetText(player.DisplayName, handIndex);
                _uiManager.PlayerInfoChipSetText(player.Chips.ToString("N0"), handIndex);

                Vector3 targetPosition = GetHandPosition(hand);
                _uiManager.RequestPlayerInfoPositionUpdate_Register(targetPosition, handIndex);
                _uiManager.RequestPlayerInfoPositionUpdate_Y_Register(handIndex);
            }
        }
    }

    private void UpdateUI_CardValues()
    {
        _uiManager.RemoveDealerCardValue();
        _uiManager.RemoveAllLabelCardValue();
    }

    public void UpdateUI()
    {
        int handIndex = _characterManager.GetHandIndex(_hand);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _uiManager.PlayerInfoBetAmountSetText(_betAmount.ToString("N0"), handIndex);
            _uiManager.PlayerInfoChipSetText((_player.Chips - _betAmount).ToString("N0"), handIndex);

            Vector3 targetPosition = GetHandPosition(_hand);
            _uiManager.RequestPlayerInfoPositionUpdate(targetPosition, handIndex);
        });
    }

    public void HandleBetReset()
    {
        _betAmount = 0;
        _chipFactory.ResetChips(_hand);
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
        _chipFactory.CreateChipType1(_hand);

        Vector3 handPosition = GetHandPosition(_hand);
        _chipFactory.UpdateHandChipPosition(_hand, handPosition);

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
        _chipFactory.CreateChipType2(_hand);

        Vector3 handPosition = GetHandPosition(_hand);
        _chipFactory.UpdateHandChipPosition(_hand, handPosition);

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
        _chipFactory.CreateChipType3(_hand);
        
        Vector3 handPosition = GetHandPosition(_hand);
        _chipFactory.UpdateHandChipPosition(_hand, handPosition);

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
        _chipFactory.CreateChipType4(_hand);

        Vector3 handPosition = GetHandPosition(_hand);
        _chipFactory.UpdateHandChipPosition(_hand, handPosition);

        UpdateUI();
    }

    public void HandleBetMax()
    {
        if (_betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        _chipFactory.ResetChips(_hand);

        if (_player.Chips < (int)E_ChipValue.BetMax)
        {
            _betAmount = _chipFactory.CreateChipsToFitValue(_player.Chips, _hand);
        }
        else
        {
            _betAmount = (int)E_ChipValue.BetMax;
            _chipFactory.CreateChipType5(_hand);
        }

        Vector3 handPosition = GetHandPosition(_hand);
        // Animate chips
        _chipFactory.UpdateHandChipPosition(_hand, handPosition);

        UpdateUI();
    }

    public void HandleBetConfirm()
    {
        _player.PlaceBet(_hand, _betAmount);

        PlaceBetDTO placeBetDTO = new();
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
                _uiManager.UnsubscribeButtonBetResetClicked(HandleBetReset);
                _uiManager.UnsubscribeButtonBet1Clicked(HandleBet1);
                _uiManager.UnsubscribeButtonBet2Clicked(HandleBet2);
                _uiManager.UnsubscribeButtonBet3Clicked(HandleBet3);
                _uiManager.UnsubscribeButtonBet4Clicked(HandleBet4);
                _uiManager.UnsubscribeButtonBetMaxClicked(HandleBetMax);
                _uiManager.UnsubscribeButtonBetConfirmClicked(HandleBetConfirm);
            });
        }
    }

    public void OnBetPlaced(OnBetPlacedDTO dto)
    {
        Player player = _characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        hand.Bet(dto.betAmount);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _chipFactory.CreateChipsToFitValue(hand.BetAmount, hand);

            // 모든 칩의 위치를 갱신
            foreach (var player in _characterManager.Players)
            {
                foreach (var hand in player.Hands)
                {
                    Vector3 handPosition = GetHandPosition(hand);

                    _chipFactory.UpdateHandChipPosition(hand, handPosition);
                }
            }

            // UI 업데이트. Player Info, Card Value
            UpdateUIChips();
        });
    }

    private void UpdateUIChips()
    {
        foreach (var player in _characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                UpdateUiChips(hand, player);
            }
        }
    }

    private void UpdateUiChips(PlayerHand hand, Player player)
    {
        int handIndex = _characterManager.GetHandIndex(hand);
        _uiManager.PlayerInfoVisible(handIndex);

        Vector3 targetPosition = GetHandPosition(hand);

        _uiManager.PlayerInfoBetAmountSetText(hand.BetAmount.ToString("N0"), handIndex);
        _uiManager.PlayerInfoNameSetText(player.DisplayName, handIndex);
        _uiManager.PlayerInfoChipSetText(player.Chips.ToString("N0"), handIndex);

        _uiManager.RequestPlayerInfoPositionUpdate(targetPosition, handIndex);
    }

    public void OnCardDealt(OnCardDealtDTO dto)
    {
        Player player = _characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        Card card = new(dto.cardSuit, dto.cardRank);
        hand.AddCard(card);

        if (hand.Cards.Count == 1)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                InstancingCardToPlayer(card, hand);

                int handIndex = _characterManager.GetHandIndex(hand);
                _uiManager.CreateLabelCardValuePlayer(handIndex);
                _uiManager.CardValuePlayerVisible(handIndex);

                Vector3 targetPosition = GetHandPosition(hand);
                _uiManager.RequestCardValueUIPositionUpdate_Register(targetPosition, handIndex);
                _uiManager.RequestCardValueUIPositionUpdate_Y_Register(handIndex);

                _uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
            });
        }
        else
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                InstancingCardToPlayer(card, hand);

                int handIndex = _characterManager.GetHandIndex(hand);

                if (hand.IsBlackjack())
                {
                    _uiManager.CardValuePlayerSetText("Blackjack", handIndex);
                }
                else
                {
                    _uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
                }
            });
        }
    }

    public void OnDealerCardDealt(OnDealerCardDealtDTO dto)
    {
        Dealer dealer = _characterManager.Dealer;

        if (dealer.Hand.Cards.Count == 0)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Card dealerCard = new(dto.cardSuit, dto.cardRank);
                dealer.Hand.AddCard(dealerCard);

                InstancingCardToDealer(dealerCard, dealer.Hand);

                _uiManager.CreateLabelCardValueDealer();
                _uiManager.RequestUpdateCardValueDealerPosition();
                _uiManager.CardValueDealerSetText(dealer.Hand.GetValue().ToString());
            });
        }
        else
        {
            Card dealerCard = new(dto.cardSuit, dto.cardRank);

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
        Tween tween = InstancingCardToDealer(dealerCard, _characterManager.Dealer.Hand);

        yield return tween.WaitForCompletion();
        _characterManager.Dealer.Hand.AddCard(dealerCard);
        _uiManager.CardValueDealerSetText(_characterManager.Dealer.Hand.GetValue().ToString());

        yield return null;
    }

    public void OnDealerHiddenCardDealt(OnDealerHiddenCardDealtDTO dto)
    {
        Dealer dealer = _characterManager.Dealer;

        // Dealer Second Card - Hidden Card
        Card dealerCard = new(E_CardSuit.Back, E_CardRank.Back);
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
            _uiManager.ChangeToPlayerActionPanel();
        });

        _player = _characterManager.ClientPlayer;

        Player player = _characterManager.GetPlayerByGuid(dto.playerGuid);
        PlayerHand hand = player.GetHandByGuid(dto.handId);
        int index = _characterManager.GetHandIndex(hand);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _uiManager.SetPlayerInfoHighlight(index);
        });

        if (_player.Id == dto.playerGuid)
        {
            _hand = _player.GetHandByGuid(dto.handId);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                // Button subscribe function
                _uiManager.SubscribeButtonHitClicked(HandleHit);
                _uiManager.SubscribeButtonStandClicked(HandleStand);
                _uiManager.SubscribeButtonSplitClicked(HandleSplit);
                _uiManager.SubscribeButtonDoubleDownClicked(HandleDoubleDown);
            });
        }
    }

    private void HandleHit()
    {
        HitDTO hitDTO = new();
        hitDTO.handId = _hand.Id;
        string hitJson = Newtonsoft.Json.JsonConvert.SerializeObject(hitDTO);
        NetworkManager.Instance.SignalRClient.Execute("Hit", hitJson);
    }

    private void HandleStand()
    {
        StandDTO standDTO = new();
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

        SplitDTO splitDTO = new();
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

        DoubleDownDTO doubleDownDTO = new();
        doubleDownDTO.handId = _hand.Id;
        string doubleDownJson = Newtonsoft.Json.JsonConvert.SerializeObject(doubleDownDTO);
        NetworkManager.Instance.SignalRClient.Execute("DoubleDown", doubleDownJson);
    }

    private void RemoveListeners()
    {
        if (!_flagPlayed)
        {
            return;
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            // Unsubscribe function
            _uiManager.UnsubscribeButtonHitClicked(HandleHit);
            _uiManager.UnsubscribeButtonStandClicked(HandleStand);
            _uiManager.UnsubscribeButtonSplitClicked(HandleSplit);
            _uiManager.UnsubscribeButtonDoubleDownClicked(HandleDoubleDown);
        });
    }

    public void OnPlayerBusted(OnPlayerBustedDTO dto)
    {
        Player player = _characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        int handIndex = _characterManager.GetHandIndex(hand);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
        });
    }

    public void OnActionDone(OnActionDoneDTO dto)
    {
        Player player = _characterManager.GetPlayerByGuid(dto.playerGuid);
        PlayerHand hand = player.GetHandByGuid(dto.handId);
        int index = _characterManager.GetHandIndex(hand);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _uiManager.ResetPlayerInfoHighlight(index);
        });

        RemoveListeners();
    }

    public void OnHandSplit(OnHandSplitDTO dto)
    {
        Player player = _characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        // 새로운 핸드를 현재 핸드의 오른편에 추가
        PlayerHand newHand = player.InsertHand(player.Hands.IndexOf(hand) + 1, dto.newHandId);

        int newHandIndex = _characterManager.GetHandIndex(newHand);

        // 현재 핸드의 2번째 카드를 새로운 핸드로 나눔
        Card splitCard = hand.Cards[1];
        hand.RemoveCard(splitCard);
        newHand.AddCard(splitCard);

        // 카드 오브젝트 나눔
        GameObject splitCardObj = hand.CardObjects[1];
        hand.RemoveCardObject(splitCardObj);
        newHand.AddCardObject(splitCardObj);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            // 핸드에 맞는 UI Insert
            _uiManager.CreateLabelCardValuePlayer(newHandIndex);
            _uiManager.CreatePlayerInfo(newHandIndex);

            _uiManager.PlayerInfoBetAmountSetText(hand.BetAmount.ToString("N0"), newHandIndex);
            _uiManager.PlayerInfoNameSetText(player.DisplayName, newHandIndex);
            _uiManager.PlayerInfoChipSetText(player.Chips.ToString("N0"), newHandIndex);

            // 모든 카드 위치 갱신
            UpdateAllPlayerHandPositions();

            Vector3 targetPosition = GetHandPosition(newHand);
            _uiManager.RequestCardValueUIPositionUpdate_Register(targetPosition, newHandIndex);
            _uiManager.RequestPlayerInfoPositionUpdate_Register(targetPosition, newHandIndex);

            _uiManager.RequestCardValueUIPositionUpdate_Y_Register(newHandIndex);
            _uiManager.RequestPlayerInfoPositionUpdate_Y_Register(newHandIndex);

            UpdateUICardValue();
        });
    }

    private void UpdateUICardValue()
    {
        foreach (var player in _characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                UpdateUICardValue(hand);
            }
        }
    }

    private void UpdateUICardValue(PlayerHand hand)
    {
        int handIndex = _characterManager.GetHandIndex(hand);
        _uiManager.CardValuePlayerVisible(handIndex);

        Vector3 targetPosition = GetHandPosition(hand);
        _uiManager.RequestCardValueUIPositionUpdate(targetPosition, handIndex);
        _uiManager.RequestPlayerInfoPositionUpdate(targetPosition, handIndex);

        _uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
    }

    public void OnDealerHoleCardRevealed(OnDealerHoleCardRevealedDTO dto)
    {
        // 딜러의 히든 카드 오픈
        Card hiddenCard = _characterManager.Dealer.Hand.Cards[1];

        hiddenCard.SetRank(dto.cardRank);
        hiddenCard.SetSuit(dto.cardSuit);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            RevealHoleCard();

            if (_characterManager.Dealer.Hand.IsBlackjack())
            {
                _uiManager.CardValueDealerSetText("Blackjack");
            }
            else
            {
                _uiManager.CardValueDealerSetText(_characterManager.Dealer.Hand.GetValue().ToString());
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

        if (!_flagPlayed)
        {
            yield break;
        }

        DealerBehaviorDoneDTO dealerBehaviorDoneDTO = new();
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

        foreach (var player in _characterManager.Players)
        {
            player.ResetForNextRound_Network();
        }

        _characterManager.Dealer.ResetHand();

        UpdateUI_PlayerInfos();
        UpdateUI_CardValues();

        _uiManager.ChangeToBetPanel();

        _uiManager.SetButtonJoinInvisible();

        if (_flagLeaveBooked)
        {
            LeaveRoom();
        }
        else
        {
            ReadyToNextRoundDTO readyToNextRoundDTO = new();
            string readyToNextRoundJson = Newtonsoft.Json.JsonConvert.SerializeObject(readyToNextRoundDTO);
            NetworkManager.Instance.SignalRClient.Execute("ReadyToNextRound", readyToNextRoundJson);
        }

        _flagPlayed = false;
    }

    public void HandleLeaveButtonClicked()
    {
        if (!_flagPlayed)
        {
            LeaveRoom();
        }

        if (_flagLeaveBooked)
        {
            _flagLeaveBooked = false;
            _uiManager.SetButtonLeaveRoomText("방 나가기");
        }
        else
        {
            _flagLeaveBooked = true;
            _uiManager.SetButtonLeaveRoomText("나가기 예약됨");
        }
    }

    public void UserLeft(UserLeftDTO dto)
    {
        Player player = _characterManager.GetPlayerByGuid(dto.playerGuid);

        player.ClearHands();

        _characterManager.RemovePlayer(player);

        UpdateUI_PlayerInfos();
    }

    private void LeaveRoom()
    {
        _uiManager.UnsubscribeButtonLeaveRoomClicked(HandleLeaveButtonClicked);

        // 방 나가기 메시지 송신
        LeaveGameDTO leaveGameDTO = new();
        leaveGameDTO.roomId = GameManager.Instance.RoomName;
        string leaveGameJson = Newtonsoft.Json.JsonConvert.SerializeObject(leaveGameDTO);
        NetworkManager.Instance.SignalRClient.Execute("LeaveGame", leaveGameJson);

        // 로비로 씬 전환
        SceneManager.LoadScene("LobbyScene");
    }

    public void OnAddCardToHand(OnAddCardToHandDTO dto)
    {
        Player player = _characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        Card card = new(dto.cardSuit, dto.cardRank);
        hand.AddCard(card);

        if (hand.Cards.Count == 1)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                InstancingCardToPlayerDontMove(card, hand);

                int handIndex = _characterManager.GetHandIndex(hand);
                _uiManager.CreateLabelCardValuePlayer(handIndex);
                _uiManager.CardValuePlayerVisible(handIndex);

                Vector3 targetPosition = GetHandPosition(hand);
                _uiManager.RequestCardValueUIPositionUpdate_Register(targetPosition, handIndex);
                _uiManager.RequestCardValueUIPositionUpdate_Y_Register(handIndex);

                _uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
            });
        }
        else
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                InstancingCardToPlayerDontMove(card, hand);

                int handIndex = _characterManager.GetHandIndex(hand);

                if (hand.IsBlackjack())
                {
                    _uiManager.CardValuePlayerSetText("Blackjack", handIndex);
                }
                else
                {
                    _uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
                }
            });
        }
    }

    public void InstancingCardToPlayerDontMove(Card card, PlayerHand hand)
    {
        GameObject cardObj = Instantiate(_cardPrefab, _deckPosition.position, _deckPosition.rotation);
        CardView view = cardObj.GetComponent<CardView>();

        view.SetCard(card);

        hand.AddCardObject(cardObj);

        int cardObjIndex = hand.CardObjects.IndexOf(cardObj);

        Vector3 targetPosition = GetHandPosition(hand);

        cardObj.transform.position = targetPosition + new Vector3(cardObjIndex * _cardOffsetX, cardObjIndex * _cardOffsetY, 0);
    }

    public void OnAddCardToDealerHand(OnAddCardToDealerHandDTO dto)
    {
        Dealer dealer = _characterManager.Dealer;

        if (dealer.Hand.Cards.Count == 0)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Card dealerCard = new(dto.cardSuit, dto.cardRank);
                dealer.Hand.AddCard(dealerCard);

                InstancingCardToDealerDontMove(dealerCard, dealer.Hand);

                _uiManager.CreateLabelCardValueDealer();
                _uiManager.RequestUpdateCardValueDealerPosition();
                _uiManager.CardValueDealerSetText(dealer.Hand.GetValue().ToString());
            });
        }
        else
        {
            Card dealerCard = new(dto.cardSuit, dto.cardRank);
            
            InstancingCardToDealerDontMove(dealerCard, _characterManager.Dealer.Hand);
            
            _characterManager.Dealer.Hand.AddCard(dealerCard);
            _uiManager.CardValueDealerSetText(_characterManager.Dealer.Hand.GetValue().ToString());
        }
    }

    public void InstancingCardToDealerDontMove(Card card, Hand hand, bool hidden = false)
    {
        GameObject cardObj = Instantiate(_cardPrefab, _deckPosition.position, _deckPosition.rotation);
        CardView view = cardObj.GetComponent<CardView>();

        view.SetCard(card, hidden);

        hand.AddCardObject(cardObj);

        int cardObjIndex = hand.CardObjects.IndexOf(cardObj);

        cardObj.transform.position = _dealerHandPosition.position + new Vector3(cardObjIndex * _cardOffsetX, cardObjIndex * _cardOffsetY, 0);
    }
}
