using UnityEngine;

namespace CsCat
{
	public partial class UIGuidePanelBase
	{
		public class FingerItem : UIObject
		{
			protected void _Init(GameObject gameObject)
			{
				base._Init();
				_SetGameObject(gameObject, true);
			}
		}
	}
}