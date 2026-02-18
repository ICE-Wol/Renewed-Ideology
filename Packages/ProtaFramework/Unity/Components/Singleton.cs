using System;

namespace Prota.Unity
{
    // 全局单例（非 MonoBehaviour 版本）.
    public abstract class Singleton<T>
        where T : Singleton<T>, new()
    {
        /// 全局实例.
        public static T instance => Get();

        /// 实例是否已创建.
        public static bool exists => _instance != null;

        static T _instance;

        /// 获取实例，不存在则创建.
        public static T Get()
        {
            if(_instance != null) return _instance;
            if(_instance == null && !object.ReferenceEquals(null, _instance)) return null;
            return _instance = new T();
        }

        /// 确保实例已存在.
        public static void EnsureExists() => Get();

        protected Singleton()
        {
            _instance = (T)this;
        }
    }
}


