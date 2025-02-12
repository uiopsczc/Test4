using UnityEngine;

namespace CsCat
{
	public class SingletonUtil
	{
		/// <summary>
		/// Mono类的单例调用这里
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static T GetInstanceMono<T>(T instance) where T : MonoBehaviour, ISingleton
		{
			if (instance != null) return instance;
			//检查场景有效的物体中是否有名为(Singleton)xxx【xxx为T的类名】
			string targetName = string.Format(SingletonConst.StringSingleFormat, typeof(T).GetLastName());
			GameObject instanceGameObject = GameObject.Find(targetName);
			if (instanceGameObject != null)
			{
				instance = instanceGameObject.GetComponent<T>();
				instance.SingleInit();
				return instance;
			}

			if (GameObject.Find(SingletonConst.StringSingletonMaster))
			{
				//检测失效物体中是否有名为(Singleton)xxx【xxx为T的类名】
				var objects = SingletonFactory.instance.GetMono<SingletonMaster>()
					.inActiveGameObjects;
				for (var i = 0; i < objects.Length; i++)
				{
					GameObject inActiveGameObject = objects[i];
					if (!inActiveGameObject.name.Equals(targetName)) continue;
					instance = inActiveGameObject.GetComponent<T>();
					instance.SingleInit();
					return instance;
				}
			}

			//如果都没有，新建一个
			instanceGameObject = new GameObject(targetName);
			instance = instanceGameObject.AddComponent<T>();
			instance.SingleInit();
			return instance;
		}

		/// <summary>
		/// 非Mono类的单例调用这里
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static T GetInstance<T>(T instance) where T : ISingleton, new()
		{
			if (instance != null) return instance;
			instance = new T();
			instance.SingleInit();

			return instance;
		}
	}
}