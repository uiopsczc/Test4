using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace CsCat
{
  public partial class GameComponent
  {
    protected EventDispatchersPlugin eventDispatchersPlugin
    {
      get
      {
        return cache.GetOrAddDefault(() => { return new EventDispatchersPlugin(Client.instance.eventDispatchers); });
      }
    }

    public EventListenerInfo AddListener(string eventName, Action handler)
    {
      return eventDispatchersPlugin.AddListener(eventName, handler);
    }

    public EventListenerInfo AddListener(EventName eventName, Action handler)
    {
      return eventDispatchersPlugin.AddListener(eventName, handler);
    }

    public bool RemoveListener(string eventName, Action handler)
    {
      return eventDispatchersPlugin.RemoveListener(eventName, handler);
    }

    public bool RemoveListener(EventName eventName, Action handler)
    {
      return eventDispatchersPlugin.RemoveListener(eventName, handler);
    }

    public bool RemoveListener(EventListenerInfo eventListenerInfo)
    {
      return eventDispatchersPlugin.RemoveListener(eventListenerInfo);
    }

    public bool RemoveListener(Action handler)
    {
      return eventDispatchersPlugin.RemoveListener(handler);
    }

    public void Broadcast(string eventName)
    {
      eventDispatchersPlugin.Broadcast(eventName);
    }

    public void Broadcast(EventName eventName)
    {
      eventDispatchersPlugin.Broadcast(eventName);
    }



    public EventListenerInfo<P0> AddListener<P0>(string eventName, Action<P0> handler)
    {
      return eventDispatchersPlugin.AddListener(eventName, handler);
    }

    public EventListenerInfo<P0> AddListener<P0>(EventName eventName, Action<P0> handler)
    {
      return eventDispatchersPlugin.AddListener(eventName, handler);
    }

    public bool RemoveListener<P0>(string eventName, Action<P0> handler)
    {
      return eventDispatchersPlugin.RemoveListener(eventName, handler);
    }

    public bool RemoveListener<P0>(EventName eventName, Action<P0> handler)
    {
      return eventDispatchersPlugin.RemoveListener(eventName, handler);
    }

    public bool RemoveListener<P0>(EventListenerInfo<P0> eventListenerInfo)
    {
      return eventDispatchersPlugin.RemoveListener(eventListenerInfo);
    }

    public bool RemoveListener<P0>(Action<P0> handler)
    {
      return eventDispatchersPlugin.RemoveListener(handler);
    }

    public void Broadcast<P0>(string eventName, P0 p0)
    {
      eventDispatchersPlugin.Broadcast(eventName, p0);
    }

    public void Broadcast<P0>(EventName eventName, P0 p0)
    {
      eventDispatchersPlugin.Broadcast(eventName, p0);
    }



    public EventListenerInfo<P0, P1> AddListener<P0, P1>(string eventName, Action<P0, P1> handler)
    {
      return eventDispatchersPlugin.AddListener(eventName, handler);
    }

    public EventListenerInfo<P0, P1> AddListener<P0, P1>(EventName eventName, Action<P0, P1> handler)
    {
      return eventDispatchersPlugin.AddListener(eventName, handler);
    }

    public bool RemoveListener<P0, P1>(string eventName, Action<P0, P1> handler)
    {
      return eventDispatchersPlugin.RemoveListener(eventName, handler);
    }

    public bool RemoveListener<P0, P1>(EventName eventName, Action<P0, P1> handler)
    {
      return eventDispatchersPlugin.RemoveListener(eventName, handler);
    }

    public bool RemoveListener<P0, P1>(EventListenerInfo<P0, P1> eventListenerInfo)
    {
      return eventDispatchersPlugin.RemoveListener(eventListenerInfo);
    }

    public bool RemoveListener<P0, P1>(Action<P0, P1> handler)
    {
      return eventDispatchersPlugin.RemoveListener(handler);
    }

    public void Broadcast<P0, P1>(string eventName, P0 p0, P1 p1)
    {
      eventDispatchersPlugin.Broadcast(eventName, p0, p1);
    }

    public void Broadcast<P0, P1>(EventName eventName, P0 p0, P1 p1)
    {
      eventDispatchersPlugin.Broadcast(eventName, p0, p1);
    }




    public EventListenerInfo<P0, P1, P2> AddListener<P0, P1, P2>(string eventName, Action<P0, P1, P2> handler)
    {
      return eventDispatchersPlugin.AddListener(eventName, handler);
    }

    public EventListenerInfo<P0, P1, P2> AddListener<P0, P1, P2>(EventName eventName, Action<P0, P1, P2> handler)
    {
      return eventDispatchersPlugin.AddListener(eventName, handler);
    }

    public bool RemoveListener<P0, P1, P2>(string eventName, Action<P0, P1, P2> handler)
    {
      return eventDispatchersPlugin.RemoveListener(eventName, handler);
    }

    public bool RemoveListener<P0, P1, P2>(EventName eventName, Action<P0, P1, P2> handler)
    {
      return eventDispatchersPlugin.RemoveListener(eventName, handler);
    }

    public bool RemoveListener<P0, P1, P2>(EventListenerInfo<P0, P1, P2> eventListenerInfo)
    {
      return eventDispatchersPlugin.RemoveListener(eventListenerInfo);
    }

    public bool RemoveListener<P0, P1, P2>(Action<P0, P1, P2> handler)
    {
      return eventDispatchersPlugin.RemoveListener(handler);
    }

    public void Broadcast<P0, P1, P2>(string eventName, P0 p0, P1 p1, P2 p2)
    {
      eventDispatchersPlugin.Broadcast(eventName, p0, p1, p2);
    }

    public void Broadcast<P0, P1, P2>(EventName eventName, P0 p0, P1 p1, P2 p2)
    {
      eventDispatchersPlugin.Broadcast(eventName, p0, p1, p2);
    }



    public EventListenerInfo<P0, P1, P2, P3> AddListener<P0, P1, P2, P3>(string eventName,
      Action<P0, P1, P2, P3> handler)
    {
      return eventDispatchersPlugin.AddListener(eventName, handler);
    }

    public EventListenerInfo<P0, P1, P2, P3> AddListener<P0, P1, P2, P3>(EventName eventName,
      Action<P0, P1, P2, P3> handler)
    {
      return eventDispatchersPlugin.AddListener(eventName, handler);
    }

    public bool RemoveListener<P0, P1, P2, P3>(string eventName,
      Action<P0, P1, P2, P3> handler)
    {
      return eventDispatchersPlugin.RemoveListener(eventName, handler);
    }

    public bool RemoveListener<P0, P1, P2, P3>(EventName eventName,
      Action<P0, P1, P2, P3> handler)
    {
      return eventDispatchersPlugin.RemoveListener(eventName, handler);
    }

    public bool RemoveListener<P0, P1, P2, P3>(
      EventListenerInfo<P0, P1, P2, P3> eventListenerInfo)
    {
      return eventDispatchersPlugin.RemoveListener(eventListenerInfo);
    }

    public bool RemoveListener<P0, P1, P2, P3>(Action<P0, P1, P2, P3> handler)
    {
      return eventDispatchersPlugin.RemoveListener(handler);
    }

    public void Broadcast<P0, P1, P2, P3>(string eventName, P0 p0, P1 p1, P2 p2, P3 p3)
    {
      eventDispatchersPlugin.Broadcast(eventName, p0, p1, p2, p3);
    }

    public void Broadcast<P0, P1, P2, P3>(EventName eventName, P0 p0, P1 p1, P2 p2, P3 p3)
    {
      eventDispatchersPlugin.Broadcast(eventName, p0, p1, p2, p3);
    }


    public void RemoveAllListeners()
    {
      eventDispatchersPlugin.RemoveAllListeners();
    }


  }
}