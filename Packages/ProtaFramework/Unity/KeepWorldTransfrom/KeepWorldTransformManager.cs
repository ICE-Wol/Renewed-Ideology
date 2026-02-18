using Prota;
using Prota.Unity;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Prota.Unity
{

    [DefaultExecutionOrder(30000)]
    public class KeepWorldTransformManager : SingletonComponent<KeepWorldTransformManager>
    {
        class JobContext : IStandaloneMultithreadJobTransformsContext
        {
            KeepWorldTransformManager manager = null!;
            
            public void Setup(KeepWorldTransformManager manager)
            {
                this.manager = manager;
            }
            
            public TransformAccessArray GetTransformAccessArray()
            {
                return manager.all.transforms;
            }
            
            public void Execute(int elementIndex, ref TransformAccess transform, int threadIndex)
            {
                var component = manager.all[elementIndex];
                if(component.keepPosition) transform.position = Vector2.zero;
                if(component.keepRotation) transform.rotation = Quaternion.identity;
                if(component.keepScale) transform.localScale = Vector3.one;
            }
        }
        
        TransformIndexedCollection<KeepWorldTransform> all = null!;
        StandaloneMultithreadJobTransforms<JobContext> job = null!;
        
        protected override void Awake()
        {
            base.Awake();
            all = new TransformIndexedCollection<KeepWorldTransform>(64, x => x.transform);
            job = new();
            job.context.Setup(this);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            job.Dispose();
            all.Dispose();
        }
        
        public void Add(KeepWorldTransform item)
        {
            if(!all.TryAdd(item)) throw new System.Exception("KeepWorldTransformManager: Add failed");
        }
        
        public void Remove(KeepWorldTransform item)
        {
            if(!all.TryRemove(item)) throw new System.Exception("KeepWorldTransformManager: Remove failed");
        }
        
        void LateUpdate()
        {
            var count = all.Count;
            if (count == 0) return;
            
            var handle = job.Schedule();
            JobHandle.ScheduleBatchedJobs();
            
            // 按理说这个不需要, 因为读取transform必定触发complete.
            // handle.Complete();
        }
    }
}
