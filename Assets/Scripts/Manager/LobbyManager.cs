using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class RoomInfo
{
    public string roomId { get; set; }
}

public class LobbyManager : BaseSceneManager
    , IWelcomeMessageHandler
    , IOnJoinLobbySuccessMessageHandler
    , IOnFullExistRoomListMessageHandler
    , IOnRoomCreateSuccessMessageHandler
    , IOnChangedRoomListDTOMessageHandler
{
    [SerializeField] private LobbyUIManager _uiManager;

    private Dictionary<string, RoomInfo> _rooms = new();

    private bool _flagRequestRoomChanges = false;
    private bool _flagRequestFullRoomList = false;

    public override void InitManager()
    {
        Debug.Log("LobbyManager가 초기화되었습니다.");

        _uiManager.Init();

        if (GameManager.Instance.UserId == "")
        {
            _uiManager.SubscribeButtonEnterOnClick(ButtonEnterOnClick);
        }
        else
        {
            _uiManager.SetSignInInvisible();

            _uiManager.SetUserId(GameManager.Instance.UserId);
            _uiManager.SetPlayerName(GameManager.Instance.UserName);
            _uiManager.SetPlayerChips(GameManager.Instance.Chips);

            _uiManager.SubscribeButtonNewRoomOnClick(NewRoomButtonOnClick);

            _uiManager.SetRoomsVisible();
            _uiManager.SetPlayerInfoVisible();

            StartRoomRequestRoutines();
        }
    }

    public void Welcome(WelcomeDTO dto)
    {
        Debug.Log("LobbyManager, Welcome");
    }

    public void ButtonEnterOnClick()
    {
        JoinLobbyDTO joinLobbyDTO = new();
        joinLobbyDTO.userId = _uiManager.GetUserId();
        joinLobbyDTO.userName = _uiManager.GetUserName();
        string joinLobbyJson = Newtonsoft.Json.JsonConvert.SerializeObject(joinLobbyDTO);
        NetworkManager.Instance.SignalRClient.Execute("JoinLobby", joinLobbyJson);
    }

    public void OnJoinLobbySuccess(OnJoinLobbySuccessDTO dto)
    {
        _uiManager.UnsubscribeButtonEnterOnClick(ButtonEnterOnClick);

        _uiManager.SetSignInInvisible();

        _uiManager.SetUserId(_uiManager.GetUserId());
        _uiManager.SetPlayerName(_uiManager.GetUserName());
        _uiManager.SetPlayerChips(dto.playerChips);

        _uiManager.SubscribeButtonNewRoomOnClick(NewRoomButtonOnClick);

        _uiManager.SetRoomsVisible();
        _uiManager.SetPlayerInfoVisible();

        GameManager.Instance.SetUserId(_uiManager.GetUserId());
        GameManager.Instance.SetUserName(_uiManager.GetUserName());
        GameManager.Instance.SetChips(dto.playerChips);

        StartRoomRequestRoutines();
    }

    public void OnFullExistRoomList(OnFullExistRoomListDTO dto)
    {
        _rooms.Clear();

        _uiManager.RemoveAllRooms(RoomButtonOnClick);

        foreach (var room in dto.rooms)
        {
            RoomInfo roomInfo = new();
            roomInfo.roomId = room.roomName;
            _rooms.Add(roomInfo.roomId, roomInfo);

            _uiManager.AddRoom(roomInfo.roomId, RoomButtonOnClick);
        }
    }

    private void RoomButtonOnClick(ClickEvent evt)
    {
        Button clickedButton = evt.target as Button;

        if (clickedButton != null)
        {
            GameManager.Instance.SetRoomName(clickedButton.text);

            StopRoomRequestRoutines();

            // 씬 전환
            SceneManager.LoadScene("BlackjackScene");
        }
    }

    private void NewRoomButtonOnClick()
    {
        string randomString = RandomStringGenerator.GenerateRandomString(8);

        // 룸 생성 메시지
        CreateNewRoomDTO createNewRoomDTO = new();
        createNewRoomDTO.roomName = randomString;
        string createNewRoomJson = Newtonsoft.Json.JsonConvert.SerializeObject(createNewRoomDTO);
        NetworkManager.Instance.SignalRClient.Execute("CreateNewRoom", createNewRoomJson);
    }

    public void OnRoomCreateSuccess(OnRoomCreateSuccessDTO dto)
    {
        GameManager.Instance.SetRoomName(dto.roomName);

        StopRoomRequestRoutines();

        // 씬 전환
        SceneManager.LoadScene("BlackjackScene");
    }

    private void StartRoomRequestRoutines()
    {
        _flagRequestRoomChanges = true;
        _flagRequestFullRoomList = true;

        StartCoroutine(RequestRoomChangesRoutine());
        StartCoroutine(RequestFullRoomListRoutine());
    }

    private void StopRoomRequestRoutines()
    {
        _flagRequestRoomChanges = false;
        _flagRequestFullRoomList = false;
    }

    private IEnumerator RequestRoomChangesRoutine()
    {
        while (_flagRequestRoomChanges)
        {
            RequestRoomChangesDTO requestRoomChangesDTO = new();
            List<RoomInfoDTO> listRoomInfoDTO = new();
            foreach (var room in _rooms.Values)
            {
                RoomInfoDTO roomInfoDTO = new();
                roomInfoDTO.roomName = room.roomId;
                listRoomInfoDTO.Add(roomInfoDTO);
            }
            requestRoomChangesDTO.roomList = listRoomInfoDTO;
            string requestRoomChangesJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestRoomChangesDTO);
            NetworkManager.Instance.SignalRClient.Execute("RequestRoomChanges", requestRoomChangesJson);

            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator RequestFullRoomListRoutine()
    {
        while (_flagRequestFullRoomList)
        {
            RequestFullRoomListDTO requestFullRoomListDTO = new();
            string requestFullRoomListJson = Newtonsoft.Json.JsonConvert.SerializeObject(requestFullRoomListDTO);
            NetworkManager.Instance.SignalRClient.Execute("RequestFullRoomList", requestFullRoomListJson);

            yield return new WaitForSeconds(60f);
        }
    }

    public void OnChangedRoomList(OnChangedRoomListDTO dto)
    {
        foreach (var room in dto.roomsRemove)
        {
            _rooms.Remove(room.roomName);

            _uiManager.RemoveRoom(room.roomName, RoomButtonOnClick);
        }

        foreach (var room in dto.roomsAdd)
        {
            if (_rooms.ContainsKey(room.roomName))
            {
                continue;
            }

            RoomInfo roomInfo = new();
            roomInfo.roomId = room.roomName;
            _rooms.TryAdd(roomInfo.roomId, roomInfo);

            _uiManager.AddRoom(roomInfo.roomId, RoomButtonOnClick);
        }
    }
}
