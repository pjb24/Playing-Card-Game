using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerRemainChipsCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnPlayerRemainChipsDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnPlayerRemainChipsDTO>(payload);

        Debug.Log("OnPlayerRemainChips, " + "플레이어 Guid: " + dto.playerGuid + "가 소지한 칩: " + dto.chips);

        Player player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        player.SetPlayerChips(int.Parse(dto.chips));
    }
}
