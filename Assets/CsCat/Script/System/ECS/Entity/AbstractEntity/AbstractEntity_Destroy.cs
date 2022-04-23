using System;
using System.Collections.Generic;

namespace CsCat
{
	public partial class AbstractEntity
	{
		private bool _isDestroyed;
		public Action preDestroyCallback;
		public Action postDestroyCallback;

		public bool IsDestroyed()
		{
			return _isDestroyed;
		}

		public void DoDestroy()
		{
			if (IsDestroyed())
				return;
			PreDestroy();
			Destroy();
			PostDestroy();
		}

		protected void PreDestroy()
		{
			preDestroyCallback?.Invoke();
			preDestroyCallback = null;
		}

		protected void Destroy()
		{
			RemoveAllChildren();
			SetIsEnabled(false, false);
			SetIsPaused(false, false);
			RemoveAllComponents();
			_Destroy();
			_isDestroyed = true;
			cache.Clear();
		}

		protected virtual void _Destroy()
		{
		}

		protected void PostDestroy()
		{
			postDestroyCallback?.Invoke();
			postDestroyCallback = null;
		}

		

		void _OnDespawn_Destroy()
		{
			_isDestroyed = false;
			preDestroyCallback = null;
			postDestroyCallback = null;
		}
	}
}