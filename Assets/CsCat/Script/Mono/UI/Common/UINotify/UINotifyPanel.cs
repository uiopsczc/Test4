using DG.Tweening;
using UnityEngine.UI;

namespace CsCat
{
	public class UINotifyPanel : UIPanel
	{
		public override EUILayerName layerName => EUILayerName.NotifyUILayer;


		private Text _TxtC_Desc;
		private Image _ImgC_Bg;


		private string _desc;
		private bool isMovingUp;
		private bool _isRised;
		private float _position;
		private bool _isCreated;

		private float _moveUpDelayDuration = 1f;
		private float _closeDelayDuration = 2.8f;

		protected void Init(string desc)
		{
			base._Init();
			this.AddChild<TimerDictTreeNode>(null, new TimerDict(Client.instance.timerManager));
			this._desc = desc;
			SetPrefabPath("Assets/Resources/common/ui/prefab/UINotifyPanel.prefab");
		}

		protected override void _InitGameObjectChildren()
		{
			base._InitGameObjectChildren();
			_TxtC_Desc = this.GetTransform().Find("TxtC_Desc").GetComponent<Text>();
			_ImgC_Bg = this.GetTransform().Find("ImgC_Bg").GetComponent<Image>();
		}

		protected override void _PostSetGameObject()
		{
			base._PostSetGameObject();
			this._AddTimers();
			this._isCreated = true;
			if (this._isRised)
				Rise();
		}

		protected override void _AddTimers()
		{
			base._AddTimers();
			var timerDictTreeNode = this.GetChild<TimerDictTreeNode>();
			timerDictTreeNode.AddTimer(MoveUp, this._moveUpDelayDuration);
			timerDictTreeNode.AddTimer((args) =>
			{
				this.Close();
				return false;
			}, this._closeDelayDuration);
		}

		protected override bool _Refresh(bool isInit = false)
		{
			if (!base._Refresh(isInit))
				return false;
			_TxtC_Desc.text = _desc;
			return true;
		}

		public bool MoveUp(params object[] args)
		{
			this.GetTransform().DOBlendableMoveYBy(5, 1);
			_TxtC_Desc.DOFade(0, 1);
			_ImgC_Bg.DOFade(0, 1);
			isMovingUp = true;
			return false;
		}

		public void Rise()
		{
			if (!_isCreated)
			{
				_isRised = true;
				this._position = this._position + 0.5f;
				return;
			}

			if (this.isMovingUp)
				return;

			this.GetTransform().DOBlendableMoveYBy(this._position + 0.5f, 0.2f);
			this._position = 0;

		}

		protected override void _Destroy()
		{
			isMovingUp = false;
			_isRised = false;
			_isCreated = false;
			base._Destroy();
		}
	}
}