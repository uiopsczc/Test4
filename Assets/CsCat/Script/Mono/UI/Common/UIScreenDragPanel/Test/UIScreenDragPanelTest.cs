namespace CsCat
{
	public static class UIScreenDragPanelTest
	{

		public static void Test()
		{
			UIScreenDragPanel panel = Client.instance.uiManager.CreateChildPanel(null, default(UIScreenDragPanel), null, false, (object)null);
			panel.SetIsEnabled(false);
		}
	}
}