using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

public class SignalRClient : MonoBehaviour
{
    private HubConnection _connection;

    async void Start()
    {
        await ConnectToSignalR();
    }

    private async Task ConnectToSignalR()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5065/blackjackHub")
            .WithAutomaticReconnect()
            .Build();

        // 수신 이벤트 설정
        _connection.On<string>("ReceiveMessage", (message) =>
        {
            Debug.Log(message);
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

    private async void SendMessageToServer(string message)
    {
        if (_connection != null && _connection.State == HubConnectionState.Connected)
        {
            await _connection.InvokeAsync("SendMessageToServer", message);
        }
    }

    private async void OnApplicationQuit()
    {
        if (_connection != null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
        }
    }
}
