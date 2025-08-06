using System.Collections;
using UnityEngine;

public class OnExistingPlayerListCommand : IGameCommand
{
    public IEnumerator Execute(string payload)
    {
        OnExistingPlayerListDTO dto = Newtonsoft.Json.JsonConvert.DeserializeObject<OnExistingPlayerListDTO>(payload);

        Debug.Log("OnExistingPlayerList, " + "게임 방에 존재하는 Player들을 확인합니다.");

        foreach (var item in dto.players)
        {
            Player player = new Player(item.playerGuid, item.userName);
            GameManager.Instance.characterManager.AddPlayer(player);
        }

        yield return null;
    }
}