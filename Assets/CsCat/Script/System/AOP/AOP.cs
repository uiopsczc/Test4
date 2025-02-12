using System;
using System.Reflection;

namespace CsCat
{
	/// <summary>
	///   必须在使用的地方才能使用，对于继承有缺陷，在叶子节点使用一下表达式包裹要方法体
	///   [AOP_Test("chen")]//AOP属性  用于AOP处理
	///   public void xxxxxxMethod()
	///   {
	///   using (new AOP(this,xxx,yyy,kkkk))//对AOP的属性处理的调用,xxx,yyy,kkk为该函数的参数
	///   {
	///   xxxxxxMethodBody
	///   }
	///   }
	///   AOPAttribute用法参考  IAOPAttribute的解释定义
	/// </summary>
	public sealed class AOP : IDisposable
	{
		#region field

		/// <summary>
		///   被切面的方法的拥有者
		/// </summary>
		private readonly object _sourceMethodOwner;

		/// <summary>
		///   被切面的方法
		/// </summary>
		private readonly MethodBase sourceMethod;

		/// <summary>
		///   被切面的方法的参数
		/// </summary>
		private readonly object[] _sourceMethodArgs;

		#endregion

		#region ctor

		public AOP(object sourceMethodOwner, params object[] sourceMethodArgs)
		{
			sourceMethod = StackTraceUtil.GetMethodOfFrame(1); //获取被切面的方法
			_sourceMethodOwner = sourceMethodOwner;
			_sourceMethodArgs = sourceMethodArgs;
			AOPHandler.instance.Pre_AOP_Handle(sourceMethodOwner, sourceMethod, sourceMethodArgs);
		}

		#endregion

		#region public sourceMethod

		public void Dispose()
		{
			AOPHandler.instance.Post_AOP_Handle(_sourceMethodOwner, sourceMethod, _sourceMethodArgs);
		}

		//public void HandleException(Exception e)
		//{
		//}
		/*TODO  用TryCatch包含被AOP的方法的方法体,然后调用这里的HandleException 
			  再间接调用AOPAttribute中的HandleException方法，原来和PrePos一样
			即
			using(var aop=new AOP(xxxxx))
			{
			  try
			  {
				方法体
			  }catch(Exception e)
			  {
				aop.HanleExcepiton(e);
			  }			
			}
		*/

		#endregion

		
	}
}