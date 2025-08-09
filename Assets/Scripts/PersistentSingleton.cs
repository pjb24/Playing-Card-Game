using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // 씬에서 해당 타입의 컴포넌트를 찾습니다.
                _instance = FindAnyObjectByType<T>();

                // 씬에 없다면 새로 생성합니다.
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 이미 인스턴스가 존재하고, 그 인스턴스가 자신이 아니라면
        if (_instance != null && _instance != this as T)
        {
            // 이 오브젝트는 파괴합니다. (중복 방지)
            Destroy(this.gameObject);
        }
        else
        {
            // 이 인스턴스를 유일한 인스턴스로 설정합니다.
            _instance = this as T;

            // 씬이 전환될 때 이 오브젝트가 파괴되지 않도록 설정합니다.
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
