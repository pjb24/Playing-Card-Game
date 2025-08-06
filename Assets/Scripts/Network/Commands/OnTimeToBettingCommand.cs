using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTimeToBettingCommand : IGameCommand
{
    private Player _player;
    private PlayerHand _hand;

    private int _betAmount = 0;

    public IEnumerator Execute(string payload)
    {
        OnTimeToBettingDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnTimeToBettingDTO>(payload);

        Debug.Log("OnTimeToBetting, " + "베팅이 필요합니다.");

        _betAmount = 0;

        _player = GameManager.Instance.characterManager.ClientPlayer;

        _hand = _player.GetNextBettingHand();

        Enter();

        yield return null;
    }

    private void Enter()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.ChangeToBetPanel();

            UpdateUI_PlayerInfos();
            UpdateUI_CardValues();

            // Hide join button
            GameManager.Instance.uiManager.button_Join.visible = false;

            // Button subscribe function
            GameManager.Instance.uiManager.button_BetReset.clicked += HandleBetReset;
            GameManager.Instance.uiManager.button_Bet1.clicked += HandleBet1;
            GameManager.Instance.uiManager.button_Bet2.clicked += HandleBet2;
            GameManager.Instance.uiManager.button_Bet3.clicked += HandleBet3;
            GameManager.Instance.uiManager.button_Bet4.clicked += HandleBet4;
            GameManager.Instance.uiManager.button_BetMax.clicked += HandleBetMax;
            GameManager.Instance.uiManager.button_BetConfirm.clicked += HandleBetConfirm;
        });
    }

    private void UpdateUI_PlayerInfos()
    {
        GameManager.Instance.uiManager.RemoveAllPlayerInfos();

        foreach (var player in GameManager.Instance.characterManager.Players)
        {
            foreach (var hand in player.Hands)
            {
                int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);
                GameManager.Instance.uiManager.CreatePlayerInfo(handIndex);
                GameManager.Instance.uiManager.PlayerInfoVisible(handIndex);

                GameManager.Instance.uiManager.PlayerInfoBetAmountSetText(hand.BetAmount.ToString("N0"), handIndex);
                GameManager.Instance.uiManager.PlayerInfoNameSetText(player.DisplayName, handIndex);
                GameManager.Instance.uiManager.PlayerInfoChipSetText(player.Chips.ToString("N0"), handIndex);

                Vector3 targetPosition = GameManager.Instance.GetHandPosition(hand);
                GameManager.Instance.uiManager.RequestPlayerInfoPositionUpdate_Register(targetPosition, handIndex);
                GameManager.Instance.uiManager.RequestPlayerInfoPositionUpdate_Y_Register(handIndex);
            }
        }
    }

    private void UpdateUI_CardValues()
    {
        GameManager.Instance.uiManager.RemoveDealerCardValue();
        GameManager.Instance.uiManager.RemoveAllLabelCardValue();
    }

    public void UpdateUI()
    {
        int handIndex = GameManager.Instance.characterManager.GetHandIndex(_hand);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.PlayerInfoBetAmountSetText(_betAmount.ToString("N0"), handIndex);
            GameManager.Instance.uiManager.PlayerInfoChipSetText((_player.Chips - _betAmount).ToString("N0"), handIndex);
        });
    }

    public void HandleBetReset()
    {
        _betAmount = 0;
        GameManager.Instance.chipFactory.ResetChips(_hand);
        UpdateUI();
    }

    public void HandleBet1()
    {
        if (_betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (_player.Chips - _betAmount < (int)E_ChipValue.Bet1)
        {
            return;
        }

        if (_betAmount + (int)E_ChipValue.Bet1 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        _betAmount += (int)E_ChipValue.Bet1;

        // Animate chips
        GameManager.Instance.chipFactory.CreateChipType1(_hand);
        GameManager.Instance.chipFactory.UpdateHandChipPosition(_hand);

        UpdateUI();
    }

    public void HandleBet2()
    {
        if (_betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (_player.Chips - _betAmount < (int)E_ChipValue.Bet2)
        {
            return;
        }

        if (_betAmount + (int)E_ChipValue.Bet2 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        _betAmount += (int)E_ChipValue.Bet2;

        // Animate chips
        GameManager.Instance.chipFactory.CreateChipType2(_hand);
        GameManager.Instance.chipFactory.UpdateHandChipPosition(_hand);

        UpdateUI();
    }

    public void HandleBet3()
    {
        if (_betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (_player.Chips - _betAmount < (int)E_ChipValue.Bet3)
        {
            return;
        }

        if (_betAmount + (int)E_ChipValue.Bet3 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        _betAmount += (int)E_ChipValue.Bet3;

        // Animate chips
        GameManager.Instance.chipFactory.CreateChipType3(_hand);
        GameManager.Instance.chipFactory.UpdateHandChipPosition(_hand);

        UpdateUI();
    }

    public void HandleBet4()
    {
        if (_betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        if (_player.Chips - _betAmount < (int)E_ChipValue.Bet4)
        {
            return;
        }

        if (_betAmount + (int)E_ChipValue.Bet4 > (int)E_ChipValue.BetMax)
        {
            return;
        }

        _betAmount += (int)E_ChipValue.Bet4;

        // Animate chips
        GameManager.Instance.chipFactory.CreateChipType4(_hand);
        GameManager.Instance.chipFactory.UpdateHandChipPosition(_hand);

        UpdateUI();
    }

    public void HandleBetMax()
    {
        if (_betAmount >= (int)E_ChipValue.BetMax)
        {
            return;
        }

        GameManager.Instance.chipFactory.ResetChips(_hand);

        if (_player.Chips < (int)E_ChipValue.BetMax)
        {
            _betAmount = GameManager.Instance.chipFactory.CreateChipsToFitValue(_player.Chips, _hand);
        }
        else
        {
            _betAmount = (int)E_ChipValue.BetMax;
            GameManager.Instance.chipFactory.CreateChipType5(_hand);
        }

        // Animate chips
        GameManager.Instance.chipFactory.UpdateHandChipPosition(_hand);

        UpdateUI();
    }

    public void HandleBetConfirm()
    {
        _player.PlaceBet(_hand, _betAmount);

        PlaceBetDTO placeBetDTO = new PlaceBetDTO();
        placeBetDTO.amount = _betAmount;
        placeBetDTO.handId = _hand.Id;
        string placeBetJson = Newtonsoft.Json.JsonConvert.SerializeObject(placeBetDTO);
        GameManager.Instance.SignalRClient.Execute("PlaceBet", placeBetJson);

        _hand = _player.GetNextBettingHand();
        _betAmount = 0;

        if (_hand == null)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                // Unsubscribe function
                GameManager.Instance.uiManager.button_BetReset.clicked -= HandleBetReset;
                GameManager.Instance.uiManager.button_Bet1.clicked -= HandleBet1;
                GameManager.Instance.uiManager.button_Bet2.clicked -= HandleBet2;
                GameManager.Instance.uiManager.button_Bet3.clicked -= HandleBet3;
                GameManager.Instance.uiManager.button_Bet4.clicked -= HandleBet4;
                GameManager.Instance.uiManager.button_BetMax.clicked -= HandleBetMax;
                GameManager.Instance.uiManager.button_BetConfirm.clicked -= HandleBetConfirm;
            });
        }
    }
}
