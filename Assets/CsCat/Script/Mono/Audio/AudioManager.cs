using System.Collections.Generic;
using UnityEngine;

namespace CsCat
{
	public class AudioManager : TickObject
	{
		private AudioListener _audioListener;
		private Transform _audioSourceContainerTransform;
		private AudioSource _bgmAudioSource;
		private Transform _bgmContainerTransform;
		public string lastBGMPath;
		private readonly List<AudioSource> _playingAudioSourceList = new List<AudioSource>();

		public override void Init()
		{
			base.Init();
			var gameObject = GameObject.Find("AudioManager");
			graphicComponent.SetGameObject(gameObject, true);
			_audioListener = graphicComponent.transform.Find("AudioListener").GetComponent<AudioListener>();
			_audioSourceContainerTransform = graphicComponent.transform.Find("AudioSourceContainer");
			_bgmContainerTransform = graphicComponent.transform.Find("BGMContainer");
			_bgmAudioSource = _bgmContainerTransform.GetComponent<AudioSource>();
			_bgmAudioSource.SetAudioMixerOutput("bgm");
			SetGroupVolume("Master", GameData.instance.audioData.volume);
		}

		protected override void _Update(float deltaTime = 0, float unscaledDeltaTime = 0)
		{
			base._Update(deltaTime, unscaledDeltaTime);
			for (var i = _playingAudioSourceList.Count - 1; i >= 0; i--)
			{
				var playingAudioSource = _playingAudioSourceList[i];
				if (!playingAudioSource.isPlaying)
				{
					_playingAudioSourceList.RemoveAt(i);
					playingAudioSource.gameObject.Destroy();
					return; //每次只删除一个
				}
			}
		}

		// value [0,1]
		public void SetGroupVolume(string groupName, float value)
		{
			value = Mathf.Clamp01(value);
			var decibel = Mathf.Lerp(-80, 0, Mathf.Pow(value, 0.3f)); //分贝与真实听到的声音并不是线性的
			SetGroupDecibel(groupName, decibel);
		}

		// value [-80,0] 分贝
		public void SetGroupDecibel(string groupName, float decibel)
		{
			decibel = Mathf.Clamp(decibel, -80f, 0f);
			SingletonMaster.instance.audioMixer.SetFloat(AudioMixerConst.Group_Dict[groupName].volumeName, decibel);
		}


		// value [0,1]
		public void SetAllGroupVolume(float value)
		{
			foreach (var keyValue in AudioMixerConst.Group_Dict)
			{
				var groupName = keyValue.Key;
				SetGroupVolume(groupName, value);
			}
		}

		public void SetAudioListenerPosition(Vector3 position)
		{
			_audioListener.transform.position = position;
		}

		public void PlaySound(string assetPath, string groupName, GameObject target, Vector3? targetLocalPosition,
			bool isLoop = false,
			float? range = null)
		{
			resLoadComponent.GetOrLoadAsset(assetPath,
				assetCat =>
				{
					_OnSoundLoadSuccess(assetCat.Get<AudioClip>(assetPath.GetSubAssetPath()), groupName, target,
						targetLocalPosition,
						isLoop, range);
				}, null, null, this);
		}

		private void _OnSoundLoadSuccess(AudioClip audioClip, string groupName, GameObject target,
			Vector3? targetLocalPosition,
			bool isLoop = false, float? range = null)
		{
			//如果没有target，音频挂AudioMgr上
			target = target ?? _audioSourceContainerTransform.gameObject;
			// 如果有pos，生成原地音频
			if (targetLocalPosition != null)
			{
				var clone = new GameObject("AudioSource");
				clone.transform.SetParent(target.transform);
				clone.transform.localPosition = targetLocalPosition.Value;
				target = clone;
			}

			var audioSource = _GetAudioSource(target);

			if (targetLocalPosition != null)
				_playingAudioSourceList.Add(audioSource);

			if (range != null)
			{
				audioSource.spatialBlend = 1; //0表示2D，1表示3D
				audioSource.minDistance = range.Value / 2;
				audioSource.maxDistance = range.Value;
			}
			else
			{
				audioSource.spatialBlend = 0; //0表示2D，1表示3D
			}

			audioSource.clip = audioClip;
			audioSource.SetAudioMixerOutput(groupName);
			audioSource.loop = isLoop;
			audioSource.playOnAwake = false;
			audioSource.Play();
		}

		private AudioSource _GetAudioSource(GameObject target)
		{
			//选择AudioSource，如果attachGmaeobj上有不在播放的AudioSource，
			//使用其播放，没有则创建新AudioSource
			var targetAudioSources = target.GetComponents<AudioSource>();
			for (var i = 0; i < targetAudioSources.Length; i++)
				if (!targetAudioSources[i].isPlaying)
					return targetAudioSources[i];
			return target.AddComponent<AudioSource>();
		}


		public void PlayUISound(string soundPath)
		{
			PlaySound(soundPath, "ui", null, null, false);
		}

		public void PlayBGMSound(string soundPath, bool isLoop = true)
		{
			if (soundPath.Equals(lastBGMPath))
				return;

			resLoadComponent.GetOrLoadAsset(soundPath,
				assetCat =>
				{
					_OnBGMLoadSuccess(assetCat.Get<AudioClip>(soundPath.GetSubAssetPath()), isLoop);
					lastBGMPath = soundPath;
				}, null, null, this);
		}

		public void PauseBGMSound(bool isPaused = true)
		{
			if (isPaused)
				_bgmAudioSource.Pause();
			else
				_bgmAudioSource.UnPause();
		}

		public void StopBGMSound()
		{
			_bgmAudioSource.Stop();
		}


		private void _OnBGMLoadSuccess(AudioClip audioClip, bool isLoop)
		{
			var audioSource = _bgmAudioSource;
			audioSource.clip = audioClip;
			audioSource.loop = isLoop;
			audioSource.playOnAwake = false;
			audioSource.Play();
		}
	}
}