using System;

public interface IEvent
{
    Type Type { get; }
}

public abstract class AEvent<A> : IEvent
{
    public Type Type => typeof(A);

    protected abstract void Run(A a);

    public void Handle(A a)
    {
        try
        {
            Run(a);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }
    }
}