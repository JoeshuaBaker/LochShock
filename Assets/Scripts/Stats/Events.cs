using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;

[System.Serializable]
public class Events
{
    public List<ScriptableAction> events;

    public Events()
    {
        if(events == null)
        {
            events = new List<ScriptableAction>();
        }
    }

    public void AddSource(Item item)
    {
        foreach(ScriptableAction e in events)
        {
            e.source = item;
        }
    }

    public Events Copy()
    {
        Events copy = new Events();
        copy.events.AddRange(events);
        return copy;
    }

    public IEnumerable<T> GetEvents<T>() where T : ScriptableAction
    {
        return events.OfType<T>();
    }

    public List<T> GetEventsAsList<T>() where T : ScriptableAction
    {
        return GetEvents<T>().ToList();
    }

    public void Add(Events other)
    {
        events.AddRange(other.events);
    }

    public void Remove(Events other)
    {
        events.RemoveRange(other.events);
    }
}
