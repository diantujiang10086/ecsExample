using UnityEngine;

[Event]
public class GameStartEvent : AEvent<GameStart>
{
    protected override void Run(GameStart a)
    {
        Debug.Log("game start");
    }
}
