using System;
using UnityEngine;
using UnityEngine.Jobs;

namespace Prota.Unity
{
	public class TransformIndexedCollectionGameObject : TransformIndexedCollection<GameObject>
	{
		public TransformIndexedCollectionGameObject(int capacity) : base(capacity, x => x.transform)
		{
			
		}
	}
	
	public class TransformIndexedCollectionComponent<T> : TransformIndexedCollection<T>
		where T : Component
	{
		public TransformIndexedCollectionComponent(int capacity) : base(capacity, x => x.transform)
		{
			
		}
	}
	
    public class TransformIndexedCollection<T> : IDisposable
        where T : class
    {
        readonly DenseSet<T> _items;
        readonly Func<T, Transform> _getTransform;
        TransformAccessArray _transforms;

        public int Count => _items.Count;

        public TransformAccessArray transforms => _transforms;

        public T this[int index] => _items[index];

        public TransformIndexedCollection(int capacity, Func<T, Transform> getTransform)
        {
            _items = new DenseSet<T>(capacity);
            _getTransform = getTransform;
            _transforms = new TransformAccessArray(capacity);
        }

        public bool TryAdd(T item)
        {
            if(!_items.TryAdd(item))
                return false;

            if(_transforms.capacity < _items.Count)
            {
                var newCapacity = _items.Count.NextPowerOfTwo();
                _transforms = _transforms.EnsureSize(newCapacity);
            }
			
			_transforms.Add(_getTransform(item));

            return true;
        }

        public bool TryRemove(T item)
        {
            if(!_items.TryGetIndex(item, out var index))
                return false;

            if(!_items.TryRemove(item))
                return false;

            _transforms.RemoveAtSwapBack(index);

            return true;
        }

        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public bool TryGetIndex(T item, out int index)
        {
            return _items.TryGetIndex(item, out index);
        }

        public void Dispose()
        {
            if(_transforms.isCreated)
                _transforms.Dispose();
			_transforms = default;
        }

        ~TransformIndexedCollection()
        {
            Dispose();
        }
    }
	
	public class TransformIndexedCollectionComponent<T, TData> : TransformIndexedCollection<T, TData>
		where T : Component
	{
		public TransformIndexedCollectionComponent(int capacity) : base(capacity, x => x.transform)
		{
			
		}
	}
	
	public class TransformIndexedCollection<T, TData> : IDisposable
		where T : class
	{
		readonly DenseSet<T> _items;
		readonly Func<T, Transform> _getTransform;
		TransformAccessArray _transforms;
		TData[] _data;
		
		public int Count => _items.Count;
		
		public TransformAccessArray transforms => _transforms;
		
		public T this[int index] => _items[index];
		
		public TData[] rawData => _data;
		
		public DataAccessor data => new DataAccessor(this);
		
		public readonly struct DataAccessor
		{
			readonly TransformIndexedCollection<T, TData> collection;
			
			public DataAccessor(TransformIndexedCollection<T, TData> collection)
			{
				this.collection = collection;
			}
			
			public ref TData this[int index]
			{
				get => ref collection._data[index];
			}
			
			public ref TData this[T item]
			{
				get
				{
					if(!collection._items.TryGetIndex(item, out var index))
						throw new ArgumentException("Item not found in collection", nameof(item));
					
					return ref collection._data[index];
				}
			}
		}
		
		public TransformIndexedCollection(int capacity, Func<T, Transform> getTransform)
		{
			_items = new DenseSet<T>(capacity);
			_getTransform = getTransform;
			_transforms = new TransformAccessArray(capacity);
			_data = new TData[capacity];
		}
		
		
		
		public bool TryAdd(T item, in TData data)
		{
			if(!_items.TryAdd(item))
				return false;
			
			var index = _items.Count - 1;
			
			if(_transforms.capacity < _items.Count)
			{
				var newCapacity = _items.Count.NextPowerOfTwo();
				RebuildTransforms(newCapacity);
				RebuildData(newCapacity);
			}
			else
			{
				_transforms.Add(_getTransform(item));
			}
			
			_data[index] = data;
			
			return true;
		}
		
		public bool TryRemove(T item)
		{
			if(!_items.TryGetIndex(item, out var index))
				return false;
			
			var lastIndex = _items.Count - 1;
			
			if(!_items.TryRemove(item))
				return false;
			
			if(lastIndex != index)
			{
				_data[index] = _data[lastIndex];
			}
			_data[lastIndex] = default;
			
			_transforms.RemoveAtSwapBack(index);
			
			return true;
		}
		
		public bool Contains(T item)
		{
			return _items.Contains(item);
		}
		
		public int GetIndex(T item)
		{
			if(!_items.TryGetIndex(item, out var index))
				throw new ArgumentException("Item not found in collection", nameof(item));
			return index;
		}
		
		public bool TryGetIndex(T item, out int index)
		{
			return _items.TryGetIndex(item, out index);
		}
		
		void RebuildTransforms(int newCapacity)
		{
			if(_transforms.isCreated)
				_transforms.Dispose();
			
			_transforms = new TransformAccessArray(newCapacity);
			for(var i = 0; i < _items.Count; i++)
			{
				_transforms.Add(_getTransform(_items[i]));
			}
		}
		
		void RebuildData(int newCapacity)
		{
			_data = _data.Resize(newCapacity);
		}
		
		public void Dispose()
		{
			if(_transforms.isCreated)
				_transforms.Dispose();
			_transforms = default;
		}
		
		~TransformIndexedCollection()
		{
			Dispose();
		}
	}

	public class TransformIndexedArray<T> : IDisposable
		where T : struct
	{
		readonly DenseSet<Transform> _items;
		TransformAccessArray _transforms;
		T[] _data;

		public int Count => _items.Count;
		public TransformAccessArray transforms => _transforms;
		public T[] rawData => _data;

		public DataAccessor data => new DataAccessor(this);

		public readonly struct DataAccessor
		{
			readonly TransformIndexedArray<T> collection;
			public DataAccessor(TransformIndexedArray<T> collection) => this.collection = collection;
			public ref T this[int index] => ref collection._data[index];
			public ref T this[Transform item]
			{
				get
				{
					if (!collection._items.TryGetIndex(item, out var index))
						throw new ArgumentException("Item not found in collection", nameof(item));
					return ref collection._data[index];
				}
			}
		}

		public TransformIndexedArray(int capacity)
		{
			_items = new DenseSet<Transform>(capacity);
			_transforms = new TransformAccessArray(capacity);
			_data = new T[capacity];
		}

		public bool TryAdd(Transform transform, in T data)
		{
			if (!_items.TryAdd(transform))
				return false;

			EnsureSize(_items.Count);
			_transforms.Add(transform);

			_data[_items.Count - 1] = data;
			return true;
		}

		public bool TryRemove(Transform transform)
		{
			if (!_items.TryGetIndex(transform, out var index))
				return false;

			var lastIndex = _items.Count - 1;
			if (!_items.TryRemove(transform))
				return false;

			if (lastIndex != index)
			{
				_data[index] = _data[lastIndex];
			}
			_data[lastIndex] = default;

			_transforms.RemoveAtSwapBack(index);
			return true;
		}

		public void RemoveAtSwapBack(int index)
		{
			if (index < 0 || index >= _items.Count) return;
			TryRemove(_items[index]);
		}

		public bool Contains(Transform transform) => _items.Contains(transform);

		public bool TryGetIndex(Transform transform, out int index) => _items.TryGetIndex(transform, out index);

		public void EnsureSize(int size)
		{
			if (_transforms.capacity >= size) return;
			var newCapacity = size.NextPowerOfTwo();
			_data = _data.Resize(newCapacity);

			var newArray = new TransformAccessArray(newCapacity);
			for (var i = 0; i < _transforms.length; i++)
				newArray.Add(_transforms[i]);

			if (_transforms.isCreated)
				_transforms.Dispose();

			_transforms = newArray;
		}

		public void Dispose()
		{
			if (_transforms.isCreated)
				_transforms.Dispose();
			_transforms = default;
		}

		~TransformIndexedArray()
		{
			Dispose();
		}
	}
}
