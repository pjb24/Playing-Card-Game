using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Linq;

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

            JoinGameDTO joinGameDTO = new JoinGameDTO();
            joinGameDTO.userName = "DisplayName_1";
            string joinGameJson = Newtonsoft.Json.JsonConvert.SerializeObject(joinGameDTO);
            GameManager.Instance.SignalRClient.Execute("JoinGame", joinGameJson);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SignalR 연결 실패: " + ex.Message);
        }
    }

    private async void SendMessageToServer(string message)
    {
        if (_connection != null && _connection.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("SendMessageToServer", message);
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
        _dispatcher.RegisterCommand("Welcome", new WelcomeCommand());
        _dispatcher.RegisterCommand("UserConnected", new UserConnectedCommand());
        _dispatcher.RegisterCommand("UserDisconnected", new UserDisconnectedCommand());
        _dispatcher.RegisterCommand("OnError", new OnErrorCommand());
        _dispatcher.RegisterCommand("OnJoinSuccess", new OnJoinSuccessCommand());
        _dispatcher.RegisterCommand("OnUserJoined", new OnUserJoinedCommand());
        _dispatcher.RegisterCommand("OnPlayerRemainChips", new OnPlayerRemainChipsCommand());
        _dispatcher.RegisterCommand("OnGameStateChanged", new OnGameStateChangedCommand());
        _dispatcher.RegisterCommand("OnBetPlaced", new OnBetPlacedCommand());
        _dispatcher.RegisterCommand("UserLeft", new UserLeftCommand());
        _dispatcher.RegisterCommand("OnTimeToBetting", new OnTimeToBettingCommand());
        _dispatcher.RegisterCommand("OnPayout", new OnPayoutCommand());
        _dispatcher.RegisterCommand("OnCardDealt", new OnCardDealtCommand());
        _dispatcher.RegisterCommand("OnPlayerBusted", new OnPlayerBustedCommand());
        _dispatcher.RegisterCommand("OnActionDone", new OnActionDoneCommand());
        _dispatcher.RegisterCommand("OnHandSplit", new OnHandSplitCommand());
        _dispatcher.RegisterCommand("OnDealerHoleCardRevealed", new OnDealerHoleCardRevealedCommand());
        _dispatcher.RegisterCommand("OnDealerCardDealt", new OnDealerCardDealtCommand());
        _dispatcher.RegisterCommand("OnDealerHiddenCardDealt", new OnDealerHiddenCardDealtCommand());
        _dispatcher.RegisterCommand("OnAddHandToPlayer", new OnAddHandToPlayerCommand());
        _dispatcher.RegisterCommand("OnGameEnd", new OnGameEndCommand());

        OnTimeToActionCommand onTimeToActionCommand = new();
        _dispatcher.RegisterCommand("OnTimeToAction", onTimeToActionCommand);
        GameManager.Instance.SetOnTimeToActionCommandInstance(onTimeToActionCommand);
    }

    public async void Execute(string commandName, string payload)
    {
        await _connection.InvokeAsync("ExecuteCommand", commandName, payload);
    }
}
