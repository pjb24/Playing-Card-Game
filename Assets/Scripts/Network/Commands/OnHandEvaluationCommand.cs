using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHandEvaluationCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnHandEvaluationDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnHandEvaluationDTO>(payload);

        Debug.Log("OnHandEvaluation, " + "�÷��̾�: " + dto.playerName + "�� " + "�ڵ� ID: " + dto.handId + "�� " + "���: " + dto.evaluationResult + "�Դϴ�." + " �÷��̾� Guid: " + dto.playerGuid);
    }
}
