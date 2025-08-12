using System.Collections;
using UnityEngine;

public class OnFullExistRoomListCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnFullExistRoomListDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnFullExistRoomListDTO>(payload);

        Debug.Log("OnFullExistRoomList, " + "존재하는 Room들을 확인합니다.");

        GameManager.Instance.HandleOnFullExistRoomListMessage(dto);

        yield return null;
    }
}