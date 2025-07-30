using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.Linq;

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

        // ���� �̺�Ʈ ����
        RegisterReceiveMessageEvents();

        try
        {
            await _connection.StartAsync();
            Debug.Log("SignalR ���� ����");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SignalR ���� ����: " + ex.Message);
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

    private void RegisterReceiveMessageEvents()
    {
        RegisterEvent_Welcome();
        RegisterEvent_UserConnected();
        RegisterEvent_UserDisconnected();
        RegisterEvent_OnError();
        RegisterEvent_OnJoinSuccess();
        RegisterEvent_OnUserJoined();
        RegisterEvent_OnPlayerRemainChips();
        RegisterEvent_OnGameStateChanged();
        RegisterEvent_OnBetPlaced();
        RegisterEvent_UserLeft();
        RegisterEvent_OnTimeToBetting();
        RegisterEvent_OnPayout();
        RegisterEvent_OnCardDealt();
        RegisterEvent_OnPlayerBusted();
        RegisterEvent_OnActionDone();
        RegisterEvent_OnHandSplit();
        RegisterEvent_OnDealerHoleCardRevealed();
        RegisterEvent_OnDealerCardDealt();
        RegisterEvent_OnDealerHiddenCardDealt();
        RegisterEvent_OnTimeToAction();
        RegisterEvent_OnHandEvaluation();
    }

    private void RegisterEvent_Welcome()
    {
        _connection.On<DTO_Welcome>("Welcome", (args) =>
        {
            Debug.Log("Welcome, " + args.message);
        });
    }

    private void RegisterEvent_UserConnected()
    {
        _connection.On<DTO_UserConnected>("UserConnected", (args) =>
        {
            Debug.Log("UserConnected, " + "ConnectionId: " + args.message + " ������ ����Ǿ����ϴ�.");
        });
    }

    private void RegisterEvent_UserDisconnected()
    {
        _connection.On<DTO_UserDisconnected>("UserDisconnected", (args) =>
        {
            Debug.Log("UserDisconnected, " + "ConnectionId: " + args.message + " ������ ������ �����Ǿ����ϴ�.");
        });
    }

    private void RegisterEvent_OnError()
    {
        _connection.On<DTO_OnError>("OnError", (args) =>
        {
            Debug.Log("OnError, " + args.message);
        });
    }

    private void RegisterEvent_OnJoinSuccess()
    {
        _connection.On<DTO_OnJoinSuccess>("OnJoinSuccess", (args) =>
        {
            Debug.Log("OnJoinSuccess, " + "���ӿ� �����Ͽ����ϴ�." + "���� �̸�: " + args.userName + " PlayerGuid: " + args.playerGuid);
        });
    }

    private void RegisterEvent_OnUserJoined()
    {
        _connection.On<DTO_OnUserJoined>("OnUserJoined", (args) =>
        {
            Debug.Log("OnUserJoined, " + "������ ���ӿ� �����Ͽ����ϴ�." + " ���� �̸�: " + args.userName);
        });
    }

    private void RegisterEvent_OnPlayerRemainChips()
    {
        _connection.On<DTO_OnPlayerRemainChips>("OnPlayerRemainChips", (args) =>
        {
            Debug.Log("OnPlayerRemainChips, " + "�÷��̾ ������ Ĩ: " + args.chips);
        });
    }

    private void RegisterEvent_OnGameStateChanged()
    {
        _connection.On<DTO_OnGameStateChanged>("OnGameStateChanged", (args) =>
        {
            Debug.Log("OnGameStateChanged, " + "������ State�� " + args.state + "�� ����Ǿ����ϴ�.");
        });
    }

    private void RegisterEvent_OnBetPlaced()
    {
        _connection.On<DTO_OnBetPlaced>("OnBetPlaced", (args) =>
        {
            Debug.Log("OnBetPlaced, " + "�÷��̾� " + args.playerName + "��/�� �ڵ� ID: " + args.handId + "�� " + args.betAmount + "�� �����Ͽ����ϴ�.");
        });
    }

    private void RegisterEvent_UserLeft()
    {
        _connection.On<DTO_UserLeft>("UserLeft", (args) =>
        {
            Debug.Log("UserLeft, " + "ConnectionId: " + args.connectionId + " ������ ���ӿ��� �����Ͽ����ϴ�.");
        });
    }

    private void RegisterEvent_OnTimeToBetting()
    {
        _connection.On<DTO_OnTimeToBetting>("OnTimeToBetting", (args) =>
        {
            Debug.Log("OnTimeToBetting, " + "�ڵ� ID: " + args.handId + "�� ������ �ʿ��մϴ�.");
        });
    }

    private void RegisterEvent_OnPayout()
    {
        _connection.On<DTO_OnPayout>("OnPayout", (args) =>
        {
            Debug.Log("OnPayout, " + "�ڵ� ID: " + args.handId + "�� " + "���: " + args.evaluationResult + "�� ������ �����մϴ�.");
        });
    }

    private void RegisterEvent_OnCardDealt()
    {
        _connection.On<DTO_OnCardDealt>("OnCardDealt", (args) =>
        {
            Debug.Log("OnCardDealt, " + "�÷��̾�: " + args.playerName + "�� " + "�ڵ� ID: " + args.handId + "�� " + "ī�� " + args.cardString + "��/�� �й��մϴ�." + " �÷��̾� Guid: " + args.playerGuid);
        });
    }

    private void RegisterEvent_OnPlayerBusted()
    {
        _connection.On<DTO_OnPlayerBusted>("OnPlayerBusted", (args) =>
        {
            Debug.Log("OnPlayerBusted, " + "�÷��̾�: " + args.playerName + "�� " + "�ڵ� ID: " + args.handId + "��/�� Bust �Ǿ����ϴ�." + " �÷��̾� Guid: " + args.playerGuid);
        });
    }

    private void RegisterEvent_OnActionDone()
    {
        _connection.On<DTO_OnActionDone>("OnActionDone", (args) =>
        {
            Debug.Log("OnActionDone, " + "�÷��̾�: " + args.playerName + "�� " + "�ڵ� ID: " + args.handId + "�� Action�� �Ϸ��մϴ�." + " �÷��̾� Guid: " + args.playerGuid);
        });
    }

    private void RegisterEvent_OnHandSplit()
    {
        _connection.On<DTO_OnHandSplit>("OnHandSplit", (args) =>
        {
            Debug.Log("OnHandSplit, " + "�÷��̾�: " + args.playerName + "�� " + "�ڵ� ID: " + args.handId + "�� Split�Ͽ� ���ڵ� ID: " + args.newHandId + "�� �����մϴ�.");
        });
    }

    private void RegisterEvent_OnDealerHoleCardRevealed()
    {
        _connection.On<DTO_OnDealerHoleCardRevealed>("OnDealerHoleCardRevealed", (args) =>
        {
            Debug.Log("OnDealerHoleCardRevealed, " + "������ ������ ī��: " + args.cardString + "��/�� �����մϴ�.");
        });
    }

    private void RegisterEvent_OnDealerCardDealt()
    {
        _connection.On<DTO_OnDealerCardDealt>("OnDealerCardDealt", (args) =>
        {
            Debug.Log("OnDealerCardDealt, " + "�������� ī��: " + args.cardString + "��/�� �й��մϴ�.");
        });
    }

    private void RegisterEvent_OnDealerHiddenCardDealt()
    {
        _connection.On<DTO_OnDealerHiddenCardDealt>("OnDealerHiddenCardDealt", (args) =>
        {
            Debug.Log("OnDealerHiddenCardDealt, " + "�������� ������ ī�带 �й��մϴ�.");
        });
    }

    private void RegisterEvent_OnTimeToAction()
    {
        _connection.On<DTO_OnTimeToAction>("OnTimeToAction", (args) =>
        {
            Debug.Log("OnTimeToAction, " + "�÷��̾�: " + args.playerName + "�� " + "�ڵ� ID: " + args.handId + "��/�� ������ ������ �����Դϴ�." + " �÷��̾� Guid: " + args.playerGuid);
        });
    }

    private void RegisterEvent_OnHandEvaluation()
    {
        _connection.On<DTO_OnHandEvaluation>("OnHandEvaluation", (args) =>
        {
            Debug.Log("OnHandEvaluation, " + "�÷��̾�: " + args.playerName + "�� " + "�ڵ� ID: " + args.handId + "�� " + "���: " + args.evaluationResult + "�Դϴ�." + " �÷��̾� Guid: " + args.playerGuid);
        });
    }
}
