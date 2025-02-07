using UnityEngine;

namespace CsCat
{
	public class AutoAssetDestroy : MonoBehaviour
	{
		private AssetCat _assetCat;

		/// <summary>
		///   只能通过这个方法添加
		/// </summary>
		/// <param name="go"></param>
		/// <param name="assetCat"></param>
		public static void Add(GameObject go, AssetCat assetCat)
		{
			assetCat.AddRefCount();
			var autoAssetDestroy = go.AddComponent<AutoAssetDestroy>();
			autoAssetDestroy._assetCat = assetCat;
		}

		private void OnDestroy()
		{
			if (_assetCat != null)
				_assetCat.SubRefCount(1, true);
			else
				LogCat.LogErrorFormat("{0} destroy but ont find assetCat", name);
		}
	}
}