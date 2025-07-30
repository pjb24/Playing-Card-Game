using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnErrorCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnErrorDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnErrorDTO>(payload);

        Debug.Log("OnError, " + dto.message);
    }
}
