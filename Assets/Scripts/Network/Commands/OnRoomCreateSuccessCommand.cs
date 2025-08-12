using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnRoomCreateSuccessCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnRoomCreateSuccessDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnRoomCreateSuccessDTO>(payload);

        Debug.Log("OnRoomCreateSuccess, " + "Room 생성에 성공했습니다.");

        GameManager.Instance.HandleOnRoomCreateSuccessMessage(dto);

        yield return null;
    }
}
