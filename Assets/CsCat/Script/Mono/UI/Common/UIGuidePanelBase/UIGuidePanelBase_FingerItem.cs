using UnityEngine;

namespace CsCat
{
	public partial class UIGuidePanelBase
	{
		public class FingerItem : UIObject
		{
			protected void Init(GameObject gameObject)
			{
				base._Init();
				SetGameObject(gameObject, true);
			}
		}
	}
}