using System;
using System.Collections.Generic;
using Prota;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;

namespace Prota.Unity
{
	public interface MultithreadJobParallelForContext
	{
		public int GetJobCount();
		void Execute(int elementIndex, int threadIndex);
	}
	
	public class MultithreadJobParallelFor<T> : MultithreadJobBase
		where T: class, MultithreadJobParallelForContext, new()
	{
		string _jobName;
		public string jobName => _jobName ??= $"MultithreadJobParallelFor<{typeof(T).Name}>";
		
		public struct Job : IJobParallelFor
		{
			[NativeSetThreadIndex]
			public int threadIndex;
			
			public int contextId;
			
			public void Execute(int elementIndex)
			{
				var jobWrapper = (MultithreadJobParallelFor<T>)MultithreadManager.instance.jobs[contextId];
				using (new ProfilerScope(jobWrapper.jobName))
				{
					jobWrapper.context.Execute(elementIndex, threadIndex);
				}
			}
		}
		
		public int batchCount = 16;
		
		T _context = new();
		
		public T context
		{
			get => _context;
			set => _context = value;
		}
		
		public JobHandle Schedule(JobHandle dependsOn)
		{
			var job = new Job();
			job.contextId = id;
			return job.Schedule(context.GetJobCount(), batchCount, dependsOn);
		}
	}
}
