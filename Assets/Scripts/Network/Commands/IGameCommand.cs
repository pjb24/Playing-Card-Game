using System.Collections;

public interface IGameCommand
{
    IEnumerator Execute(string payload);
}
