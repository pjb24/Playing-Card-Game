using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

public class RandomStringGenerator
{
    private static System.Random _random = new System.Random();

    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateRandomString(int length)
    {
        StringBuilder result = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            result.Append(Chars[_random.Next(Chars.Length)]);
        }

        return result.ToString();
    }
}

public class SignalRClient
{
    private HubConnection _connection;
    private CommandDispatcher _dispatcher;

    public async void Start()
    {
        _dispatcher = new CommandDispatcher();
        RegisterCommands();

        await ConnectToSignalR();
    }

    private async Task ConnectToSignalR()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5065/blackjackHub")
            .WithAutomaticReconnect()
            .Build();

        // 수신 이벤트 설정
        _connection.On<string, string>("ReceiveCommand", (commandName, payload) =>
        {
            Debug.Log($"Received command: {commandName}, payload: {payload}");
            _dispatcher.Dispatch(commandName, payload);
        });

        try
        {
            await _connection.StartAsync();
            Debug.Log("SignalR 연결 성공");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SignalR 연결 실패: " + ex.Message);
        }
    }

    public async void OnApplicationQuit()
    {
        if (_connection != null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
        }
    }

    private void RegisterCommands()
    {
        _dispatcher.RegisterCommand("OnActionDone", new OnActionDoneCommand());
        _dispatcher.RegisterCommand("OnAddHandToPlayer", new OnAddHandToPlayerCommand());
        _dispatcher.RegisterCommand("OnBetPlaced", new OnBetPlacedCommand());
        _dispatcher.RegisterCommand("OnCardDealt", new OnCardDealtCommand());
        _dispatcher.RegisterCommand("OnDealerCardDealt", new OnDealerCardDealtCommand());
        _dispatcher.RegisterCommand("OnDealerCardDealtComplete", new OnDealerCardDealtCompleteCommand());
        _dispatcher.RegisterCommand("OnDealerHiddenCardDealt", new OnDealerHiddenCardDealtCommand());
        _dispatcher.RegisterCommand("OnDealerHoleCardRevealed", new OnDealerHoleCardRevealedCommand());
        _dispatcher.RegisterCommand("OnError", new OnErrorCommand());
        _dispatcher.RegisterCommand("OnExistingPlayerList", new OnExistingPlayerListCommand());
        _dispatcher.RegisterCommand("OnFullExistRoomList", new OnFullExistRoomListCommand());
        _dispatcher.RegisterCommand("OnGameEnd", new OnGameEndCommand());
        _dispatcher.RegisterCommand("OnGrantRoomMaster", new OnGrantRoomMasterCommand());
        _dispatcher.RegisterCommand("OnHandSplit", new OnHandSplitCommand());
        _dispatcher.RegisterCommand("OnJoinLobbySuccess", new OnJoinLobbySuccessCommand());
        _dispatcher.RegisterCommand("OnJoinSuccess", new OnJoinSuccessCommand());
        _dispatcher.RegisterCommand("OnPayout", new OnPayoutCommand());
        _dispatcher.RegisterCommand("OnPlayerBusted", new OnPlayerBustedCommand());
        _dispatcher.RegisterCommand("OnPlayerRemainChips", new OnPlayerRemainChipsCommand());
        _dispatcher.RegisterCommand("OnRoomCreateSuccess", new OnRoomCreateSuccessCommand());
        _dispatcher.RegisterCommand("OnTimeToAction", new OnTimeToActionCommand());
        _dispatcher.RegisterCommand("OnTimeToBetting", new OnTimeToBettingCommand());
        _dispatcher.RegisterCommand("OnUserJoined", new OnUserJoinedCommand());
        _dispatcher.RegisterCommand("UserConnected", new UserConnectedCommand());
        _dispatcher.RegisterCommand("UserDisconnected", new UserDisconnectedCommand());
        _dispatcher.RegisterCommand("UserLeft", new UserLeftCommand());
        _dispatcher.RegisterCommand("Welcome", new WelcomeCommand());
    }

    public async void Execute(string commandName, string payload)
    {
        Debug.Log($"Execute, {commandName}, {payload}");
        await _connection.InvokeAsync("ExecuteCommand", commandName, payload);
    }
}
