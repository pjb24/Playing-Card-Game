using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using PimDeWitte.UnityMainThreadDispatcher;

public class CommandDispatcher
{
    private Dictionary<string, IGameCommand> _commands = new();

    private ConcurrentQueue<IGameCommand> _queue = new();
    private ConcurrentQueue<string> _payloadQueue = new();

    private readonly object _lock = new();
    private volatile bool _isCoroutineRunning = false;

    public void RegisterCommand(string commandName, IGameCommand command)
    {
        _commands[commandName] = command;
    }

    public void Dispatch(string commandName, string payload)
    {
        if (_commands.TryGetValue(commandName, out var command))
        {
            _queue.Enqueue(command);
            _payloadQueue.Enqueue(payload);

            lock (_lock)
            {
                if (!_isCoroutineRunning)
                {
                    _isCoroutineRunning = true;
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        GameManager.Instance.StartCoroutine(ExecuteCoroutine());
                    });
                }
            }
        }
        else
        {
            Debug.LogWarning($"Unknown command received: {commandName}");
        }
    }

    private IEnumerator ExecuteCoroutine()
    {
        while (_queue.TryDequeue(out IGameCommand command))
        {
            string payload;
            _payloadQueue.TryDequeue(out payload);
            yield return GameManager.Instance.StartCoroutine(command.Execute(payload));
        }

        _isCoroutineRunning = false;
    }
}
