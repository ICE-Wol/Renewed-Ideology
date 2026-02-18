using UnityEngine;
using System;
using System.Collections.Generic;

namespace Prota.Unity
{
    // 全局单例.
    // 不随场景改变, 与场景内容无关.
    [DisallowMultipleComponent]
    public abstract class SingletonComponent<T> : MonoBehaviour
        where T: SingletonComponent<T> 
    {
		// 需要手动指定创建时机.
		public static T instance => _instance ?? throw new NullReferenceException("SingletonComponent<T> is not initialized");
        
        public static bool exists => _instance is not null;
        
		static T _instance = null!;
        
		// 这个是自动获取, 不存在则创建.
		public static T Get()
        {
            #if UNITY_EDITOR
            if(!Application.isPlaying) throw new InvalidOperationException("Cannot create singletion in edit mode!");
            #endif
            
            if(_instance is not null) return _instance;
            
            if(_instance == null && (_instance is not null)) return null;
            var g = new GameObject("#" + typeof(T).Name);
            GameObject.DontDestroyOnLoad(g.transform.root);
            return _instance = g.AddComponent<T>();
        }
        
        public static void EnsureExists() => Get();
        
        protected SingletonComponent() => _instance = (T)this;
        
        protected virtual void OnDestroy()
        {
            if(_instance == this) _instance = null;
        }
        
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject.transform.root);
        }
    }
    
}   
