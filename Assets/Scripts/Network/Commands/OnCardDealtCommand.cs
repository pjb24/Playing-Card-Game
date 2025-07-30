using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCardDealtCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnCardDealtDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnCardDealtDTO>(payload);

        Debug.Log("OnCardDealt, " + "�÷��̾�: " + dto.playerName + "�� " + "�ڵ� ID: " + dto.handId + "�� " + "ī�� " + dto.cardString + "��/�� �й��մϴ�." + " �÷��̾� Guid: " + dto.playerGuid);
    }
}
