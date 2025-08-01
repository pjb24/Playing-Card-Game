using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerBustedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnPlayerBustedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnPlayerBustedDTO>(payload);

        Debug.Log("OnPlayerBusted, " + "�÷��̾�: " + dto.playerName + "�� " + "�ڵ� ID: " + dto.handId + "��/�� Bust �Ǿ����ϴ�." + " �÷��̾� Guid: " + dto.playerGuid);

        Player player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        WorkForUI(hand);
    }

    private void WorkForUI(PlayerHand hand)
    {
        int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.CardValuePlayerSetText(hand.GetValue().ToString(), handIndex);
        });
    }
}
