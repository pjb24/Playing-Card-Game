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

        // 수신 이벤트 설정
        RegisterReceiveMessageEvents();

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
            Debug.Log("UserConnected, " + "ConnectionId: " + args.message + " 유저가 연결되었습니다.");
        });
    }

    private void RegisterEvent_UserDisconnected()
    {
        _connection.On<DTO_UserDisconnected>("UserDisconnected", (args) =>
        {
            Debug.Log("UserDisconnected, " + "ConnectionId: " + args.message + " 유저의 연결이 해제되었습니다.");
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
            Debug.Log("OnJoinSuccess, " + "게임에 입장하였습니다." + "유저 이름: " + args.userName + " PlayerGuid: " + args.playerGuid);
        });
    }

    private void RegisterEvent_OnUserJoined()
    {
        _connection.On<DTO_OnUserJoined>("OnUserJoined", (args) =>
        {
            Debug.Log("OnUserJoined, " + "유저가 게임에 입장하였습니다." + " 유저 이름: " + args.userName);
        });
    }

    private void RegisterEvent_OnPlayerRemainChips()
    {
        _connection.On<DTO_OnPlayerRemainChips>("OnPlayerRemainChips", (args) =>
        {
            Debug.Log("OnPlayerRemainChips, " + "플레이어가 소지한 칩: " + args.chips);
        });
    }

    private void RegisterEvent_OnGameStateChanged()
    {
        _connection.On<DTO_OnGameStateChanged>("OnGameStateChanged", (args) =>
        {
            Debug.Log("OnGameStateChanged, " + "게임의 State가 " + args.state + "로 변경되었습니다.");
        });
    }

    private void RegisterEvent_OnBetPlaced()
    {
        _connection.On<DTO_OnBetPlaced>("OnBetPlaced", (args) =>
        {
            Debug.Log("OnBetPlaced, " + "플레이어 " + args.playerName + "이/가 핸드 ID: " + args.handId + "에 " + args.betAmount + "를 베팅하였습니다.");
        });
    }

    private void RegisterEvent_UserLeft()
    {
        _connection.On<DTO_UserLeft>("UserLeft", (args) =>
        {
            Debug.Log("UserLeft, " + "ConnectionId: " + args.connectionId + " 유저가 게임에서 퇴장하였습니다.");
        });
    }

    private void RegisterEvent_OnTimeToBetting()
    {
        _connection.On<DTO_OnTimeToBetting>("OnTimeToBetting", (args) =>
        {
            Debug.Log("OnTimeToBetting, " + "핸드 ID: " + args.handId + "에 베팅이 필요합니다.");
        });
    }

    private void RegisterEvent_OnPayout()
    {
        _connection.On<DTO_OnPayout>("OnPayout", (args) =>
        {
            Debug.Log("OnPayout, " + "핸드 ID: " + args.handId + "에 " + "결과: " + args.evaluationResult + "로 정산을 수행합니다.");
        });
    }

    private void RegisterEvent_OnCardDealt()
    {
        _connection.On<DTO_OnCardDealt>("OnCardDealt", (args) =>
        {
            Debug.Log("OnCardDealt, " + "플레이어: " + args.playerName + "의 " + "핸드 ID: " + args.handId + "에 " + "카드 " + args.cardString + "을/를 분배합니다." + " 플레이어 Guid: " + args.playerGuid);
        });
    }

    private void RegisterEvent_OnPlayerBusted()
    {
        _connection.On<DTO_OnPlayerBusted>("OnPlayerBusted", (args) =>
        {
            Debug.Log("OnPlayerBusted, " + "플레이어: " + args.playerName + "의 " + "핸드 ID: " + args.handId + "이/가 Bust 되었습니다." + " 플레이어 Guid: " + args.playerGuid);
        });
    }

    private void RegisterEvent_OnActionDone()
    {
        _connection.On<DTO_OnActionDone>("OnActionDone", (args) =>
        {
            Debug.Log("OnActionDone, " + "플레이어: " + args.playerName + "의 " + "핸드 ID: " + args.handId + "의 Action을 완료합니다." + " 플레이어 Guid: " + args.playerGuid);
        });
    }

    private void RegisterEvent_OnHandSplit()
    {
        _connection.On<DTO_OnHandSplit>("OnHandSplit", (args) =>
        {
            Debug.Log("OnHandSplit, " + "플레이어: " + args.playerName + "의 " + "핸드 ID: " + args.handId + "를 Split하여 새핸드 ID: " + args.newHandId + "를 생성합니다.");
        });
    }

    private void RegisterEvent_OnDealerHoleCardRevealed()
    {
        _connection.On<DTO_OnDealerHoleCardRevealed>("OnDealerHoleCardRevealed", (args) =>
        {
            Debug.Log("OnDealerHoleCardRevealed, " + "딜러의 숨겨진 카드: " + args.cardString + "을/를 공개합니다.");
        });
    }

    private void RegisterEvent_OnDealerCardDealt()
    {
        _connection.On<DTO_OnDealerCardDealt>("OnDealerCardDealt", (args) =>
        {
            Debug.Log("OnDealerCardDealt, " + "딜러에게 카드: " + args.cardString + "을/를 분배합니다.");
        });
    }

    private void RegisterEvent_OnDealerHiddenCardDealt()
    {
        _connection.On<DTO_OnDealerHiddenCardDealt>("OnDealerHiddenCardDealt", (args) =>
        {
            Debug.Log("OnDealerHiddenCardDealt, " + "딜러에게 숨겨진 카드를 분배합니다.");
        });
    }

    private void RegisterEvent_OnTimeToAction()
    {
        _connection.On<DTO_OnTimeToAction>("OnTimeToAction", (args) =>
        {
            Debug.Log("OnTimeToAction, " + "플레이어: " + args.playerName + "의 " + "핸드 ID: " + args.handId + "이/가 동작을 수행할 차례입니다." + " 플레이어 Guid: " + args.playerGuid);
        });
    }

    private void RegisterEvent_OnHandEvaluation()
    {
        _connection.On<DTO_OnHandEvaluation>("OnHandEvaluation", (args) =>
        {
            Debug.Log("OnHandEvaluation, " + "플레이어: " + args.playerName + "의 " + "핸드 ID: " + args.handId + "의 " + "결과: " + args.evaluationResult + "입니다." + " 플레이어 Guid: " + args.playerGuid);
        });
    }
}
