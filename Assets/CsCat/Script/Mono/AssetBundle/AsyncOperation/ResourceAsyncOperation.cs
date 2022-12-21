using System;
using System.Collections;

namespace CsCat
{
	public abstract class ResourceAsyncOperation : GameEntity, IEnumerator
	{
		public ResultInfo _resultInfo;


		public ResultInfo resultInfo
		{
			get
			{
				if (_resultInfo != null) return _resultInfo;
				_resultInfo = PoolCatManagerUtil.Spawn<ResultInfo>();
				_resultInfo.Init(_OnSuccess, _OnFail, _OnDone);
				return _resultInfo;
			}
		}
		public float progress => _GetProgress();
		public object Current => null;


		public Action onSuccessCallback;
		public Action onFailCallback;
		public Action onDoneCallback;




		public bool MoveNext()
		{
			return !resultInfo.isDone;
		}

		public abstract void Update();

		protected virtual void _OnSuccess()
		{
			this.onSuccessCallback?.Invoke();
			this.onSuccessCallback = null;
		}

		protected virtual void _OnFail()
		{
			this.onFailCallback?.Invoke();
			this.onFailCallback = null;
		}

		protected virtual void _OnDone()
		{
			this.onDoneCallback?.Invoke();
			this.onDoneCallback = null;
		}


		protected abstract float _GetProgress();

		public virtual long GetNeedDownloadBytes()
		{
			return 1;
		}

		public virtual long GetDownloadedBytes()
		{
			return 1;
		}

		public void Reset()
		{
			onSuccessCallback = null;
			onFailCallback = null;
			onDoneCallback = null;
			if (_resultInfo != null)
				PoolCatManagerUtil.Despawn(_resultInfo);
			_resultInfo = null;
		}

		public override void Despawn()
		{
			base.Despawn();
			Reset();
		}
	}
}