using PimDeWitte.UnityMainThreadDispatcher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerRemainChipsCommand : IGameCommand
{
    Player _player;

    public IEnumerator Execute(string payload)
    {
        OnPlayerRemainChipsDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnPlayerRemainChipsDTO>(payload);

        Debug.Log("OnPlayerRemainChips, " + "플레이어 Guid: " + dto.playerGuid + "가 소지한 칩: " + dto.chips);

        _player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        _player.SetPlayerChips(int.Parse(dto.chips));

        foreach (var hand in _player.Hands)
        {
            WorkForUI(hand);
        }

        yield return null;
    }

    private void WorkForUI(PlayerHand hand)
    {
        int handIndex = GameManager.Instance.characterManager.GetHandIndex(hand);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.uiManager.PlayerInfoChipSetText((_player.Chips).ToString("N0"), handIndex);
        });
    }
}
