using System;
using System.Diagnostics;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using CsCat;

namespace CsCat
{
	public class ShellUtil
	{
		private static readonly List<Action> _queue = new List<Action>();
		private readonly List<string> _environmentVarList = new List<string>();

		private static string shellApp
		{
			get
			{
#if UNITY_EDITOR_WIN
				string app = "cmd.exe";
#elif UNITY_EDITOR_OSX
			string app = "bash";
#endif
				return app;
			}
		}

		static ShellUtil()
		{
			EditorApplication.update += OnUpdate;
		}

		private static void OnUpdate()
		{
			foreach (var action in _queue)
			{
				try
				{
					action?.Invoke();
				}
				catch (Exception e)
				{
					LogCat.LogError(e);
				}
			}

			_queue.Clear();
		}


		public static ShellRequest ProcessCommand(string cmd, string workDirectory = StringConst.String_Empty,
			List<string> environmentVars = null)
		{
			ShellRequest shellRequest = new ShellRequest();
			ThreadPool.QueueUserWorkItem(delegate
			{
				Process process = null;
				try
				{
					ProcessStartInfo processStartInfo = new ProcessStartInfo(shellApp);

#if UNITY_EDITOR_OSX
				string splitChar = ":";
				start.Arguments = "-c";
#elif UNITY_EDITOR_WIN
					string splitChar = ";";
					processStartInfo.Arguments = "/c";
#endif

					if (environmentVars != null)
					{
						foreach (string var in environmentVars)
							processStartInfo.EnvironmentVariables["PATH"] += (splitChar + var);
					}

					processStartInfo.Arguments += (" \"" + cmd + " \"");
					processStartInfo.CreateNoWindow = true;
					processStartInfo.ErrorDialog = true;
					processStartInfo.UseShellExecute = false;
					processStartInfo.WorkingDirectory = workDirectory;

					if (processStartInfo.UseShellExecute)
					{
						processStartInfo.RedirectStandardOutput = false;
						processStartInfo.RedirectStandardError = false;
						processStartInfo.RedirectStandardInput = false;
					}
					else
					{
						processStartInfo.RedirectStandardOutput = true;
						processStartInfo.RedirectStandardError = true;
						processStartInfo.RedirectStandardInput = true;
						processStartInfo.StandardOutputEncoding = Encoding.UTF8;
						processStartInfo.StandardErrorEncoding = Encoding.UTF8;
					}

					process = Process.Start(processStartInfo);
					process.ErrorDataReceived += delegate (object sender, DataReceivedEventArgs e)
					{
						LogCat.LogError(EncodingUtil.GBK2UTF8(e.Data));
					};
					process.OutputDataReceived += delegate (object sender, DataReceivedEventArgs e)
					{
						LogCat.LogError(EncodingUtil.GBK2UTF8(e.Data));
					};
					process.Exited += delegate (object sender, EventArgs e) { LogCat.LogError(e.ToString()); };

					bool isHasError = false;
					do
					{
						string line = process.StandardOutput.ReadLine();
						if (line == null)
							break;

						line = line.Replace("\\", "/");
						_queue.Add(delegate () { shellRequest.Log(0, line); });
					} while (true);

					while (true)
					{
						string error = process.StandardError.ReadLine();
						if (string.IsNullOrEmpty(error))
							break;

						isHasError = true;
						_queue.Add(delegate () { shellRequest.Log(LogCatType.Error, error); });
					}

					process.Close();
					if (isHasError)
						_queue.Add(delegate () { shellRequest.Error(); });
					else
						_queue.Add(delegate () { shellRequest.NotifyDone(); });
				}
				catch (Exception e)
				{
					LogCat.LogError(e);
					process?.Close();
				}
			});
			return shellRequest;
		}


		public void AddEnvironmentVars(params string[] vars)
		{
			for (var i = 0; i < vars.Length; i++)
			{
				var var = vars[i];
				if (var.IsNullOrWhiteSpace())
					continue;
				_environmentVarList.Add(var);
			}
		}

		public ShellRequest ProcessCMD(string cmd, string workDirectory)
		{
			return ShellUtil.ProcessCommand(cmd, workDirectory, _environmentVarList);
		}
	}
}