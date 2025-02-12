using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Object = UnityEngine.Object;

namespace CsCat
{
	public class ResLoad
	{
		private readonly Dictionary<string, ResLoadDataInfo> _resLoadDataInfoDict = new Dictionary<string, ResLoadDataInfo>();
		public bool isNotCheckDestroy;
		public ResLoad(bool isNotCheckDestroy = false)
		{
			this.isNotCheckDestroy = isNotCheckDestroy;
		}
		public bool IsAllLoadDone()
		{
			foreach (var keyValue in _resLoadDataInfoDict)
			{
				var resLoadDataInfo = keyValue.Value;
				if (!resLoadDataInfo.resLoadData.assetCat.IsLoadDone())
					return false;
			}

			return true;
		}

		public IEnumerator IEIsAllLoadDone(Action onAllLoadDoneCallback = null)
		{
			yield return new WaitUntil(IsAllLoadDone);
			onAllLoadDoneCallback?.Invoke();
		}

		public bool IsLoadDone(string assetPath)
		{
			if (!_resLoadDataInfoDict.TryGetValue(assetPath.GetMainAssetPath(), out var resLoadDataInfo))
				return false;
			return resLoadDataInfo.resLoadData.assetCat.IsLoadDone();
		}

		public IEnumerator IEIsLoadDone(string assetPath, Action onLoadDoneCallback = null)
		{
			yield return new WaitUntil(() => IsLoaded(assetPath));
			onLoadDoneCallback?.Invoke();
		}

		// 加载某个资源
		public AssetCat GetOrLoadAsset(string assetPath, Action<AssetCat> onLoadSuccessCallback = null,
		  Action<AssetCat> onLoadFailCallback = null,
		  Action<AssetCat> onLoadDoneCallback = null, object callbackCause = null)
		{
			AssetCat assetCat;
			if (!Application.isPlaying)
			{
#if UNITY_EDITOR
				Object obj = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
				if (!_resLoadDataInfoDict.ContainsKey(assetPath.GetMainAssetPath()))
				{
					assetCat = new AssetCat(assetPath);
					_resLoadDataInfoDict[assetPath.GetMainAssetPath()] = new ResLoadDataInfo(new ResLoadData(assetCat), isNotCheckDestroy);
				}

				var resLoadDataInfo = _resLoadDataInfoDict[assetPath.GetMainAssetPath()];
				if (!resLoadDataInfo.resLoadData.assetCat.assetDict.ContainsKey(obj.name))
					resLoadDataInfo.resLoadData.assetCat.assetDict[obj.name] = new Dictionary<Type, Object>();
				if (!resLoadDataInfo.resLoadData.assetCat.assetDict[obj.name].ContainsKey(obj.GetType()))
					resLoadDataInfo.resLoadData.assetCat.assetDict[obj.name][obj.GetType()] = obj;
				onLoadSuccessCallback?.Invoke(resLoadDataInfo.resLoadData.assetCat);
				onLoadDoneCallback?.Invoke(resLoadDataInfo.resLoadData.assetCat);
				return resLoadDataInfo.resLoadData.assetCat;
#endif
			}
			callbackCause = callbackCause ?? this;
			assetCat =
			  Client.instance.assetBundleManager.GetOrLoadAssetCat(assetPath.GetMainAssetPath(), onLoadSuccessCallback,
				onLoadFailCallback, onLoadDoneCallback, callbackCause);
			if (!_resLoadDataInfoDict.ContainsKey(assetPath.GetMainAssetPath()))
				_resLoadDataInfoDict[assetPath.GetMainAssetPath()] = new ResLoadDataInfo(new ResLoadData(assetCat), isNotCheckDestroy);
			_resLoadDataInfoDict[assetPath.GetMainAssetPath()].AddCallbackCause(callbackCause);
			return assetCat;
		}


		public void CancelLoadCallback(AssetCat assetCat, object callbackCause = null)
		{
			string toRemoveKey = null;
			foreach (var keyValue in _resLoadDataInfoDict)
			{
				var key = keyValue.Key;
				var resLoadDataInfo = _resLoadDataInfoDict[key];
				if (resLoadDataInfo.resLoadData.assetCat == assetCat)
				{
					resLoadDataInfo.RemoveCallbackCause(callbackCause);
					if (resLoadDataInfo.callbackCauseDict.Count == 0 && !isNotCheckDestroy)//is_not_check_destroy的时候不删除，因为要在destroy的时候作为删除的asset的依据
						toRemoveKey = key;
					break;
				}
			}

			if (toRemoveKey != null)
				_resLoadDataInfoDict.Remove(toRemoveKey);
		}

		public void CancelLoadAllCallbacks(AssetCat assetCat)
		{
			string toRemoveKey = null;
			foreach (var keyValue in _resLoadDataInfoDict)
			{
				var key = keyValue.Key;
				var resLoadDataInfo = _resLoadDataInfoDict[key];
				if (resLoadDataInfo.resLoadData.assetCat == assetCat)
				{
					resLoadDataInfo.RemoveAllCallbackCauses();
					if (resLoadDataInfo.callbackCauseDict.Count == 0 && !isNotCheckDestroy)//is_not_check_destroy的时候不删除，因为要在destroy的时候作为删除的asset的依据
						toRemoveKey = key;
					break;
				}
			}
			if (toRemoveKey != null)
				_resLoadDataInfoDict.Remove(toRemoveKey);
		}


		public void Reset()
		{
			foreach (var keyValue in _resLoadDataInfoDict)
			{
				var resLoadDataInfo = keyValue.Value;
				resLoadDataInfo.Destroy();
			}
			_resLoadDataInfoDict.Clear();
		}

		public void Destroy()
		{
			Reset();
		}
	}
}