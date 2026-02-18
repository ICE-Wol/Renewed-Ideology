using System;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

namespace Prota.Unity
{
	public interface IStandaloneMultithreadJobTransformsContext
	{
		TransformAccessArray GetTransformAccessArray();
		void Execute(int elementIndex, ref TransformAccess transform, int threadIndex);
	}
	
	/// <summary>
	/// 一个不经过MultithreadManager, 手动调用的Job.
	/// </summary>
	public class StandaloneMultithreadJobTransforms<T> : StandaloneMultithreadJob
		where T: class, IStandaloneMultithreadJobTransformsContext, new()
	{
		int completedCount = 0;
		int totalCount = 0;
		string _jobName;
		public string jobName => _jobName ??= $"StandaloneMultithreadJobTransforms<{typeof(T).Name}>";
		
		public struct Job : IJobParallelForTransform
		{
			[NativeSetThreadIndex]
			public int threadIndex;
			
			public int contextId;
			
			public void Execute(int elementIndex, TransformAccess transform)
			{
				var jobWrapper = (StandaloneMultithreadJobTransforms<T>)GetInstance(contextId);
				if(jobWrapper != null)
				{
					using (new ProfilerScope(jobWrapper.jobName))
					{
						jobWrapper.context.Execute(elementIndex, ref transform, threadIndex);
						var newCount = Interlocked.Increment(ref jobWrapper.completedCount);
						if(newCount == jobWrapper.totalCount)
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
			var transformArray = context.GetTransformAccessArray();
			totalCount = transformArray.length;
			completedCount = 0;
			var job = new Job();
			job.contextId = id;
			return job.Schedule(transformArray, dependsOn);
		}
	}
}

