using System.Collections;
using UnityEngine;

public class OnGrantRoomMasterCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnGrantRoomMasterDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnGrantRoomMasterDTO>(payload);

        Debug.Log("OnGrantRoomMaster, " + "당신이 룸마스터로 설정되었습니다.");

        GameManager.Instance.HandleOnGrantRoomMasterMessage(dto);

        yield return null;
    }
}