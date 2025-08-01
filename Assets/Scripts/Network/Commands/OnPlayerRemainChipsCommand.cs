using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerRemainChipsCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnPlayerRemainChipsDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnPlayerRemainChipsDTO>(payload);

        Debug.Log("OnPlayerRemainChips, " + "�÷��̾� Guid: " + dto.playerGuid + "�� ������ Ĩ: " + dto.chips);

        Player player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);

        player.SetPlayerChips(int.Parse(dto.chips));
    }
}
