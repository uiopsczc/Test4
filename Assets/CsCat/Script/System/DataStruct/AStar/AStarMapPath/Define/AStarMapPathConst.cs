namespace CsCat
{
	public class AStarMapPathConst
	{
		//屏幕大小
		public static int Client_View_Width_Grid_Count = 2;
		public static int Client_View_Height_Grid_Count = 2;

		//障碍类型相关 数值0是保留位(即第一位)
		public const int Invalid_Obstacle_Types = 0xff; // 无效区域的障碍类型 0表示不能通过，1表示可以通过

		//障碍类型是 从格子类型的后3位， 范围[0,2^3-1]即[0,7]
		public static int[] Critter_Can_Pass_Obstacle_Types =
		{
			1, //保留位,对应int的数值是0
			1, //正常道路,对应int的数值是1
			1, //遮挡,即走在这里的话，该生物半透明显示，看起来该生物被其他东西遮挡,对应int的数值是2
			0, //低障碍,对应int的数值是3
			0, //高障碍,对应int的数值是4
			0, //未定义,对应int的数值是5
			0, //未定义,对应int的数值是6
			0, //填充区域,对应int的数值是7
		};

		public static int[] Skill_Can_Pass_Obstacle_Types = {1, 1, 1, 1, 0, 0, 0, 0}; // skill默认可通过障碍 0表示不能通过，1表示可以通过

		/////////////////////////////////////////////////////0, 1, 2, 3, 4, 5, 6, 7  对应的int数值
		public static int[] Air_Can_Pass_Obstacle_Types = {1, 1, 1, 1, 1, 1, 1, 1}; // 空中可通过障碍 0表示不能通过，1表示可以通过


		//地形类型相关 数值0是保留位(即第一位)
		//地形类型是 从格子类型的后3位向前5位，  范围[0,2^5-1]即[0,31]
		// 技能可通过的地形  0表示不能通过，1表示可以通过
		public static int[] Skill_Can_Pass_Terrain_Types =
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}; //31个

		////////////////////////////////////////////////////0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31 //对应的int数值  对应的int数值
		// 玩家可通过的地形 0表示不能通过，1表示可以通过
		public static int[] User_Can_Pass_Terrain_Types =
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};

		// 空中可通过的地形 0表示不能通过，1表示可以通过
		public static int[] Air_Can_Pass_Terrain_Types =
			{1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};

		// 巡视
		public static int Random_Move_Distance_Min = 1; // 巡视走动最小距离
		public static int Random_Move_Distance_Max = 10; // 巡视走动最大距离
	}
}