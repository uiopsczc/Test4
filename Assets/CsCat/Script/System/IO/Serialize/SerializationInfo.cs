using System;
using System.Collections;

namespace CsCat
{
	public struct SerializationInfo
	{
		private readonly ArrayList _list;
		private readonly object _context;


		internal SerializationInfo(ArrayList list, object context)
		{
			this._list = list;
			this._context = context;
		}


		public T GetValue<T>(string name)
		{
			var value = GetValue(name, typeof(T));
			if (value == null) return default;
			return (T)value;
		}

		public object GetValue(string name, Type type)
		{
			var count = _list.Count;
			for (var i = 0; i < count; i++)
			{
				var enumerator = (_list[i] as Hashtable).GetEnumerator();
				enumerator.MoveNext();
				var key = enumerator.Key.ToString();
				if (name == key) return JsonSerializer.Deserialize(enumerator.Value, type, _context);
			}

			return null;
		}

		public void SetValue(string name, object value)
		{
			if (value == null) return;
			var value2 = JsonSerializer.SerializeObject(value, _context);
			var hashtable = new Hashtable { [name] = value2 };
			_list.Add(hashtable);
		}
	}
}