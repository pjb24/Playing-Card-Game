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
{
    [SerializeField] private LobbyUIManager _uiManager;

    private List<RoomInfo> _rooms = new();

    public override void InitManager()
    {
        Debug.Log("LobbyManager가 초기화되었습니다.");
    }

    public void Welcome(WelcomeDTO dto)
    {
        Debug.Log("LobbyManager, Welcome");

        _uiManager.SubscribeButtonEnterOnClick(ButtonEnterOnClick);
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
    }

    public void OnFullExistRoomList(OnFullExistRoomListDTO dto)
    {
        _rooms.Clear();

        _uiManager.RemoveAllRooms(RoomButtonOnClick);

        foreach (var room in dto.rooms)
        {
            RoomInfo roomInfo = new RoomInfo();
            roomInfo.roomId = room.roomName;
            _rooms.Add(roomInfo);

            _uiManager.AddRoom(roomInfo.roomId, RoomButtonOnClick);
        }
    }

    private void RoomButtonOnClick(ClickEvent evt)
    {
        Button clickedButton = evt.target as Button;

        if (clickedButton != null)
        {
            GameManager.Instance.SetRoomName(clickedButton.text);

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

        // 씬 전환
        SceneManager.LoadScene("BlackjackScene");
    }
}
