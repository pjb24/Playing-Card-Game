using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTimeToBettingCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnTimeToBettingDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnTimeToBettingDTO>(payload);

        Debug.Log("OnTimeToBetting, " + "베팅이 필요합니다.");

        GameManager.Instance.HandleOnTimeToBettingMessage(dto);

        yield return null;
    }
}
