using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.ShadowCascadeGUI;

public class OnTimeToActionCommand : IGameCommand
{
    private Player _player;
    private PlayerHand _hand;

    public IEnumerator Execute(string payload)
    {
        OnTimeToActionDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnTimeToActionDTO>(payload);

        Debug.Log("OnTimeToAction, " + "플레이어: " + dto.playerName + "의 " + "핸드 ID: " + dto.handId + "이/가 동작을 수행할 차례입니다." + " 플레이어 Guid: " + dto.playerGuid);

        Enter();

        _player = GameManager.Instance.characterManager.ClientPlayer;

        if (_player.Id == dto.playerGuid)
        {
            _hand = _player.GetHandByGuid(dto.handId);

            WorkForUI();
        }

        yield return null;
    }

    private void Enter()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.ChangeToPlayerActionPanel();
        });
    }

    private void WorkForUI()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            // Button subscribe function
            GameManager.Instance.uiManager.button_Hit.clicked += HandleHit;
            GameManager.Instance.uiManager.button_Stand.clicked += HandleStand;
            GameManager.Instance.uiManager.button_Split.clicked += HandleSplit;
            GameManager.Instance.uiManager.button_DoubleDown.clicked += HandleDoubleDown;
        });
    }

    private void HandleHit()
    {
        HitDTO hitDTO = new HitDTO();
        hitDTO.handId = _hand.Id;
        string hitJson = Newtonsoft.Json.JsonConvert.SerializeObject(hitDTO);
        GameManager.Instance.SignalRClient.Execute("Hit", hitJson);
    }

    private void HandleStand()
    {
        StandDTO standDTO = new StandDTO();
        standDTO.handId = _hand.Id;
        string standJson = Newtonsoft.Json.JsonConvert.SerializeObject(standDTO);
        GameManager.Instance.SignalRClient.Execute("Stand", standJson);
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
            return;
        }

        SplitDTO splitDTO = new SplitDTO();
        splitDTO.handId = _hand.Id;
        string splitJson = Newtonsoft.Json.JsonConvert.SerializeObject(splitDTO);
        GameManager.Instance.SignalRClient.Execute("Split", splitJson);
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

        DoubleDownDTO doubleDownDTO = new DoubleDownDTO();
        doubleDownDTO.handId = _hand.Id;
        string doubleDownJson = Newtonsoft.Json.JsonConvert.SerializeObject(doubleDownDTO);
        GameManager.Instance.SignalRClient.Execute("DoubleDown", doubleDownJson);
    }

    public void RemoveListeners()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            // Unsubscribe function
            GameManager.Instance.uiManager.button_Hit.clicked -= HandleHit;
            GameManager.Instance.uiManager.button_Stand.clicked -= HandleStand;
            GameManager.Instance.uiManager.button_Split.clicked -= HandleSplit;
            GameManager.Instance.uiManager.button_DoubleDown.clicked -= HandleDoubleDown;
        });
    }
}
