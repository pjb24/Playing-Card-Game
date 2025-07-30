using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandDispatcher
{
    private Dictionary<string, IGameCommand> _commands = new();

    public void RegisterCommand(string commandName, IGameCommand command)
    {
        _commands[commandName] = command;
    }

    public void Dispatch(string commandName, string payload)
    {
        if (_commands.TryGetValue(commandName, out var command))
        {
            command.Execute(payload);
        }
        else
        {
            Debug.LogWarning($"Unknown command received: {commandName}");
        }
    }
}
