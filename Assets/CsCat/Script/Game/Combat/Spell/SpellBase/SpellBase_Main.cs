using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CsCat
{
	public partial class SpellBase
	{
		private List<Unit> _targetUnitList;

		public override void Start()
		{
			base.Start();
			if ("被动".Equals(this.cfgSpellData.type))
				this.CounterIncrease(); // 被动默认不被消耗
			this.CounterIncrease();

			this._targetUnitList = Client.instance.combat.spellManager.RecommendSpellRule(this.sourceUnit,
				this.targetUnit,
				this.cfgSpellData, this.originPosition.Value);
			this.targetUnit = this._targetUnitList.IsNullOrEmpty() ? null : this._targetUnitList[0];
			if (this.IsHasMethod("OnStart"))
				this.InvokeMethod("OnStart", false);
			this.RegisterTriggerSpell();
			this.FireEvent<Unit, Unit, SpellBase>(null, SpellEventNameConst.On_Spell_Start, this.sourceUnit,
				this.targetUnit, this);
			Client.instance.combat.spellManager.UnRegisterListener("onStart", this.sourceUnit, this,
				"RegisterTriggerSpell");
			if (!this.cfgSpellData.actionName.IsNullOrWhiteSpace())
			{
				//      if not self.source_unit.action_dict or
				//      not self.source_unit.action_dict[self.cfgSpellData.action_name] then
				//      Error("action is not find", self.spell_id, self.source_unit.unit_id)
				//      end
				//      self.action = SpellAction.New(self.source_unit.action_dict[self.cfgSpellData.action_name], self.source_unit, self)
				//      self.action:Play()
			}
			else
			{
				this.PlaySpellAnimation();
				if (this.IsHasMethod("OnCast"))
				{
					//起手前摇
					var castTimePct = this.GetAnimationTimePct(this.cfgSpellData.castTime, 0);
					this.RegisterAnimationEvent(castTimePct, "_OnCast");
				}

				//可打断后摇
				var breakTimePct = this.GetAnimationTimePct(this.cfgSpellData.breakTime, 1);
				this.RegisterAnimationEvent(breakTimePct
					, "PassBreakTime");
				if ("触发".Equals(this.cfgSpellData.castType))
				{
					var castTimePct = this.GetAnimationTimePct(this.cfgSpellData.castTime, 0);
					var breakTimePctValue = this.GetAnimationTimePct(this.cfgSpellData.breakTime, 1);
					if (breakTimePctValue < castTimePct)
						LogCat.LogError("技能脱手时间比出手时间快");
					this.RegisterAnimationEvent(breakTimePctValue, "OnSpellAnimationFinished");
				}
			}

			this.CounterDecrease();
		}

		private float GetAnimationTimePct(float time, float defaultValue)
		{
			if (this.cfgSpellData.animationDuration != 0)
				return time / this.cfgSpellData.animationDuration;
			return defaultValue;
		}

		protected void _OnCast()
		{
			if (this.IsHasMethod("OnCast"))
				this.InvokeMethod("OnCast", false);
			this.FireEvent<Unit, Unit, SpellBase>(null, SpellEventNameConst.On_Spell_Cast, this.sourceUnit,
				this.targetUnit, this);
			Client.instance.combat.spellManager.UnRegisterListener("onCast", this.sourceUnit, this,
				"RegisterTriggerSpell");
		}

		protected void RegisterTriggerSpell()
		{
			//注册表里填的技能触发事件，由简单的技能按顺序触发组成复杂的技能
			var newSpellTriggerIds = this.cfgSpellData.newSpellTriggerIds;
			if (newSpellTriggerIds.IsNullOrEmpty())
				return;
			for (var i = 0; i < newSpellTriggerIds.Length; i++)
			{
				var newSpellTriggerId = newSpellTriggerIds[i];
				this.__RegisterTriggerSpell(newSpellTriggerId);
			}
		}

		public void __RegisterTriggerSpell(string newSpellTriggerId)
		{
			var cfgSpellTriggerData = CfgSpellTrigger.Instance.GetById(newSpellTriggerId);
			var triggerType = cfgSpellTriggerData.triggerType;
			triggerType = SpellConst.Trigger_Type_Dict[triggerType];
			var triggerSpellId = cfgSpellTriggerData.triggerSpellId; // 触发的技能id
			var triggerSpellDelayDuration = cfgSpellTriggerData.triggerSpellDelayDuration;
			Action<Unit, Unit, SpellBase> func = (sourceUnit, targetUnit, spell) =>
			{
				//这里可以添加是否满足其它触发条件判断
				if (!this.CheckTriggerCondition(cfgSpellTriggerData, sourceUnit, targetUnit))
					return;
				var triggerArgDict = new Hashtable();
				triggerArgDict["sourceSpell"] = this;
				triggerArgDict["transmitArgDict"] = this.GetTransmitArgDict();
				triggerArgDict["newSpellTriggerId"] = newSpellTriggerId;
				Action triggerFunc = () =>
				{
					//启动技能时需要把新技能需要的参数传进去，如果当前技能没有提供这样的方法，则说明当前技能不能启动目标技能
					Client.instance.combat.spellManager.CastSpell(this.sourceUnit, triggerSpellId, targetUnit,
						triggerArgDict);
				};
				if (triggerSpellDelayDuration > 0)
				{
					this.CounterIncrease();
					this.AddTimer((args) =>
					{
						triggerFunc();
						this.CounterDecrease();
						return false;
					}, triggerSpellDelayDuration);
				}
				else
					triggerFunc();
			};
			Client.instance.combat.spellManager.RegisterListener(triggerType, this.sourceUnit, this,
				"RegisterTriggerSpell",
				new MethodInvoker(func));
		}


		protected bool CheckTriggerCondition(CfgSpellTriggerData cfgSpellTriggerData, Unit sourceUnit,
			Unit targetUnit)
		{
			return true;
		}

		// 需要解决的问题，比如一个技能同时攻击了几个单位，触发了几次on_hit，怎么在回调中知道这个hit是由哪次攻击造成的
		// 定义几种参数类型
		//
		//  SpellBase提供默认参数，具体技能根据自己实际情况重写
		//  1.攻击方向
		//  2.技能基础位置
		////////////////////////////////////////////传递给下一个技能的方法///////////////////////////////////
		public Hashtable GetTransmitArgDict()
		{
			Hashtable result = new Hashtable();
			result["originPosition"] = this.GetOriginPosition();
			result["attackDir"] = this.GetAttackDir();
			return result;
		}

		public Vector3 GetOriginPosition()
		{
			return this.originPosition.GetValueOrDefault(this.sourceUnit.GetPosition());
		}

		public Vector3 GetAttackDir()
		{
			return Vector3.zero;
		}

		public void SwitchAction()
		{
			//    self.action = SpellAction.New(self.source_unit.action_dict[action_name], self.source_unit, self)
			//    self.action:Play()
		}

		// 技能脱手，表示角色释放技能完成，可以做其他动作，但是技能本身可能没有完成，继续运行
		// 比如脱手后子弹任然要飞，打到人才真正结束
		// 使用CounterIncrease()和CounterDecrease()计数来控制真正结束
		public void OnSpellAnimationFinished()
		{
			if (this.isSpellAnimationFinished)
				return;
			this.isSpellAnimationFinished = true;
			Client.instance.combat.spellManager.OnSpellAnimationFinished(this);
			if (this.counter._count <= 0)
				this.RemoveSelf();
			if (this.cfgSpellData.isCanMoveWhileCast && this.sourceUnit != null && !this.sourceUnit.IsDead())
				this.sourceUnit.SetIsMoveWithMoveAnimation(true);
		}

		public void Break()
		{
			this.StopSpellAnimation();
			this.OnSpellAnimationFinished();
		}

		//子类添加 FilterUnit 函数可以自定义过滤掉不需要的目标
		protected bool FilterUnit(Unit unit, string spellId, Unit targetUnit, CfgSpellTriggerData cfgSpellTriggerData)
		{
			return true;
		}

		protected void OnMissileReach(EffectEntity missileEffect)
		{
			this.FireEvent<Unit, EffectEntity, SpellBase>(null, SpellEventNameConst.On_Missile_Reach, this.sourceUnit,
				missileEffect, this);
			this.CounterDecrease();
		}
	}
}