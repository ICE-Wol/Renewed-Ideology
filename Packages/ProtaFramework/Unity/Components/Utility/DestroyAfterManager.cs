using UnityEngine;
using System.Collections.Generic;

namespace Prota.Unity
{
    public class DestroyAfterManager : SingletonComponent<DestroyAfterManager>
    {
        struct DestroyAfterItem
        {
            public GameObject target;
            public float remaining;
        }

        readonly List<DestroyAfterItem> items = new();

        /// <summary>
        /// 将对象加入延时销毁队列
        /// </summary>
        public void Schedule(GameObject target, float delay)
        {
            if(target == null) return;
            if(delay <= 0f)
            {
                target.ActiveDestroy();
                return;
            }
            items.Add(new DestroyAfterItem { target = target, remaining = delay });
        }

        void Update()
        {
            if(items.Count == 0) return;
            var dt = Time.deltaTime;
            for(int i = items.Count - 1; i >= 0; i--)
            {
                var it = items[i];
                if(it.target == null)
                {
                    items.RemoveAt(i);
                    continue;
                }
                it.remaining -= dt;
                if(it.remaining > 0f)
                {
                    items[i] = it;
                    continue;
                }
                it.target.ActiveDestroy();
                items.RemoveAt(i);
            }
        }
    }
}


