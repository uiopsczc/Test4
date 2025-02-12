using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using XLua.LuaDLL;

namespace CsCat
{
	/// <summary>
	///   1、整个Lua虚拟机执行的脚本分成3个模块：热修复、公共模块、逻辑模块
	///   2、公共模块：提供Lua语言级别的工具类支持，和游戏逻辑无关，最先被启动
	///   3、热修复模块：脚本全部放Lua目录下，随着游戏的启动而启动
	///   4、逻辑模块：资源热更完毕后启动
	///   5、资源热更以后，理论上所有被加载的Lua脚本都要重新执行加载，
	/// </summary>
	public class XLuaManager : MonoBehaviour, ISingleton
	{
		private Action<bool> luaOnApplicationPause;

		private Action luaOnApplicationQuite;
		private Action luaOnGUI;
		private XLuaUpdater luaUpdater;

		public static XLuaManager instance => SingletonFactory.instance.GetMono<XLuaManager>();

		public bool isGameStarted { get; protected set; }


		public LuaEnv luaEnv { get; private set; }

		public void SingleInit()
		{
			InitLuaEnv();
		}


		private void AddLibs()
		{
			luaEnv.AddBuildin("rapidjson", Lua.LoadRapidJson);
			luaEnv.AddBuildin("lpeg", Lua.LoadLpeg);
			luaEnv.AddBuildin("pb", Lua.LoadLuaProfobuf);
			luaEnv.AddBuildin("ffi", Lua.LoadFFI);
		}

		private void InitLuaEnv()
		{
			luaEnv = new LuaEnv();
			AddLibs();
			isGameStarted = false;
			luaEnv.AddLoader(LuaRequireLoader.GetLoader);
		}

		public void OnInit()
		{
			if (luaEnv == null)
				return;
			luaEnv.LoadLua(XLuaConst.Common_Main_Lua_Name);
			luaUpdater = gameObject.GetOrAddComponent<XLuaUpdater>();
			luaUpdater.OnInit(luaEnv);
			luaEnv.LoadLua("LuaTest");
		}


		private void Update()
		{
			if (luaEnv == null)
				return;
			luaEnv.Tick();

			if (Time.frameCount % 100 == 0)
				luaEnv.FullGc();
		}

		public void Dispose()
		{
			if (luaUpdater != null)
				luaUpdater.OnDispose();
			luaEnv.SafeDispose();
		}

		public void StartXLua()
		{
			StartMain();
			//    StartHotfix();
		}


		public void StartHotfix(bool restart = false)
		{
			if (luaEnv == null)
				return;

			if (restart)
			{
				StopHotfix();
				luaEnv.ReloadLua(XLuaConst.Hotfix_Main_Lua_Name);
			}
			else
			{
				luaEnv.LoadLua(XLuaConst.Hotfix_Main_Lua_Name);
			}

			luaEnv.SafeDoString(string.Format("{0}.Start()", XLuaConst.Hotfix_Main_Lua_Name));
		}

		public void StopHotfix()
		{
			luaEnv.SafeDoString(string.Format("{0}.Stop()", XLuaConst.Hotfix_Main_Lua_Name));
		}


		public void StartMain()
		{
			if (luaEnv == null)
				return;
			luaEnv.LoadLua(XLuaConst.Main_Lua_Name);
			luaOnApplicationQuite =
				luaEnv.Global.Get<Action>("OnApplicationQuit");
			luaOnApplicationPause =
				luaEnv.Global.Get<Action<bool>>("OnApplicationPause");
			luaOnGUI =
				luaEnv.Global.Get<Action>("OnGUI");

			luaEnv.SafeDoString(string.Format("{0}.StartUp()", XLuaConst.Main_Lua_Name));
			isGameStarted = true;
		}

		void OnApplicationQuit()
		{
			if (luaEnv == null || !isGameStarted)
				return;
			luaOnApplicationQuite();
		}

		private void OnApplicationPause(bool isPaused)
		{
			if (luaEnv == null || !isGameStarted)
				return;
			luaOnApplicationPause(isPaused);
		}

		private void OnGUI()
		{
			if (luaEnv == null || !isGameStarted)
				return;
			luaOnGUI();
		}

		public T Get<T>(string key)
		{
			//    string[] splits = key.Split(".");
			//    int i = 0;
			//    if (splits.Length == 1)
			//      return luaEnv.Global.Get<T>(splits[0]);
			//    LuaTable luaTable = luaEnv.Global;
			//    while (i < splits.Length - 1)
			//    {
			//      luaTable = luaTable.Get<LuaTable>(splits[i]);
			//      i++;
			//    }
			//
			//    return luaTable.Get<T>(splits[i]);
			return this.luaEnv.Global.GetInPath<T>(key);
		}


		public object[] CallLuaFunction(string key, params object[] args)
		{
			List<string> list = new List<string>();
			string[] splits = key.Split(".");
			for (int i = 0; i <= splits.Length - 2; i++)
			{
				list.Add(splits[i]);
			}

			string luaFunctionString;
			bool isStatic = key.IndexOf(":") == -1 ? true : false;
			if (!isStatic)
			{
				string[] lastSplits = splits[splits.Length - 1].Split(":");
				list.Add(lastSplits[0]);
				luaFunctionString = lastSplits[1];
			}
			else
				luaFunctionString = splits[splits.Length - 1];

			string tableKey = list.Concat(".");
			LuaTable luaTable = tableKey.IsNullOrWhiteSpace() ? luaEnv.Global : Get<LuaTable>(tableKey);
			LuaFunction luaFunction = luaTable.Get<LuaFunction>(luaFunctionString);
			List<object> argList = new List<object>();
			if (args.Length > 0)
				argList.AddRange(args);
			if (!isStatic)
				argList.AddFirst(luaTable);
			object[] result = luaFunction.Call(argList.ToArray());
			luaFunction.Dispose();
			return result;
		}
	}
}