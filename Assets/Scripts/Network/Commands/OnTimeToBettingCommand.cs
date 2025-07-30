using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTimeToBettingCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnTimeToBettingDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnTimeToBettingDTO>(payload);

        Debug.Log("OnTimeToBetting, " + "�ڵ� ID: " + dto.handId + "�� ������ �ʿ��մϴ�.");
    }
}
