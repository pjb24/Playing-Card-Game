using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBetPlacedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnBetPlacedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnBetPlacedDTO>(payload);

        Debug.Log("OnBetPlaced, " + "�÷��̾� " + dto.playerName + "��/�� �ڵ� ID: " + dto.handId + "�� " + dto.betAmount + "�� �����Ͽ����ϴ�.");
    }
}
