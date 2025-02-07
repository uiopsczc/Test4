using System.Collections.Generic;
using System.Reflection;
using Type = System.Type;

namespace CsCat
{
	public partial class ReflectionUtil
	{
		private static readonly Dictionary<Type, Dictionary<string, Dictionary<object, object>>> _cacheDict =
			new Dictionary<Type, Dictionary<string, Dictionary<object, object>>>();

		private const string _methodInfoString = "methodInfo";
		private const string _defaultMethodInfoSubKey = "methodInfo_sub_key";
		private const string _filedInfoString = "fieldInfo";
		private const string _propertyInfoString = "propertyInfo";
		private const string _splitString = StringConst.String_Underline;

		//////////////////////////////////////////////////////////////////////
		// MethodInfoCache
		//////////////////////////////////////////////////////////////////////
		public static bool IsContainsMethodInfoCache(Type type, string methodName, params Type[] parameterTypes)
		{
			if (!_cacheDict.ContainsKey(type))
				return false;
			string mainKey = _methodInfoString + _splitString + methodName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				return false;
			var subKey = new Args(parameterTypes);
			return _cacheDict[type][mainKey].ContainsKey(subKey);
		}

		public static void SetMethodInfoCache(Type type, string methodName, Type[] parameterTypes,
			MethodInfo methodInfo)
		{
			if (!_cacheDict.ContainsKey(type))
				_cacheDict[type] = new Dictionary<string, Dictionary<object, object>>();
			string mainKey = _methodInfoString + _splitString + methodName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				_cacheDict[type][mainKey] = new Dictionary<object, object>();
			var subKey = new Args(parameterTypes);
			_cacheDict[type][mainKey][subKey] = methodInfo;
		}

		public static MethodInfo GetMethodInfoCache(Type type, string methodName, params Type[] parameterTypes)
		{
			if (!_cacheDict.ContainsKey(type))
				return null;
			string mainKey = _methodInfoString + _splitString + methodName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				return null;
			var subKey = new Args(parameterTypes);
			return _cacheDict[type][mainKey][subKey] as MethodInfo;
		}

		public static bool IsContainsMethodInfoCache2(Type type, string methodName)
		{
			if (!_cacheDict.ContainsKey(type))
				return false;
			string mainKey = _methodInfoString + _splitString + methodName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				return false;
			var subKey = _defaultMethodInfoSubKey;
			return _cacheDict[type][mainKey].ContainsKey(subKey);
		}

		public static void SetMethodInfoCache2(Type type, string methodName, MethodInfo methodInfo)
		{
			if (!_cacheDict.ContainsKey(type))
				_cacheDict[type] = new Dictionary<string, Dictionary<object, object>>();
			string mainKey = _methodInfoString + _splitString + methodName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				_cacheDict[type][mainKey] = new Dictionary<object, object>();
			var subKey = _defaultMethodInfoSubKey;
			_cacheDict[type][mainKey][subKey] = methodInfo;
		}

		public static MethodInfo GetMethodInfoCache2(Type type, string methodName)
		{
			if (!_cacheDict.ContainsKey(type))
				return null;
			string mainKey = _methodInfoString + _splitString + methodName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				return null;
			var subKey = _defaultMethodInfoSubKey;
			return _cacheDict[type][mainKey][subKey] as MethodInfo;
		}

		//////////////////////////////////////////////////////////////////////
		// FieldInfoCache
		//////////////////////////////////////////////////////////////////////
		public static bool IsContainsFieldInfoCache(Type type, string fieldName)
		{
			if (!_cacheDict.ContainsKey(type))
				return false;
			string mainKey = _filedInfoString + _splitString + fieldName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				return false;
			var subKey = StringConst.String_Empty;
			return _cacheDict[type][mainKey].ContainsKey(subKey);
		}

		public static void SetFieldInfoCache(Type type, string fieldName, FieldInfo fieldInfo)
		{
			if (!_cacheDict.ContainsKey(type))
				_cacheDict[type] = new Dictionary<string, Dictionary<object, object>>();
			string mainKey = _filedInfoString + _splitString + fieldName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				_cacheDict[type][mainKey] = new Dictionary<object, object>();
			var subKey = StringConst.String_Empty;
			_cacheDict[type][mainKey][subKey] = fieldInfo;
		}

		public static FieldInfo GetFieldInfoCache(Type type, string fieldName)
		{
			if (!_cacheDict.ContainsKey(type))
				return null;
			string mainKey = _filedInfoString + _splitString + fieldName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				return null;
			var subKey = StringConst.String_Empty;
			return _cacheDict[type][mainKey][subKey] as FieldInfo;
		}

		//////////////////////////////////////////////////////////////////////
		// PropertyInfoCache
		//////////////////////////////////////////////////////////////////////
		public static bool IsContainsPropertyInfoCache(Type type, string propertyName)
		{
			if (!_cacheDict.ContainsKey(type))
				return false;
			string mainKey = _propertyInfoString + _splitString + propertyName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				return false;
			var subKey = StringConst.String_Empty;
			return _cacheDict[type][mainKey].ContainsKey(subKey);
		}

		public static void SetPropertyInfoCache(Type type, string propertyName, PropertyInfo propertyInfo)
		{
			if (!_cacheDict.ContainsKey(type))
				_cacheDict[type] = new Dictionary<string, Dictionary<object, object>>();
			string mainKey = _propertyInfoString + _splitString + propertyName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				_cacheDict[type][mainKey] = new Dictionary<object, object>();
			var subKey = StringConst.String_Empty;
			_cacheDict[type][mainKey][subKey] = propertyInfo;
		}

		public static PropertyInfo GetPropertyInfoCache(Type type, string propertyName)
		{
			if (!_cacheDict.ContainsKey(type))
				return null;
			string mainKey = _propertyInfoString + _splitString + propertyName;
			if (!_cacheDict[type].ContainsKey(mainKey))
				return null;
			var subKey = StringConst.String_Empty;
			return _cacheDict[type][mainKey][subKey] as PropertyInfo;
		}
	}
}