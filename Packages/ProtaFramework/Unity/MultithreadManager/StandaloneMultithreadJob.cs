using System;
using System.Collections.Generic;
using Unity.Jobs;

namespace Prota.Unity
{
	public interface IStandaloneMultithreadJobContext
	{
		void Execute();
	}
	
	/// <summary>
	/// 一个不经过MultithreadManager, 手动调用的Job.
	/// </summary>
	public abstract class StandaloneMultithreadJob : IDisposable
	{
		static int nextId = 0;
		static readonly HashSet<int> freeIds = new();
		static readonly Dictionary<int, StandaloneMultithreadJob> instances = new();
		
		public int id { get; private set; } = -1;
		bool disposed = false;
		JobHandle currentHandle = default;
		volatile bool isRunning = false;
		
		public bool IsRunning => isRunning;
		
		internal void MarkJobCompleted()
		{
			isRunning = false;
			currentHandle = default;
		}
		
		protected StandaloneMultithreadJob()
		{
			if(freeIds.Count > 0)
			{
				var enumerator = freeIds.GetEnumerator();
				enumerator.MoveNext();
				id = enumerator.Current;
				freeIds.Remove(id);
			}
			else
			{
				id = nextId++;
			}
			instances[id] = this;
		}
		
		internal static StandaloneMultithreadJob GetInstance(int id)
		{
			return instances.TryGetValue(id, out var instance) ? instance : null;
		}
		
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if(disposed) return;
			
			if(disposing)
			{
				if(!currentHandle.Equals(default(JobHandle)))
				{
					currentHandle.Complete();
					currentHandle = default;
				}
				
				if(id >= 0)
				{
					if(instances.TryGetValue(id, out var instance) && instance == this)
					{
						instances.Remove(id);
						freeIds.Add(id);
					}
					id = -1;
				}
			}
			
			disposed = true;
		}
		
		~StandaloneMultithreadJob()
		{
			Dispose(false);
		}
		
		public JobHandle Schedule(JobHandle dependsOn = default)
		{
			if(isRunning)
			{
				throw new InvalidOperationException($"StandaloneMultithreadJob is already running.");
			}
			
			isRunning = true;
			currentHandle = ScheduleInternal(dependsOn);
			return currentHandle;
		}
		
		protected abstract JobHandle ScheduleInternal(JobHandle dependsOn);
	}
	
	public class StandaloneMultithreadJob<T> : StandaloneMultithreadJob
		where T: class, IStandaloneMultithreadJobContext, new()
	{
		string _jobName;
		public string jobName => _jobName ??= $"StandaloneMultithreadJob<{typeof(T).Name}>";
		
		public struct Job : IJob
		{
			public int contextId;
			
			public void Execute()
			{
				var jobWrapper = (StandaloneMultithreadJob<T>)GetInstance(contextId);
				if(jobWrapper != null)
				{
					using (new ProfilerScope(jobWrapper.jobName))
					{
						jobWrapper.context.Execute();
						jobWrapper.MarkJobCompleted();
					}
				}
			}
		}
		
		T _context = new();
		
		public T context
		{
			get => _context;
			set => _context = value;
		}
		
		protected override JobHandle ScheduleInternal(JobHandle dependsOn = default)
		{
			var job = new Job();
			job.contextId = id;
			return job.Schedule(dependsOn);
		}
	}
}

