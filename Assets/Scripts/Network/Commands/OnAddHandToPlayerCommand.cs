using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnAddHandToPlayerCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnAddHandToPlayerDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnAddHandToPlayerDTO>(payload);

        Debug.Log("OnAddHandToPlayer, " + "�÷��̾� Guid: " + dto.playerGuid + "�� " + "�ڵ� ID: " + dto.handId + "�� �߰��մϴ�.");

        Player player = GameManager.Instance.characterManager.GetPlayerByGuid(dto.playerGuid);
        player.AddHand(dto.handId);
    }
}
