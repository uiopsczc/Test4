using System;
using UnityEngine;
using UnityEngine.UI;

namespace CsCat
{
	public partial class UIGuidePanelBase
	{
		public class BgItem : UIObject
		{
			public Image image;
			public Button button;

			protected void Init(GameObject gameObject)
			{
				base._Init();
				SetGameObject(gameObject, true);
			}

			protected override void InitGameObjectChildren()
			{
				base.InitGameObjectChildren();
				image = this.GetGameObject().GetComponent<Image>();
				button = this.GetGameObject().GetComponent<Button>();
			}

			public void Show(bool isClickable = true, Action<UIPanel> clickCallback = null, bool isVisible = true)
			{
				if (!isVisible)
					image.SetAlpha(0.007f);
				if (isClickable)
				{
					if (clickCallback == null)
						this.parentUIPanel.RegisterOnClick(button, () => { parentUIPanel.Close(); });
					else
						this.parentUIPanel.RegisterOnClick(button, () => { clickCallback(parentUIPanel); });
				}

			}
		}
	}
}