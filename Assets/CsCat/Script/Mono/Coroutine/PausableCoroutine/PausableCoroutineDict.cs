using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CsCat
{
	public class PausableCoroutineDict
	{
		private readonly MonoBehaviour _monoBehaviour;
		private readonly PoolItemDict<ulong> _idPoolItemDict = new PoolItemDict<ulong>(new IdPool());
		private readonly Dictionary<string, PausableCoroutine> _dict = new Dictionary<string, PausableCoroutine>();
		private readonly List<string> _toRemoveKeyList = new List<string>();
		public PausableCoroutineDict(MonoBehaviour monoBehaviour)
		{
			this._monoBehaviour = monoBehaviour;
		}

		public MonoBehaviour GetMonoBehaviour()
		{
			return _monoBehaviour;
		}

		public string StartCoroutine(IEnumerator iEnumerator, string key = null)
		{
			_CleanFinishedCoroutines();
			key = key ?? _idPoolItemDict.Get().ToString();
			var coroutine = _monoBehaviour.StopAndStartCachePausableCoroutine(key.ToGuid(this), iEnumerator);
			this._dict[key] = coroutine;
			return key;
		}

		/// <summary>
		/// 此处的key与StartCoroutine的key保持一致
		/// </summary>
		/// <param name="key"></param>
		public void StopCoroutine(string key)
		{
			_CleanFinishedCoroutines();
			if (!this._dict.ContainsKey(key))
				return;
			this._dict.Remove(key);
			if (ulong.TryParse(key, out ulong id))
				_idPoolItemDict.Remove(id);
			_monoBehaviour.StopCachePausableCoroutine(key.ToGuid(this));
		}

		public void StopAllCoroutines()
		{
			foreach (var keyValue in _dict)
			{
				var key = keyValue.Key;
				_monoBehaviour.StopCachePausableCoroutine(key.ToGuid(this));
			}

			_dict.Clear();
			_idPoolItemDict.Clear();
		}

		public void SetIsPaused(bool isPaused)
		{
			_CleanFinishedCoroutines();
			foreach (var keyValue in _dict)
			{
				var key = keyValue.Key;
				this._dict[key].SetIsPaused(isPaused);
			}
		}

		void _CleanFinishedCoroutines()
		{
			foreach (var keyValue in _dict)
			{
				var key = keyValue.Key;
				var coroutine = keyValue.Value;
				if (coroutine.isFinished)
				{
					if(ulong.TryParse(key, out var id))
						_idPoolItemDict.Remove(id);
					_toRemoveKeyList.Add(key);
				}
			}
			for (var i = 0; i < _toRemoveKeyList.Count; i++)
			{
				var toRemoveKey = _toRemoveKeyList[i];
				_dict.Remove(toRemoveKey);
			}
			_toRemoveKeyList.Clear();
		}
	}
}