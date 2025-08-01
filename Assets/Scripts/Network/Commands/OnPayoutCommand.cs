using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPayoutCommand : IGameCommand
{
    public void Execute(string payload)
    {
        OnPayoutDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnPayoutDTO>(payload);

        Debug.Log("OnPayout, " + "플레이어 Guid: " + dto.playerGuid + " 핸드 ID: " + dto.handId + "에 " + "결과: " + dto.evaluationResult.ToString() + "로 정산을 수행합니다.");
    }
}
