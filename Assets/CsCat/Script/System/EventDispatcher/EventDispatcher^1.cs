using System;

namespace CsCat
{
	public class EventDispatcher<P0> :IEventDispatcher
	{
		private readonly ValueListDictionary<string, (Action<P0> handler, bool isValid)> _listenerDict =
			new ValueListDictionary<string, (Action<P0> handler, bool isValid)>();

		public Action<P0> AddListener(string eventName, Action<P0> handler)
		{
			_listenerDict.Add(eventName, (handler, true));
			return handler;
		}

		public void IRemoveListener(string eventName, Delegate handler)
		{
			RemoveListener(eventName, (Action<P0>)handler);
		}

		public bool RemoveListener(string eventName, Action<P0> handler)
		{
			if (_listenerDict.TryGetValue(eventName, out var listenerList))
			{
				for (var i = 0; i < listenerList.Count; i++)
				{
					var handlerInfo = listenerList[i];
					if (handlerInfo.handler != handler) continue;
					if (!handlerInfo.isValid) continue;
					handlerInfo.isValid = false;
					return true;
				}
			}

			return false;
		}

		public void RemoveAllListeners()
		{
			_listenerDict.Clear();
		}

		public void Broadcast(string eventName, P0 p0)
		{
			if (_listenerDict.TryGetValue(eventName, out var listenerList))
			{
				for (int i = 0; i < listenerList.Count; i++)
				{
					var handlerInfo = listenerList[i];
					if (handlerInfo.isValid == false)
					{
						listenerList.RemoveAt(i);
						i--;//使i保持不变，因为有i++
						continue;
					}
					handlerInfo.handler(p0);
				}
			}
		}
	}
}