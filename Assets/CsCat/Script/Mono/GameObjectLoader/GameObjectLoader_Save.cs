using System.Collections;
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace CsCat
{
	public partial class GameObjectLoader
	{
		private Hashtable _refIdHashtable = new Hashtable();
#if UNITY_EDITOR
		public void Save()
		{
			_refIdHashtable.Clear();
			Hashtable dict = new Hashtable();
			dict["childList"] = _GetSaveChildList(this.transform);
			string content = MiniJson.JsonEncode(dict);
			string filePath = textAsset.GetAssetPath().WithRootPath(FilePathConst.ProjectPath);
			StdioUtil.WriteTextFile(filePath, content);
			AssetPathRefManager.instance.Save();
			AssetDatabase.Refresh();
			LogCat.log("保存完成");
		}


		private ArrayList _GetSaveChildList(Transform parentTransform)
		{
			int childCount = parentTransform.childCount;
			ArrayList tilemapList = new ArrayList();
			for (int i = 0; i < childCount; i++)
			{
				Transform childTransform = parentTransform.GetChild(i);
				Hashtable childHashtable = _GetSaveChild(childTransform);
				tilemapList.Add(childHashtable);
			}

			return tilemapList;
		}

		private Hashtable _GetSaveChild(Transform childTransform)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["name"] = childTransform.name;
			hashtable["TransformHashtable"] = childTransform.GetSerializeHashtable();

			Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(childTransform.gameObject);
			bool isPrefab = prefab != null;
			if (isPrefab)
			{
				long prefabRefId = AssetPathRefManager.instance.GetRefIdByGuid(prefab.GetGUID());
				hashtable["prefabRefId"] = prefabRefId;
				_refIdHashtable[prefabRefId] = true;
			}
			//如果是预设则不用递归子节点
			if (!isPrefab && childTransform.childCount > 0)
				hashtable["childList"] = _GetSaveChildList(childTransform);

			List<Type> exceptList = new List<Type> { typeof(Transform), typeof(Tilemap) };
			var components = childTransform.GetComponents<UnityEngine.Component>();
			for (var i = 0; i < components.Length; i++)
			{
				var component = components[i];
				if (!exceptList.Contains(component.GetType()))
				{
					var componentHashtable = component.InvokeExtensionMethod<Hashtable>("GetSerializeHashtable") ??
					                          new Hashtable();
					string key = string.Format("{0}Hashtable", component.GetType().FullName);
					hashtable[key] = componentHashtable;
				}
			}

			Tilemap tilemap = childTransform.GetComponent<Tilemap>();
			if (tilemap != null)
				hashtable["TilemapHashtable"] = tilemap.GetSerializeHashtable(_refIdHashtable);

			return hashtable;
		}




#endif

	}
}




