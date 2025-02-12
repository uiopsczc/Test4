using System.Collections;

namespace CsCat
{
	//引用计数、不缓存,自己主动管理AB的回收unload(true)
	public partial class AssetBundleManager
	{
		// 从服务器下载网页内容，需提供完整url，非AB（不计引用计数、不缓存），Creater使用后记得回收
		public ResourceWebRequester DownloadFileAsyncNoCache(string url)
		{
			var resourceWebRequester = PoolCatManager.Default.SpawnValue<ResourceWebRequester>(null, null);
			resourceWebRequester.DoInit(url, true);
			resourceWebRequesterAllDict[url] = resourceWebRequester;
			_resourceWebRequesterWaitingQueue.Enqueue(resourceWebRequester);
			return resourceWebRequester;
		}

		// 从资源服务器下载非Assetbundle资源，非AB（不计引用计数、不缓存），Creater使用后记得回收
		public ResourceWebRequester DownloadFileAsyncNoCache(string downloadURL, string filePath)
		{
			var resourceWebRequester = PoolCatManager.Default.SpawnValue<ResourceWebRequester>(null, null);
			var url = downloadURL + filePath;
			resourceWebRequester.DoInit(url, true);
			resourceWebRequesterAllDict[url] = resourceWebRequester;
			_resourceWebRequesterWaitingQueue.Enqueue(resourceWebRequester);
			return resourceWebRequester;
		}

		//max_reload_count 失败的重新load的最大次数
		public IEnumerator DownloadFileAsync(string url, int maxReloadCount, int curReloadCount = 0)
		{
			var resourceWebRequester = DownloadFileAsyncNoCache(url);
			curReloadCount++;
			yield return resourceWebRequester;
			while (curReloadCount < maxReloadCount)
			{
				if (!resourceWebRequester.error.IsNullOrWhiteSpace())
				{
					LogCat.LogError(resourceWebRequester.error);
					resourceWebRequester.DoDestroy();
					PoolCatManager.Default.DespawnValue(resourceWebRequester);
					resourceWebRequester = DownloadFileAsyncNoCache(url);
					yield return resourceWebRequester;
				}
				curReloadCount++;
			}
		}

		//max_reload_count 失败的重新load的最大次数
		public IEnumerator DownloadFileAsync(string downloadURL, string filePath, int maxReloadCount, int curReloadCount = 0)
		{
			var resourceWebRequester = DownloadFileAsyncNoCache(downloadURL, filePath);
			curReloadCount++;
			yield return resourceWebRequester;
			while (curReloadCount < maxReloadCount)
			{
				if (!resourceWebRequester.error.IsNullOrWhiteSpace())
				{
					LogCat.LogError(resourceWebRequester.error);
					resourceWebRequester.DoDestroy();
					PoolCatManager.Default.DespawnValue(resourceWebRequester);
					resourceWebRequester = DownloadFileAsyncNoCache(downloadURL, filePath);
					yield return resourceWebRequester;
				}
				curReloadCount++;
			}
		}
	}
}