using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGameStateChangedCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnGameStateChangedDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnGameStateChangedDTO>(payload);

        Debug.Log("OnGameStateChanged, " + "게임의 State가 " + dto.state + "로 변경되었습니다.");

        GameManager.Instance.ChangeState(GameStateFactory.Create(dto.state));
    }
}
