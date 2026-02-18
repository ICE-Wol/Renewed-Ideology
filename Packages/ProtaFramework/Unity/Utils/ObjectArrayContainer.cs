using System;
using Prota.Unity;
using UnityEngine;

namespace Prota.Unity
{

	// 管理基于模板的 MonoBehaviour 元素数组的通用类
	// T: MonoBehaviour 类型的元素
	[Serializable]
	public class ObjectArrayContainer<T> where T : Component
	{
		public T[] items = Array.Empty<T>();
		
		private T template;
		private Transform container;
		
		public T this[int index] => items[index];

		public int count => items.Length;
		
		public ObjectArrayContainer(T template, Transform container)
		{
			this.template = template;
			this.container = container;
			
			if(!template.gameObject.IsPrefab())
			{
				template.gameObject.SetActive(false);
			}
		}
		
		// 清理所有元素
		public void Clear()
		{
			foreach (var item in items)
			{
				if (item != null) UnityEngine.Object.Destroy(item.gameObject);
			}
			items = Array.Empty<T>();
		}
		
		// 重建数组（总是重新创建）
		// count: 要创建的元素数量
		// initAction: 初始化每个元素的回调函数，参数为 (item, index)
		public void Rebuild(int count, Action<T, int> initAction = null)
		{
			Clear();
			
			items = new T[count];
			for (int i = 0; i < count; i++)
			{
				items[i] = UnityEngine.Object.Instantiate(template, container);
				items[i].gameObject.SetActive(true);
				initAction?.Invoke(items[i], i);
			}
		}
		
		// 重建数组（总是重新创建），使用数据数组
		// data: 数据数组，数组长度决定创建的元素数量
		// initAction: 初始化每个元素的回调函数，参数为 (item, dataItem)
		public void Rebuild<TData>(TData[] data, Action<T, TData> initAction = null)
		{
			if (data == null)
			{
				Rebuild(0);
				return;
			}
			
			Clear();
			
			items = new T[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				items[i] = UnityEngine.Object.Instantiate(template, container);
				items[i].gameObject.SetActive(true);
				initAction?.Invoke(items[i], data[i]);
			}
		}

		public T Add(Action<T> initAction = null)
		{
			var newItem = UnityEngine.Object.Instantiate(template, container);
			newItem.gameObject.SetActive(true);
			initAction?.Invoke(newItem);
			
			var list = new System.Collections.Generic.List<T>(items);
			list.Add(newItem);
			items = list.ToArray();
			
			return newItem;
		}

		public void Remove(T item)
		{
			if (item == null) return;
			UnityEngine.Object.Destroy(item.gameObject);
			
			var list = new System.Collections.Generic.List<T>(items);
			list.Remove(item);
			items = list.ToArray();
		}
	}

}
