using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : PersistentSingleton<GameManager>
{
    private BaseSceneManager _currentSceneManager;

    private string _userId = "";
    public string UserId => _userId;
    private string _userName = "";
    public string UserName => _userName;
    private string _roomName = "";
    public string RoomName => _roomName;

    private int _chips = 0;
    public int Chips => _chips;

    protected override void Awake()
    {
        base.Awake();

        // 씬이 로드될 때마다 OnSceneLoaded 함수를 호출하도록 이벤트에 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // GameManager가 파괴될 때 이벤트 등록을 해제해야 메모리 누수가 없습니다.
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "InitScene")
        {
            SceneManager.LoadScene("LobbyScene");
        }

        // 새로 로드된 씬에서 SceneManagerBase를 상속받는 컴포넌트를 찾습니다.
        _currentSceneManager = FindAnyObjectByType<BaseSceneManager>();

        if (_currentSceneManager != null)
        {
            // 찾았다면 초기화 함수를 호출해줍니다.
            _currentSceneManager.InitManager();
        }
        else
        {
            Debug.LogWarning("현재 씬에 BaseSceneManager가 없습니다.");
        }
    }

    public void SetUserId(string userId)
    {
        _userId = userId;
    }

    public void SetUserName(string userName)
    {
        _userName = userName;
    }

    public void SetRoomName(string roomName)
    {
        _roomName = roomName;
    }

    public void HandleWelcomeMessage(WelcomeDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IWelcomeMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.Welcome(dto);
        }
        else
        {
            Debug.LogWarning("OnExistingPlayerList 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnJoinLobbySuccessMessage(OnJoinLobbySuccessDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnJoinLobbySuccessMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnJoinLobbySuccess(dto);
        }
        else
        {
            Debug.LogWarning("OnJoinLobbySuccess 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnFullExistRoomListMessage(OnFullExistRoomListDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnFullExistRoomListMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnFullExistRoomList(dto);
        }
        else
        {
            Debug.LogWarning("OnFullExistRoomList 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnChangedRoomListMessage(OnChangedRoomListDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnChangedRoomListDTOMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnChangedRoomList(dto);
        }
        else
        {
            Debug.LogWarning("OnChangedRoomList 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnExistingPlayerListMessage(OnExistingPlayerListDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnExistingPlayerListMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnExistingPlayerList(dto);
        }
        else
        {
            Debug.LogWarning("OnExistingPlayerList 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnAddCardToHandMessage(OnAddCardToHandDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnAddCardToHandMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnAddCardToHand(dto);
        }
        else
        {
            Debug.LogWarning("OnAddCardToHand 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnAddCardToDealerHandMessage(OnAddCardToDealerHandDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnAddCardToDealerHandMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnAddCardToDealerHand(dto);
        }
        else
        {
            Debug.LogWarning("OnAddCardToDealerHand 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnRoomCreateSuccessMessage(OnRoomCreateSuccessDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnRoomCreateSuccessMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnRoomCreateSuccess(dto);
        }
        else
        {
            Debug.LogWarning("OnRoomCreateSuccess 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnJoinSuccessMessage(OnJoinSuccessDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnJoinSuccessMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnJoinSuccess(dto);
        }
        else
        {
            Debug.LogWarning("OnJoinSuccess 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnUserJoinedMessage(OnUserJoinedDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnUserJoinedMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnUserJoined(dto);
        }
        else
        {
            Debug.LogWarning("OnUserJoined 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnPlayerRemainChipsMessage(OnPlayerRemainChipsDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnPlayerRemainChipsMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnPlayerRemainChips(dto);
        }
        else
        {
            Debug.LogWarning("OnPlayerRemainChips 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnGrantRoomMasterMessage(OnGrantRoomMasterDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnGrantRoomMasterMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnGrantRoomMaster(dto);
        }
        else
        {
            Debug.LogWarning("OnGrantRoomMaster 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnAddHandToPlayerMessage(OnAddHandToPlayerDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnAddHandToPlayerMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnAddHandToPlayer(dto);
        }
        else
        {
            Debug.LogWarning("OnAddHandToPlayer 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnTimeToBettingMessage(OnTimeToBettingDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnTimeToBettingMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnTimeToBetting(dto);
        }
        else
        {
            Debug.LogWarning("OnTimeToBetting 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnBetPlacedMessage(OnBetPlacedDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnBetPlacedMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnBetPlaced(dto);
        }
        else
        {
            Debug.LogWarning("OnBetPlaced 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnCardDealtMessage(OnCardDealtDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnCardDealtMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnCardDealt(dto);
        }
        else
        {
            Debug.LogWarning("OnCardDealt 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnDealerCardDealtMessage(OnDealerCardDealtDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnDealerCardDealtMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnDealerCardDealt(dto);
        }
        else
        {
            Debug.LogWarning("OnDealerCardDealt 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnDealerHiddenCardDealtMessage(OnDealerHiddenCardDealtDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnDealerHiddenCardDealtMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnDealerHiddenCardDealt(dto);
        }
        else
        {
            Debug.LogWarning("OnDealerHiddenCardDealt 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnTimeToActionMessage(OnTimeToActionDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnTimeToActionMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnTimeToAction(dto);
        }
        else
        {
            Debug.LogWarning("OnTimeToAction 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnPlayerBustedMessage(OnPlayerBustedDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnPlayerBustedMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnPlayerBusted(dto);
        }
        else
        {
            Debug.LogWarning("OnPlayerBusted 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnActionDoneMessage(OnActionDoneDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnActionDoneMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnActionDone(dto);
        }
        else
        {
            Debug.LogWarning("OnActionDone 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnHandSplitMessage(OnHandSplitDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnHandSplitMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnHandSplit(dto);
        }
        else
        {
            Debug.LogWarning("OnHandSplit 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnDealerHoleCardRevealedMessage(OnDealerHoleCardRevealedDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnDealerHoleCardRevealedMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnDealerHoleCardRevealed(dto);
        }
        else
        {
            Debug.LogWarning("OnDealerHoleCardRevealed 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnDealerCardDealtCompleteMessage(OnDealerCardDealtCompleteDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnDealerCardDealtCompleteMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnDealerCardDealtComplete(dto);
        }
        else
        {
            Debug.LogWarning("OnDealerCardDealtComplete 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnPayoutMessage(OnPayoutDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnPayoutMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnPayout(dto);
        }
        else
        {
            Debug.LogWarning("OnPayout 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleOnGameEndMessage(OnGameEndDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IOnGameEndMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.OnGameEnd(dto);
        }
        else
        {
            Debug.LogWarning("OnGameEnd 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void HandleUserLeftMessage(UserLeftDTO dto)
    {
        // 현재 씬 매니저가 이 메시지 처리 역할을 수행할 수 있는지 확인
        if (_currentSceneManager is IUserLeftMessageHandler handler)
        {
            // 역할을 수행할 수 있다면, 해당 역할의 함수를 호출
            handler.UserLeft(dto);
        }
        else
        {
            Debug.LogWarning("UserLeft 메시지를 받았지만, 현재 씬 매니저는 해당 메시지를 처리할 수 없습니다.");
        }
    }

    public void SetChips(int chips)
    {
        _chips = chips;
    }
}