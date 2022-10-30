using System;
using System.CodeDom;
using System.Collections.Generic;

namespace CsCat
{
	public partial class Entity
	{
		bool CheckCanAddComponentType(Type componentType)
		{
			var componentKey = componentType.FullName;
			if (this.keyToComponentPoolItemIndexDict.ContainsKey(componentKey))
			{
				LogCat.error("duplicate add component:", componentKey, componentType);
				return false;
			}
			return true;
		}
		protected Component _AddComponent(Type componentType, IPoolItemIndex componentPoolItemIndex)
		{
			var component = componentPoolItemIndex.GetValue<Component>();
			component.SetEntityPoolItemIndex(this._poolItemIndex);
			_AddComponentRelationship(componentType, componentPoolItemIndex);
			return component;
		}

		public Component AddComponentWithoutInit(Type componentType)
		{
			if (CheckCanAddComponentType(componentType))
				return null;
			var (componentPoolItem, componentPoolItemIndex) = this.GetPoolManager().Spawn(componentType);
			return _AddComponent(componentType, componentPoolItemIndex);
		}

		public T AddComponentWithoutInit<T>() where T : Component
		{
			return AddComponentWithoutInit(typeof(T)) as T;
		}

		public Component AddComponent(Type componentType, params object[] initArgs)
		{
			var component = AddComponentWithoutInit(componentType);
			if (component == null) //没有加成功
				return null;
			component.InvokeMethod("Init", false, initArgs);
			component.PostInit();
			component.SetIsEnabled(true);
			return component;
		}

		public T AddComponent<T>(params object[] initArgs) where T : Component
		{
			return AddComponent(typeof(T), initArgs) as T;
		}

		void _AddComponentRelationship(Type componentType, IPoolItemIndex componentPoolItemIndex)
		{
			var key = componentType.FullName;
			keyToComponentPoolItemIndexDict[key] = componentPoolItemIndex;
			componentPoolItemIndexList.Add(componentPoolItemIndex);
		}
	}
}