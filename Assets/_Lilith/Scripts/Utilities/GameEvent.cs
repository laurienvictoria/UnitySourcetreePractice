public class GameEvent
{
    public delegate void GameEventDelegate();
    private event GameEventDelegate gameEvent;

    public void Register(GameEventDelegate callback)
    {
        gameEvent += callback;
    }

    public void Unregister(GameEventDelegate callback)
    {
        gameEvent -= callback;
    }

    public void Dispatch()
    {
        gameEvent?.Invoke();
    }
}

public class GameEvent<T>
{
    public delegate void GameEventDelegate(T arg1);
    private event GameEventDelegate gameEvent;

    public void Register(GameEventDelegate callback)
    {
        gameEvent += callback;
    }

    public void Unregister(GameEventDelegate callback)
    {
        gameEvent -= callback;
    }

    public void Dispatch(T data)
    {
        gameEvent?.Invoke(data);
    }
}