namespace CsCat
{
	public partial class Critter
	{
		////////////////////////////装备////////////////////////
		//获得指定的装备
		public Item[] GetEquips(string id = null)
		{
			return this.oEquips.GetEquips(id);
		}

		//是否有装备
		public bool HasEquips()
		{
			return this.oEquips.HasEquips();
		}

		//获得指定的装备
		public Item[] GetEquipsOfTypes(string type1, string type2 = null)
		{
			return this.oEquips.GetEquipsOfTypes(type1, type2);
		}

		public bool HasEquipsOfTypes(string type1, string type2 = null)
		{
			return this.oEquips.IsHasEquipsOfTypes(type1, type2);
		}

		//获得指定的装备
		public Item GetEquip(string idOrRid)
		{
			return this.oEquips.GetEquip(idOrRid);
		}

		//获得指定类别的装备
		public Item GetEquipOfTypes(string type1, string type2 = null)
		{
			return this.oEquips.GetEquipOfTypes(type1, type2);
		}

		//清除所有装备
		public void ClearEquips()
		{
			this.oEquips.ClearEquips();
		}

		//检测穿上装备
		public bool CheckPutOnEquip(Item equip)
		{
			return this.OnCheckPutOnEquip(equip) && equip.OnCheckPutOnEquip(this);
		}

		// 穿上装备
		public bool PutOnEquip(Item equip)
		{
			var env = equip.GetEnv();
			if (env != null)
			{
				LogCat.error(string.Format("PutOnEquip error:{0} still in {1}", equip, env));
				return false;
			}

			var list = this.oEquips.GetEquips_ToEdit();
			if (list.Contains(equip))
			{
				LogCat.error(string.Format("PutOnEquip error:{0} already put on {1}", this, equip));
				return false;
			}

			if (!(this.OnPutOnEquip(equip) && equip.OnPutOnEquip(this)))
				return false;

			equip.SetEnv(this);
			equip.SetIsPutOn(true);
			list.Add(equip);
			return true;
		}

		public bool CheckTakeOffEquip(Item equip)
		{
			return this.OnCheckTakeOffEquip(equip) && equip.OnCheckTakeOffEquip(this);
		}

		public virtual bool TakeOffEquip(Item equip)
		{
			var list = this.oEquips.GetEquips_ToEdit();
			if (!list.Contains(equip))
			{
				LogCat.error(string.Format("TakeOffEquip error:{0} not contains equip:{1}", this, equip));
				return false;
			}

			if (!(this.OnTakeOffEquip(equip) && equip.OnTakeOffEquip(this)))
				return false;
			list.Remove(equip);
			equip.SetIsPutOn(false);
			equip.SetEnv(null);
			return true;
		}

		//////////////////////OnXXX/////////////////////////////////////
		public virtual bool OnCheckPutOnEquip(Item equip)
		{
			return true;
		}

		public virtual bool OnPutOnEquip(Item equip)
		{
			return true;
		}

		public virtual bool OnCheckTakeOffEquip(Item equip)
		{
			return true;
		}

		public virtual bool OnTakeOffEquip(Item equip)
		{
			return true;
		}
	}
}