using System.Collections.Generic;
using UnityEngine;

namespace CsCat
{
	public class ViewTreeNode : TreeNode
	{
		protected TransformInfoProxy _transformInfoProxy = new TransformInfoProxy();

		protected override void _Init()
		{
			base._Init();
			this.AddChild<ListenerDictTreeNode>(null);
			this.AddChild<ResLoadDictTreeNode>(null, new ResLoadDict(new ResLoad()));
		}

		public void SetParentTransform(Transform parentTransform)
		{
			this._transformInfoProxy.SetParentTransform(parentTransform);
		}

		public Transform GetTransform()
		{
			return this._transformInfoProxy.GetTransform();
		}

		public GameObject GetGameObject()
		{
			return this._transformInfoProxy.GetGameObject();
		}

		public RectTransform GetRectTransform()
		{
			return this._transformInfoProxy.GetRectTransform();
		}

		protected void ApplyToTransform(Transform toApplyTransform)
		{
			_transformInfoProxy.ApplyToTransform(toApplyTransform);
		}

		public void SetLocalPosition(Vector3 localPosition)
		{
			_transformInfoProxy.SetLocalPosition(localPosition);
		}

		public Vector3 GetLocalPosition()
		{
			return _transformInfoProxy.GetLocalPosition();
		}


		public void SetLocalEulerAngles(Vector3 localEulerAngles)
		{
			_transformInfoProxy.SetLocalEulerAngles(localEulerAngles);
		}

		public Vector3 GetLocalEulerAngles()
		{
			return _transformInfoProxy.GetLocalEulerAngles();
		}


		public void SetLocalRotation(Quaternion localRotation)
		{
			_transformInfoProxy.SetLocalRotation(localRotation);
		}

		public Quaternion GetLocalRotation()
		{
			return _transformInfoProxy.GetLocalRotation();
		}


		public void SetLocalScale(Vector3 localScale)
		{
			_transformInfoProxy.SetLocalScale(localScale);
		}

		public Vector3 GetLocalScale()
		{
			return _transformInfoProxy.GetLocalScale();
		}

		public void SetPosition(Vector3 position)
		{
			_transformInfoProxy.SetPosition(position);
		}

		public Vector3 GetPosition()
		{
			return _transformInfoProxy.GetPosition();
		}

		public void SetEulerAngles(Vector3 eulerAngles)
		{
			_transformInfoProxy.SetEulerAngles(eulerAngles);
		}


		public Vector3 GetEulerAngles()
		{
			return _transformInfoProxy.GetEulerAngles();
		}


		public void SetRotation(Quaternion rotation)
		{
			_transformInfoProxy.SetRotation(rotation);
		}


		public Quaternion GetRotation()
		{
			return _transformInfoProxy.GetRotation();
		}

		public void SetScale(Vector3 scale)
		{
			_transformInfoProxy.SetScale(scale);
		}


		public Vector3 GetScale()
		{
			return _transformInfoProxy.GetScale();
		}


		public bool SetIsShow(bool isShow)
		{
			var isChange = _transformInfoProxy.SetIsShow(isShow);
			if (isChange)
			{
				if (isShow)
					_OnShow();
				else
					_OnHide();
			}
			return isChange;
		}


		public bool IsShow()
		{
			return this._transformInfoProxy.IsShow();
		}

		protected virtual void _OnShow()
		{

		}

		protected virtual void _OnHide()
		{

		}

		protected override void _Reset()
		{
			base._Reset();
			this.SetIsShow(false);
		}

		protected override void _PostReset()
		{
			base._PostReset();
			this._transformInfoProxy.Reset();
		}

		protected override void _Destroy()
		{
			base._Destroy();
			this.SetIsShow(false);
		}

		protected override void _PostDestroy()
		{
			base._PostDestroy();
			this._transformInfoProxy.Reset();
		}
	}
}