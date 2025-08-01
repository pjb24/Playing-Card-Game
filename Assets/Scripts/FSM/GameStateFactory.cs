using System;
using System.Collections;
using System.Collections.Generic;

public static class GameStateFactory
{
    private static Dictionary<string, Func<IGameState>> _registry = new()
    {
        { "GameStartState", () => new GameStartState() },
        { "BettingState", () => new BettingState() },
        { "DealingState", () => new DealingState() },
        { "PlayerTurnState", () => new PlayerTurnState() },
        { "DealerTurnState", () => new DealerTurnState() },
        { "ResultState", () => new ResultState() },
        { "GameEndState", () => new GameEndState() },
    };

    public static IGameState Create(string name)
    {
        if (_registry.TryGetValue(name, out var creator))
        {
            return creator();
        }

        throw new ArgumentException($"No class registered for name: {name}");
    }
}
