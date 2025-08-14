using System.Collections;
using UnityEngine;

public class OnChangedRoomListCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnChangedRoomListDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnChangedRoomListDTO>(payload);

        Debug.Log("OnChangedRoomList, " + "Room의 변경사항을 확인합니다.");

        GameManager.Instance.HandleOnChangedRoomListMessage(dto);

        yield return null;
    }
}
