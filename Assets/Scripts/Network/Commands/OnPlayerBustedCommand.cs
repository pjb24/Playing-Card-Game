using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerBustedCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnPlayerBustedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnPlayerBustedDTO>(payload);

        Debug.Log("OnPlayerBusted, " + "플레이어: " + dto.playerName + "의 " + "핸드 ID: " + dto.handId + "이/가 Bust 되었습니다." + " 플레이어 Guid: " + dto.playerGuid);

        Player player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        PlayerHand hand = player.GetHandByGuid(dto.handId);

        WorkForUI(hand);

        yield return null;
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
