using UnityEngine;

namespace CsCat
{
	public partial class UIGuidePanelBase
	{
		public class FingerItem : UIObject
		{
			public void Init(GameObject gameObject)
			{
				base.Init();
				graphicComponent.SetGameObject(gameObject, true);
			}
		}
	}
}