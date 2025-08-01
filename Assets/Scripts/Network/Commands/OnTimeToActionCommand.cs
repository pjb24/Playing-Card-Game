using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.ShadowCascadeGUI;

public class OnTimeToActionCommand : IGameCommand
{
    private Player _player;
    private PlayerHand _hand;

    public void Execute(string payload)
    {
        OnTimeToActionDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnTimeToActionDTO>(payload);

        Debug.Log("OnTimeToAction, " + "�÷��̾�: " + dto.playerName + "�� " + "�ڵ� ID: " + dto.handId + "��/�� ������ ������ �����Դϴ�." + " �÷��̾� Guid: " + dto.playerGuid);

        _player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        _hand = _player.GetHandByGuid(dto.handId);

        WorkForUI();
    }

    private void WorkForUI()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.ChangeToPlayerActionPanel();

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
        // ���ÿ� ����� Ĩ�� ������ ���� Ĩ�� �ʿ�
        if (_player.Chips < _hand.BetAmount)
        {
            return;
        }

        // �ڵ忡 ī�尡 2��, ī���� ���� �Ǵ� ���ڰ� ���ƾ� ��
        if (!_hand.CanSplit())
        {
            //return;
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
