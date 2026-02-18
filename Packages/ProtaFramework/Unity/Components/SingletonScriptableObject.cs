using System;
using System.Reflection;
using UnityEngine;

namespace Prota.Unity
{
	public abstract class SingletonScriptableObject<T> : ScriptableObject
		where T: SingletonScriptableObject<T>
	{
		protected static T _instance;
		static string _cachedPath;
		
		public static T instance
		{
			get
			{
				if (_instance is not null) return _instance;
				_instance = Resources.Load<T>(GetPath());
				if(_instance == null)
				{
					throw new Exception("unable to load singleton " + typeof(T));
				}
				return _instance;
			}
		}
		
		public static bool exists
		{
			get
			{
				if(_instance != null) return true;
				_instance = Resources.Load<T>(GetPath());
				return _instance != null;
			}
		}

		static string GetPath()
		{
			if (!string.IsNullOrEmpty(_cachedPath)) return _cachedPath;

			const BindingFlags bindingFlags =
				BindingFlags.Public |
				BindingFlags.NonPublic |
				BindingFlags.Static |
				BindingFlags.FlattenHierarchy;

			var field = typeof(T).GetField("path", bindingFlags);
			
			if (field == null)
				throw new InvalidOperationException($"{typeof(T).FullName} must declare: public const string path = \"...\"; (or static readonly string path)");

			if (field.FieldType != typeof(string))
				throw new InvalidOperationException($"{typeof(T).FullName}.path must be a string.");

			_cachedPath = field.IsLiteral ? (string)field.GetRawConstantValue() : (string)field.GetValue(null);
			if (string.IsNullOrEmpty(_cachedPath))
				throw new InvalidOperationException($"{typeof(T).FullName}.path is null or empty.");
			
			return _cachedPath;
		}
	}
}
