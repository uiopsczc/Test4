using UnityEngine;

namespace CsCat
{
	public partial class EffectGraphicComponent : GraphicComponent
	{
		public EffectEntity effectEntity => this.GetEntity<EffectEntity>();

		protected void _Init()
		{
			base._Init();
			base.Init(Client.instance.combat.effectManager.resLoadComponent);
		}


		public override void OnAllAssetsLoadDone()
		{
			base.OnAllAssetsLoadDone();
			ApplyToTransform(this.effectEntity.TransformInfoComponent.position, this.effectEntity.TransformInfoComponent.eulerAngles);
		}

		public void ApplyToTransform(Vector3? position, Vector3? eulerAngles)
		{
			if (this.transform == null)
				return;
			if (position != null)
				transform.position = position.Value;
			if (eulerAngles != null)
				transform.eulerAngles = eulerAngles.Value;
		}

		public override void DestroyGameObject()
		{
			if (this._gameObject != null)
				this._gameObject.Despawn();
		}

		public GameObjectPoolCat GetEffectGameObjectPool(GameObject prefab)
		{
			return PoolCatManagerUtil.GetOrAddGameObjectPool(GetEffectGameObjectPoolName(), prefab,
			  EffectManagerConst.Pool_Name + "/" + effectEntity.effectId);
		}

		public string GetEffectGameObjectPoolName()
		{
			return EffectManagerConst.Pool_Name + "_" + effectEntity.effectId;
		}

		public override GameObject InstantiateGameObject(GameObject prefab)
		{
			return GetEffectGameObjectPool(prefab).SpawnGameObject();
		}

		public override bool IsCanUpdate()
		{
			return this._gameObject != null && base.IsCanUpdate();
		}

		protected override void _Update(float deltaTime = 0, float unscaledDeltaTime = 0)
		{
			base._Update(deltaTime, unscaledDeltaTime);
			ApplyToTransform(this.effectEntity.TransformInfoComponent.position, this.effectEntity.TransformInfoComponent.eulerAngles);
		}
	}
}