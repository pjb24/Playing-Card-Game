using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : PersistentSingleton<NetworkManager>
{
    private SignalRClient _signalRClient;
    public SignalRClient SignalRClient => _signalRClient;

    protected override void Awake()
    {
        // 반드시 base.Awake()를 호출하여 싱글톤 로직이 실행되게 해야 합니다.
        base.Awake();

        // 여기에 개별 초기화 코드를 추가할 수 있습니다.
        _signalRClient = new SignalRClient();
    }

    private void Start()
    {
        _signalRClient.Start();
    }

    private void OnApplicationQuit()
    {
        _signalRClient.OnApplicationQuit();
    }
}