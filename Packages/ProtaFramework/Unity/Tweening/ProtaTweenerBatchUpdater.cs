using UnityEngine;
using Prota.Unity;
using Prota;
using System;
using Unity.Jobs;
using System.Collections.Generic;

namespace Prota.Unity
{
    public class ProtaTweenerBatchUpdater : SingletonComponent<ProtaTweenerBatchUpdater>, IMultithreadTask
    {
		public bool isJobRunning { get; set; }
		public bool completeAfterUpdate => false;
		DenseSet<ProtaTweener> _allTweeners = new(128);
        
        public IReadOnlyCollection<ProtaTweener> allTweeners => _allTweeners;
		
        bool[] shouldUpdate = new bool[256];
        MultithreadJobParallelFor<StepProgressJobContext> stepProgressJob = null!;
        
        protected override void Awake()
        {
            base.Awake();
            EnsureCapacity(256);
            MultithreadManager.instance.AddTask(this);
            stepProgressJob = new MultithreadJobParallelFor<StepProgressJobContext>();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
			stepProgressJob = null!;
            
			if(AppQuit.isQuitting) return;
            MultithreadManager.instance.RemoveTask(this);
        }



        void EnsureCapacity(int count)
        {
            if(shouldUpdate.Length < count)
            {
                Array.Resize(ref shouldUpdate, count);
            }
        }
        
		public void RemoveTweener(ProtaTweener tweener)
		{
			if (isJobRunning) throw new InvalidOperationException("Cannot remove tweener while job is running.");
			_allTweeners.TryRemove(tweener);
		}
		
		public void AddTweener(ProtaTweener tweener)
		{
			if (isJobRunning) throw new InvalidOperationException("Cannot add tweener while job is running.");
			_allTweeners.TryAdd(tweener);
		}
        
        public JobHandle OnJobStart(JobHandle dependsOn)
        {
            var count = _allTweeners.Count;
            if (count == 0) return dependsOn;

            EnsureCapacity(count);

            var dtGame = Time.deltaTime;
            var dtRealtime = Time.unscaledDeltaTime;

            stepProgressJob.context.Setup(_allTweeners, shouldUpdate, count, dtGame, dtRealtime);
            stepProgressJob.batchCount = 32;
            var stepProgressJobHandle = stepProgressJob.Schedule(dependsOn);
            JobHandle.ScheduleBatchedJobs();
			
			return stepProgressJobHandle;
        }
        
        public void OnBeforeJobComplete()
        {
        }
        
        public void OnJobComplete()
        {
			var count = _allTweeners.Count;
            for (int i = 0; i < count; i++)
            {
                var tweener = _allTweeners[i];
                tweener.StepControl(shouldUpdate[i]);
            }
        }
        
        public Type[] GetDependencies()
        {
            return Array.Empty<Type>();
        }
        
        public Type[] GetCompleteDependencies()
        {
            return Array.Empty<Type>();
        }
        
        class StepProgressJobContext : MultithreadJobParallelForContext
        {
            public DenseSet<ProtaTweener> allTweeners = null!;
            public bool[] shouldUpdate = null!;
            public float dtGame;
            public float dtRealtime;
            public int count;

            public void Setup(DenseSet<ProtaTweener> allTweeners, bool[] shouldUpdate, int count, float dtGame, float dtRealtime)
            {
                this.allTweeners = allTweeners;
                this.shouldUpdate = shouldUpdate;
                this.count = count;
                this.dtGame = dtGame;
                this.dtRealtime = dtRealtime;
            }

            public int GetJobCount()
            {
                return count;
            }

            public void Execute(int elementIndex, int threadIndex)
            {
                var tweener = allTweeners[elementIndex];
                var dt = tweener.timerType == TweenTimerType.Game ? dtGame : dtRealtime;
                shouldUpdate[elementIndex] = tweener.StepProgress(dt);
            }
        }
    }
}

