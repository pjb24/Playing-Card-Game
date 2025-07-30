using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGameStateChangedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnGameStateChangedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnGameStateChangedDTO>(payload);

        Debug.Log("OnGameStateChanged, " + "������ State�� " + dto.state + "�� ����Ǿ����ϴ�.");
    }
}
