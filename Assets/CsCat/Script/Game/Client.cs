using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CsCat
{
	public class Client : TickObject, ISingleton
	{
		public MoveManager moveManager;

		//管理模块
		public AssetBundleManager assetBundleManager;
		public AssetBundleUpdater assetBundleUpdater;
		public AudioManager audioManager;

		public PhysicsManager physicsManager;
		public CommandManager commandManager = new CommandManager();
		public GameHotKeyManager gameHotKeyManager;

		public FrameCallbackManager FrameCallbackManager = new FrameCallbackManager();
		public GuidManager guidManager;

		public RandomManager randomManager = new RandomManager();

		//    public RedDotManager redDotManager;
		public CfgManager cfgManager;


		public RPN rpn = new RPN();
		public CombatBase combat;

		public UserFactory userFactory;
		public RoleFactory roleFactory;
		public ItemFactory itemFactory;
		public MissionFactory missionFactory;
		public DoerEventFactory doerEventFactory;
		public SceneFactory sceneFactory;


		//    public RedDotLogic redDotLogic;

		public User user;
		public Role mainRole;


		public StageBase stage;
		public SyncUpdate syncUpdate = new SyncUpdate();

		public IdPool idPool = new IdPool();

		//通用模块
		public override TimerManager timerManager => _cache.GetOrAddDefault(() => new TimerManager());

		public UIManager uiManager;
		public static Client instance => SingletonFactory.instance.Get<Client>();


		public string language;

		public void SingleInit()
		{
		}

		public void Init(GameObject gameObject)
		{
			base.Init();
			this.graphicComponent.SetGameObject(gameObject, true);
		}

		public override void Start()
		{
			base.Start();
#if !UNITY_EDITOR
      EditorModeConst.Is_Editor_Mode = false;
#endif
			this.moveManager = graphicComponent.gameObject.AddComponent<MoveManager>();

			language = GameData.instance.langData.language;
			guidManager = new GuidManager(GameData.instance.guidCurrent);
			assetBundleUpdater = AddChild<AssetBundleUpdater>("AssetBundleUpdater");
			assetBundleManager = AddChild<AssetBundleManager>("AssetBundleManager");
			cfgManager = AddChild<CfgManager>("CfgManager");
			audioManager = AddChild<AudioManager>("AudioManager");
			physicsManager = AddChild<PhysicsManager>("physicsManager");
			gameHotKeyManager = AddChild<GameHotKeyManager>("DefaultInputManager");
			uiManager = AddChildWithoutInit<UIManager>("UIManager");
			uiManager.Init();
			uiManager.PostInit();
			uiManager.SetIsEnabled(true, false);

			//      redDotManager = AddChild<RedDotManager>("RedDotManager");


			userFactory = AddChild<UserFactory>("UserFactory");
			roleFactory = AddChild<RoleFactory>("RoleFactory");
			missionFactory = AddChild<MissionFactory>("MissionFactory");
			itemFactory = AddChild<ItemFactory>("ItemFactory");
			doerEventFactory = AddChild<DoerEventFactory>("DoerEventFactory");
			sceneFactory = AddChild<SceneFactory>("SceneFactory");


			user = GameData2.instance.RestoreUser();
			TestUser();
			Test();
			Goto<StageShowLogo>();
		}

		public void Test()
		{
			TestProtoTest.Test();
		}


		public void TestUser()
		{
			//    ItemTest.Test();
		}


		public void Goto<T>(float fadeHideDuration = 0f, Action onStageShowCallback = null)
			where T : StageBase, new()
		{
			StartCoroutine(IEGoto<T>(fadeHideDuration, onStageShowCallback));
		}

		public IEnumerator IEGoto<T>(float fadeHideDuration = 0f, Action onStageShowCallback = null)
			where T : StageBase, new()
		{
			if (stage != null)
			{
				if (fadeHideDuration > 0)
				{
					uiManager.FadeTo(0, 1, fadeHideDuration);

					yield return new WaitForSeconds(fadeHideDuration);
				}

				yield return stage.IEPreDestroy();
				this.RemoveChild(stage.key);
				stage = null;
			}

			stage = this.AddChild<T>(null);
			stage.onShowCallback = onStageShowCallback;
			stage.Start();
		}

		//重启
		public void Reboot()
		{
			Goto<StageTest>();
		}

		protected override void _Update(float deltaTime, float unscaledDeltaTime)
		{
			base._Update(deltaTime, unscaledDeltaTime);
			eventDispatchers.FireEvent(GlobalEventNameConst.Update);
			eventDispatchers.FireEvent(GlobalEventNameConst.Update, deltaTime);
			eventDispatchers.FireEvent(GlobalEventNameConst.Update, deltaTime, unscaledDeltaTime);

			this.timerManager.Update(deltaTime, unscaledDeltaTime);
			syncUpdate.Update();
			FrameCallbackManager.Update();
		}

		protected override void _LateUpdate(float deltaTime, float unscaledDeltaTime)
		{
			base._LateUpdate(deltaTime, unscaledDeltaTime);
			eventDispatchers.FireEvent(GlobalEventNameConst.LateUpdate);
			eventDispatchers.FireEvent(GlobalEventNameConst.LateUpdate, deltaTime);
			eventDispatchers.FireEvent(GlobalEventNameConst.LateUpdate, deltaTime, unscaledDeltaTime);

			this.timerManager.LateUpdate(deltaTime, unscaledDeltaTime);
			FrameCallbackManager.LateUpdate();
		}

		protected override void _FixedUpdate(float deltaTime, float unscaledDeltaTime)
		{
			base._FixedUpdate(deltaTime, unscaledDeltaTime);
			eventDispatchers.FireEvent(GlobalEventNameConst.FixedUpdate);
			eventDispatchers.FireEvent(GlobalEventNameConst.FixedUpdate, deltaTime);
			eventDispatchers.FireEvent(GlobalEventNameConst.FixedUpdate, deltaTime, unscaledDeltaTime);

			this.timerManager.FixedUpdate(deltaTime, unscaledDeltaTime);
			FrameCallbackManager.FixedUpdate();
		}

		public void OnApplicationQuit()
		{
			GameData.instance.quitTimeTicks = DateTimeUtil.NowTicks();
			GameData.instance.Save();
			GameData2.instance.Save();
		}

		public void OnApplicationPause(bool isPaused)
		{
		}


	}
}