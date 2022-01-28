using System.Collections;

namespace CsCat
{
	public static class EmbedsTest
	{
		public static void Test()
		{
			User user = Client.instance.user;
			user.AddItems("5", 5);
			user.AddItems("6", 6);
			user.PutOnEquip("6", user.mainRole);
			user.EmbedOn(user.mainRole.GetEquipOfTypes("装备", "武器"), "5");
			//    user.EmbedOff(user.main_role.GetEquipOfTypes("装备", "武器"), "5");

			var dict = new Hashtable();
			var dictTmp = new Hashtable();
			user.DoSave(dict, dictTmp);
			LogCat.log(dict, dictTmp);
		}


	}
}