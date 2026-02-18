using System;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace Prota.Unity
{
	public interface IStandaloneMultithreadJobParallelForContext
	{
		int GetJobCount();
		void Execute(int elementIndex, int threadIndex);
	}
	
	/// <summary>
	/// 一个不经过MultithreadManager, 手动调用的Job.
	/// </summary>
	public class StandaloneMultithreadJobParallelFor<T> : StandaloneMultithreadJob
		where T: class, IStandaloneMultithreadJobParallelForContext, new()
	{
		int completedCount = 0;
		int totalCount = 0;
		string _jobName;
		public string jobName => _jobName ??= $"StandaloneMultithreadJobParallelFor<{typeof(T).Name}>";
		
		public struct Job : IJobParallelFor
		{
			[NativeSetThreadIndex]
			public int threadIndex;
			
			public int contextId;
			
			public void Execute(int elementIndex)
			{
				var jobWrapper = (StandaloneMultithreadJobParallelFor<T>)GetInstance(contextId);
				if(jobWrapper != null)
				{
					using (new ProfilerScope(jobWrapper.jobName))
					{
						jobWrapper.context.Execute(elementIndex, threadIndex);
						var newCount = Interlocked.Increment(ref jobWrapper.completedCount);
						if (newCount == jobWrapper.totalCount)
						{
							jobWrapper.MarkJobCompleted();
						}
					}
				}
			}
		}
		
		public int batchCount = 8;
		
		T _context = new();
		
		public T context
		{
			get => _context;
			set => _context = value;
		}
		
		protected override JobHandle ScheduleInternal(JobHandle dependsOn = default)
		{
			totalCount = context.GetJobCount();
			completedCount = 0;
			var job = new Job();
			job.contextId = id;
			return job.Schedule(totalCount, batchCount, dependsOn);
		}
	}
}

