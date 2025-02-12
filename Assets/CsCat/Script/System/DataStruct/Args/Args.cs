using System;
using System.Text;

namespace CsCat
{
	public class Args
	{
		private object[] _args;

		public Args()
		{
		}

		public Args(params object[] args)
		{
			Init(args);
		}

		public Args(object args0, params object[] args)
		{
			Init(args0, args);
		}

		public void Init(params object[] args)
		{
			this._args = args;
		}

		public void Init(object args0, params object[] args)
		{
			int offset = 1;
			object[] _args = new object[args?.Length + offset ?? 0];
			_args[0] = args0;
			if (args != null)
				Array.Copy(args, 0, _args, 1, args.Length);
			this._args = _args;
		}

		public override bool Equals(object obj)
		{
			Args other = (Args) obj;

			if (other == null)
				return false;

			if (this._args == null && other._args == null)
				return true;
			if (this._args == null && other._args != null)
				return false;
			if (this._args != null && other._args == null)
				return false;

			if (this._args.Length == other._args.Length)
			{
				for (int i = 0; i < this._args.Length; i++)
				{
					if (!ObjectUtil.Equals(_args[i], other._args[i]))
						return false;
				}

				return true;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return ObjectUtil.GetHashCode(_args);
		}

		public override string ToString()
		{
			var result = new StringBuilder("(");
			if (this._args == null)
			{
				result.Append(")");
				return result.ToString();
			}

			for (int i = 0; i < _args.Length; i++)
			{
				var arg = _args[i];
				result.Append(arg);
				if (i != _args.Length - 1)
					result.Append(",");
			}

			result.Append(")");
			return result.ToString();
		}

		//    public void OnDespawn()
		//    {
		//      args = null;
		//    }
	}
}