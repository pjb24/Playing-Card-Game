using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUIManager : MonoBehaviour
{
    [Header("Rooms")]
    [SerializeField] private UIDocument _lobbyRooms;

    private VisualElement _section_blackjackRooms;
    private Button _button_newRoom;

    private Dictionary<string, Button> _list_roomButton = new();

    [Header("Player Infos")]
    [SerializeField] private UIDocument _lobbyPlayerInfo;

    private Label _label_userId;
    private Label _label_playerName;
    private Label _label_playerChips;

    [Header("Sign In Info")]
    [SerializeField] private UIDocument _lobbySignInInfo;

    private TextField _text_userId;
    private TextField _text_playerName;
    private Button _button_enter;

    private void Awake()
    {
        GetSignInInfoItems();
        GetRoomsItems();
        GetPlayerInfoItems();

        SetRoomsInvisible();
        SetPlayerInfoInvisible();
    }

    public void SubscribeButtonNewRoomOnClick(Action action)
    {
        _button_newRoom.clicked += action;
    }

    public void UnsubscribeButtonNewRoomOnClick(Action action)
    {
        _button_newRoom.clicked -= action;
    }

    private void GetRoomsItems()
    {
        VisualElement root = _lobbyRooms.rootVisualElement;

        _section_blackjackRooms = root.Q<VisualElement>("Section_BlackjackRooms");
        _button_newRoom = _section_blackjackRooms.Q<Button>("Button_NewRoom");
    }

    private void GetPlayerInfoItems()
    {
        VisualElement root = _lobbyPlayerInfo.rootVisualElement;

        _label_userId = root.Q<Label>("Label_UserId");
        _label_playerName = root.Q<Label>("Label_PlayerName");
        _label_playerChips = root.Q<Label>("Label_PlayerChips");
    }

    private void GetSignInInfoItems()
    {
        VisualElement root = _lobbySignInInfo.rootVisualElement;

        _text_userId = root.Q<TextField>("TextField_ID");
        _text_playerName = root.Q<TextField>("TextField_Name");
        _button_enter = root.Q<Button>("Button_Enter");
    }

    private void SetRoomsInvisible()
    {
        _lobbyRooms.rootVisualElement.visible = false;
    }

    public void SetRoomsVisible()
    {
        _lobbyRooms.rootVisualElement.visible = true;
    }

    private void SetPlayerInfoInvisible()
    {
        _lobbyPlayerInfo.rootVisualElement.visible = false;
    }

    public void SetPlayerInfoVisible()
    {
        _lobbyPlayerInfo.rootVisualElement.visible = true;
    }

    public void AddRoom(string roomName, EventCallback<ClickEvent> evt)
    {
        if (_list_roomButton.ContainsKey(roomName))
        {
            return;
        }

        Button room = new();

        room.text = roomName;
        room.AddToClassList("Button_Room");

        room.RegisterCallback(evt);

        _list_roomButton.TryAdd(roomName, room);

        _section_blackjackRooms.Add(room);
    }

    public void RemoveAllRooms(EventCallback<ClickEvent> evt)
    {
        foreach (var room in _list_roomButton)
        {
            room.Value.UnregisterCallback(evt);
            room.Value.RemoveFromHierarchy();
        }

        _list_roomButton.Clear();
    }

    public void RemoveRoom(string roomName, EventCallback<ClickEvent> evt)
    {
        _list_roomButton[roomName].UnregisterCallback(evt);
        _list_roomButton[roomName].RemoveFromHierarchy();

        _list_roomButton.Remove(roomName);
    }

    public void SetUserId(string userId)
    {
        if (_label_userId != null)
        {
            _label_userId.text = userId;
        }
    }

    public void SetPlayerName(string playerName)
    {
        if (_label_playerName != null)
        {
            _label_playerName.text = playerName;
        }
    }

    public void SetPlayerChips(int playerChips)
    {
        if (_label_playerChips != null)
        {
            _label_playerChips.text = playerChips.ToString("N0");
        }
    }

    public void SubscribeButtonEnterOnClick(Action action)
    {
        _button_enter.clicked += action;
    }

    public void UnsubscribeButtonEnterOnClick(Action action)
    {
        _button_enter.clicked -= action;
    }

    public string GetUserId()
    {
        return _text_userId?.text;
    }

    public string GetUserName()
    {
        return _text_playerName?.text;
    }

    public void SetSignInInvisible()
    {
        _lobbySignInInfo.rootVisualElement.visible = false;
    }
}
