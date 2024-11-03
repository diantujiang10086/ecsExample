using System;
using System.Collections.Generic;
public static class EventSystem
{
    private static readonly Dictionary<Type, List<IEvent>> allEvents = new();

    static EventSystem()
    {
        foreach (var type in typeof(EventSystem).Assembly.GetTypes())
        {
            var objects = type.GetCustomAttributes(typeof(EventAttribute), true);
            if (objects.Length == 0)
                continue;

            var obj = Activator.CreateInstance(type) as IEvent;

            foreach (var o in objects)
            {
                if (!allEvents.TryGetValue(obj.Type, out var list))
                {
                    list = new List<IEvent>();
                    allEvents[obj.Type] = list;
                }
                list.Add(obj);
            }
        }
    }

    public static void Publish<T>(T a) where T : struct
    {
        if (!allEvents.TryGetValue(typeof(T), out var list))
            return;

        foreach (var e in list)
        {
            if (e is AEvent<T> aEvent)
            {
                aEvent.Handle(a);
            }
        }
    }
}
