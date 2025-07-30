using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTimeToBettingCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnTimeToBettingDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnTimeToBettingDTO>(payload);

        Debug.Log("OnTimeToBetting, " + "핸드 ID: " + dto.handId + "에 베팅이 필요합니다.");
    }
}
