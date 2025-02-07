using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;

namespace CsCat
{
	public partial class LinkedDictionary<K, V> : Dictionary<K, V>, IGetLinkedHashtable
	{
		private readonly List<K> _keyList = new List<K>();
		private readonly List<V> _valueList = new List<V>();
		private DictionaryEnumerator<K, V> __enumerator;
		LinkedHashtable _table = new LinkedHashtable();

		private DictionaryEnumerator<K, V> _enumerator =>
			__enumerator ?? (__enumerator = new DictionaryEnumerator<K, V>(_keyList, _valueList));

		public new List<K> Keys => this._keyList;
		public new List<V> Values => this._valueList;

		public new V this[K key]
		{
			get => base[key];
			set => Put(key, value);
		}


		#region override method

		public new void Add(K key, V value)
		{
			_keyList.Add(key);
			_valueList.Add(value);
			base.Add(key, value);
		}

		public new void Clear()
		{
			_keyList.Clear();
			_valueList.Clear();
			base.Clear();
		}

		public new void Remove(K key)
		{
			int index = _keyList.IndexOf(key);
			if (index == -1) return;
			_keyList.RemoveAt(index);
			_valueList.RemoveAt(index);
			base.Remove(key);
		}

		public new DictionaryEnumerator<K, V> GetEnumerator()
		{
			_enumerator.Reset();
			return _enumerator;
		}

		#endregion


		public void Put(K key, V value)
		{
			int index = _keyList.IndexOf(key);
			//删除原来的
			if (index != -1)
			{
				_keyList.RemoveAt(index);
				_valueList.RemoveAt(index);
			}

			_keyList.Add(key);
			_valueList.Add(value);
			//然后放到最后
			base[key] = value;
		}

		public void Sort(Func<K, K, bool> compareFunc)
		{
			_keyList.QuickSort(compareFunc);
			_valueList.Clear();
			for (var i = 0; i < _keyList.Count; i++)
			{
				var key = _keyList[i];
				_valueList.Add(this[key]);
			}
		}


		public LinkedHashtable GetLinkedHashtable()
		{
			_table.Clear();
			for (var i = 0; i < Keys.Count; i++)
			{
				var key = Keys[i];
				_table[key] = this[key];
			}

			return _table;
		}
	}

}