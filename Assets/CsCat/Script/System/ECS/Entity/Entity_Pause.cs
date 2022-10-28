using System;
using System.Collections.Generic;

namespace CsCat
{
	public partial class Entity
	{
		protected bool _isPaused;
		public bool isPaused => _isPaused;

		public virtual void SetIsPaused(bool isPaused)
		{
			if (_isPaused == isPaused)
				return;
			this._isPaused = isPaused;
			SetAllComponentsIsPaused(isPaused);
			_SetIsPaused(isPaused);
		}

		protected virtual void _SetIsPaused(bool isPaused)
		{
		}

		public void SetAllComponentsIsPaused(bool isPaused)
		{
			for (int i = 0; i < componentPoolIndexList.Count; i++)
			{
				var componentPoolObjectIndex = componentPoolIndexList[i];
				var component = _GetComponent(componentPoolObjectIndex);
				component?.SetIsPaused(isPaused);
			}
		}

		void _OnReset_Pause()
		{
			_isPaused = false;
		}

		void _OnDespawn_Pause()
		{
			_isPaused = false;
		}
	}
}