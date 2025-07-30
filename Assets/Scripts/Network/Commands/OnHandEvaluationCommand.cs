using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHandEvaluationCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnHandEvaluationDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnHandEvaluationDTO>(payload);

        Debug.Log("OnHandEvaluation, " + "플레이어: " + dto.playerName + "의 " + "핸드 ID: " + dto.handId + "의 " + "결과: " + dto.evaluationResult + "입니다." + " 플레이어 Guid: " + dto.playerGuid);
    }
}
