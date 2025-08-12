using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGameEndCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnGameEndDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnGameEndDTO>(payload);

        Debug.Log("OnGameEnd");

        GameManager.Instance.HandleOnGameEndMessage(dto);

        yield return null;
    }
}
