using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CsCat
{
	public class Main : MonoBehaviour, ISingleton
	{
		private MonoBehaviourCache _monoBehaviourCache;
		public Client client = Client.instance;
		public IEnumerator ie;

		public GameObject prefab;

		public MonoBehaviourCache monoBehaviourCache => _monoBehaviourCache ?? (_monoBehaviourCache = new MonoBehaviourCache(this));

		public static Main instance => SingletonFactory.instance.GetMono<Main>();

		public void SingleInit()
		{
		}

		private void Awake()
		{
			if (Application.isPlaying)
				DontDestroyOnLoad(gameObject);
			client.Init(this.gameObject);
			client.PostInit();
			client.SetIsEnabled(true, false);
		}

		private void Start()
		{
			client.Start();
			this.ie = DOIE();
		}

		public void Update()
		{
			client.Update(Time.deltaTime, Time.unscaledDeltaTime);
			client.CheckDestroyed();
		}

		public void LateUpdate()
		{
			client.LateUpdate(Time.deltaTime, Time.unscaledDeltaTime);
		}

		public void FixedUpdate()
		{
			client.FixedUpdate(Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
		}

		void OnApplicationQuit()
		{
			Client.instance.OnApplicationQuit();
		}


		void OnApplicationPause(bool isPaused)
		{
			Client.instance.OnApplicationPause(isPaused);
		}

		IEnumerator DOIE()
		{
			LogCat.log(1);
			yield return new WaitForSeconds(5);
			LogCat.log(2);
			yield return new WaitForSeconds(5);
			LogCat.log(3);
		}


	}
}