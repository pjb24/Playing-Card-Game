using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHandSplitCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnHandSplitDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnHandSplitDTO>(payload);

        Debug.Log("OnHandSplit, " + "플레이어: " + dto.playerName + "의 " + "핸드 ID: " + dto.handId + "를 Split하여 새핸드 ID: " + dto.newHandId + "를 생성합니다. " + "플레이어 Guid: " + dto.playerGuid);

        GameManager.Instance.HandleOnHandSplitMessage(dto);

        yield return null;
    }
}
