namespace CsCat
{
	public enum AssetBundleExportType
	{
		/// <summary>
		/// 普通素材，被根素材依赖的
		/// </summary>
		Asset = 1,

		/// <summary>
		/// 根
		/// </summary>
		Root = 1 << 1,

		/// <summary>
		/// 需要单独打包，说明这个素材是被两个或以上的素材依赖的
		/// </summary>
		Standalone = 1 << 2,
	}
}