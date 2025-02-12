using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace CsCat
{
	public partial class AssetBundleManager
	{
		#region 常驻内存的

		//常驻内存的(游戏中一直存在的)asset
		public void SetResidentAssets(bool isResident, params string[] assetPaths)
		{
			for (var i = 0; i < assetPaths.Length; i++)
			{
				var assetPath = assetPaths[i];
				if (isResident)
					assetResidentDict[assetPath] = true;
				else
				{
					assetResidentDict.Remove(assetPath);
					AddAssetCatOfNoRef(this.GetAssetCat(assetPath));
				}
			}
		}

		//常驻内存的(游戏中一直存在的)asset
		public void SetResidentAssets(List<string> assetPathList, bool isResident = true)
		{
			SetResidentAssets(isResident, assetPathList.ToArray());
		}

		#endregion

		private AssetCat _GetOrAddAssetCat(string assetPath)
		{
			assetCatDict.TryGetValue(assetPath, out var targetAssetCat);

			//编辑器模式下的或者是resouce模式下的,没有找到则创建一个AssetCat
			if (Application.isEditor && EditorModeConst.IsEditorMode
				|| assetPath.Contains(FilePathConst.ResourcesFlag)
			)
			{
				Object[] assets = null;
				if (assetPath.Contains(FilePathConst.ResourcesFlag))
				{
					var resourcePath = assetPath.Substring(assetPath.IndexEndOf(FilePathConst.ResourcesFlag) + 1)
					  .GetMainAssetPath().WithoutSuffix();
					assets = Resources.LoadAll(resourcePath);
				}
				else
				{
#if UNITY_EDITOR
					assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
#endif
				}

				if (!assets.IsNullOrEmpty()) //有该资源
				{
					targetAssetCat = new AssetCat(assetPath);
					targetAssetCat.SetAssets(assets);
					assetCatDict[assetPath] = targetAssetCat;
				}
				else
					return null;
			}

			return targetAssetCat;
		}

		private bool _UnLoadUnUseAsset(string assetPath)
		{
			return _UnLoadUnUseAsset(GetAssetCat(assetPath));
		}

		private bool _UnLoadUnUseAsset(AssetCat assetCat)
		{
			if (assetCat == null)
				return false;
			if (!IsAssetLoadSuccess(assetCat.assetPath))
				return false;

			if (assetCat.refCount == 0)
			{
				_RemoveAssetCat(assetCat);
				return true;
			}

			return false;
		}

		public AssetCat GetOrLoadAssetCat(string assetPath, Action<AssetCat> onLoadSuccessCallback = null,
		  Action<AssetCat> onLoadFailCallback = null, Action<AssetCat> onLoadDoneCallback = null,
		  object callbackCause = null)
		{
			AssetCat assetCat;
			//编辑器模式下的或者是resouce模式下的
			if (Application.isEditor && EditorModeConst.IsEditorMode
				|| assetPath.Contains(FilePathConst.ResourcesFlag)
			)
			{
				assetCat = _GetOrAddAssetCat(assetPath);
				onLoadSuccessCallback?.Invoke(assetCat);
				onLoadDoneCallback?.Invoke(assetCat);
				return assetCat;
			}

			if (IsAssetLoadSuccess(assetPath))
			{
				assetCat = GetAssetCat(assetPath);
				onLoadSuccessCallback?.Invoke(assetCat);
				onLoadDoneCallback?.Invoke(assetCat);
				return assetCat;
			}

			LoadAssetAsync(assetPath, onLoadSuccessCallback, onLoadFailCallback, onLoadDoneCallback,
			  callbackCause);
			assetCat = GetAssetCat(assetPath);
			return assetCat;
		}

		public IEnumerator LoadAssetAsync(List<string> assetPathList,
		  Action<AssetCat> onLoadSuccessCallback = null, Action<AssetCat> onLoadFailCallback = null,
		  Action<AssetCat> onLoadDoneCallback = null, object callbackCause = null)
		{
			if (Application.isEditor && EditorModeConst.IsEditorMode)
			{
				for (var i = 0; i < assetPathList.Count; i++)
				{
					var assetPath = assetPathList[i];
					onLoadSuccessCallback?.Invoke(_GetOrAddAssetCat(assetPath));
					onLoadDoneCallback?.Invoke(_GetOrAddAssetCat(assetPath));
				}

				yield break;
			}

			for (var i = 0; i < assetPathList.Count; i++)
			{
				var assetPath = assetPathList[i];
				LoadAssetAsync(assetPath, onLoadSuccessCallback, onLoadFailCallback, onLoadDoneCallback,
					callbackCause);
			}

			yield return new WaitUntil(() =>
			{
				for (var i = 0; i < assetPathList.Count; i++)
				{
					var assetPath = assetPathList[i];
					var assetCat = _GetOrAddAssetCat(assetPath);
					if (!assetCat.IsLoadDone())
						return false;
				}

				return true;
			});
		}

		public BaseAssetAsyncLoader LoadAssetAsync(string assetPath, Action<AssetCat> onLoadSuccessCallback = null,
		  Action<AssetCat> onLoadFailCallback = null, Action<AssetCat> onLoadDoneCallback = null,
		  object callbackCause = null)
		{
			AssetCat assetCat;
			if (Application.isEditor && EditorModeConst.IsEditorMode
				|| assetPath.Contains(FilePathConst.ResourcesFlag)
			)
			{
				assetCat = _GetOrAddAssetCat(assetPath);
				onLoadSuccessCallback?.Invoke(assetCat);
				onLoadDoneCallback?.Invoke(assetCat);
				return new EditorAssetAsyncLoader(assetCat);
			}

			var assetBundleName = assetPathMap.GetAssetBundleName(assetPath);
			if (assetBundleName == null)
				LogCat.error(string.Format("{0}没有设置成ab包", assetPath));

			if (assetCatDict.ContainsKey(assetPath))
			{
				assetCat = GetAssetCat(assetPath);
				//已经加载成功
				if (assetCat.IsLoadSuccess())
				{
					onLoadSuccessCallback?.Invoke(assetCat);
					onLoadDoneCallback?.Invoke(assetCat);
					return null;
				}

				//加载中
				assetCat.AddOnLoadSuccessCallback(onLoadSuccessCallback, callbackCause);
				assetCat.AddOnLoadFailCallback(onLoadFailCallback, callbackCause);
				assetCat.AddOnLoadDoneCallback(onLoadDoneCallback, callbackCause);
				return assetAsyncloaderProcessingList.Find(assetAsyncloader =>
				  assetAsyncloader._assetCat.assetPath.Equals(assetPath));
			}

			//没有加载
			var assetAsyncLoader = PoolCatManager.Default.SpawnValue<AssetAsyncLoader>(null, null);
			assetCat = new AssetCat(assetPath);
			_AddAssetCat(assetPath, assetCat);
			//添加对assetAsyncLoader的引用
			assetCat.AddRefCount();
			assetCat.AddOnLoadSuccessCallback(onLoadSuccessCallback, callbackCause);
			assetCat.AddOnLoadFailCallback(onLoadFailCallback, callbackCause);
			assetCat.AddOnLoadDoneCallback(onLoadDoneCallback, callbackCause);

			var assetBundleLoader = _LoadAssetBundleAsync(assetBundleName);
			assetAsyncLoader.Init(assetCat, assetBundleLoader);
			//asset拥有对assetBundle的引用
			var assetBundleCat = assetBundleLoader._assetBundleCat;
			assetBundleCat.AddRefCount();

			assetCat.assetBundleCat = assetBundleCat;

			assetAsyncloaderProcessingList.Add(assetAsyncLoader);
			return assetAsyncLoader;
		}

		private void _CheckAssetOfNoRefList()
		{
			for (var i = _assetCatOfNoRefList.Count - 1; i >= 0; i--)
			{
				var assetCat = _assetCatOfNoRefList[i];
				if (assetCat.refCount <= 0)
				{
					_assetCatOfNoRefList.Remove(assetCat);
					_RemoveAssetCat(assetCat);
				}
			}
		}

		private void _OnAssetAsyncLoaderFail(AssetAsyncLoader assetAsyncLoader)
		{
			if (!assetAsyncloaderProcessingList.Contains(assetAsyncLoader))
				return;

			//失败的时候解除对assetCat的引用
			assetAsyncLoader._assetCat.SubRefCount();
		}

		private void _OnAssetAsyncLoaderDone(AssetAsyncLoader assetAsyncLoader)
		{
			if (!assetAsyncloaderProcessingList.Contains(assetAsyncLoader))
				return;

			//解除对assetAsyncLoader的引用
			assetAsyncLoader._assetCat.SubRefCount();

			assetAsyncloaderProcessingList.Remove(assetAsyncLoader);
			assetAsyncLoader.DoDestroy();
			PoolCatManager.Default.DespawnValue(assetAsyncLoader);
		}

		// 添加没有被引用的assetCat,延迟到下一帧删除
		public void AddAssetCatOfNoRef(AssetCat assetCat)
		{
			if (assetCat == null)
				return;
			if (!_assetCatOfNoRefList.Contains(assetCat) && !assetResidentDict.ContainsKey(assetCat.assetPath))
				_assetCatOfNoRefList.Add(assetCat);
		}

		public bool IsAssetLoadSuccess(string assetPath)
		{
			return assetCatDict.ContainsKey(assetPath) && _GetOrAddAssetCat(assetPath).IsLoadSuccess();
		}

		public AssetCat GetAssetCat(string assetPath)
		{
			assetCatDict.TryGetValue(assetPath, out var target);
			return target;
		}


		public void _RemoveAssetCat(string assetPath)
		{
			_RemoveAssetCat(this.GetAssetCat(assetPath));
		}

		private void _RemoveAssetCat(AssetCat assetCat)
		{
			if (assetCat == null)
				return;
			assetCatDict.RemoveByFunc((key, value) => value == assetCat);
			if (Application.isEditor && EditorModeConst.IsEditorMode
				|| assetCat.assetPath.Contains(FilePathConst.ResourcesFlag)
			)
				return;
			var assetBundleCat = assetCat.assetBundleCat;
			//assetBundle_name的dependencies的引用-1
			assetBundleCat?.SubRefCountOfDependencies();
			//减少一个assetBundle对asset的引用
			assetBundleCat?.SubRefCount();
		}

		private void _AddAssetCat(string assetPath, AssetCat asset)
		{
			assetCatDict[assetPath] = asset;
		}

		public string GetAssetBundlePath(string assetPath)
		{
			return assetPathMap.GetAssetBundleName(assetPath);
		}
	}
}