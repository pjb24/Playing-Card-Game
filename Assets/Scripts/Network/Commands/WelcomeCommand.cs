using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WelcomeCommand : IGameCommand
{
    public void Execute(string payload)
    {
        WelcomeDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<WelcomeDTO>(payload);

        Debug.Log("Welcome, " + dto.message);
    }
}
