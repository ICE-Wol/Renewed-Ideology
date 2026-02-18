using System;
using System.Collections.Generic;
using System.Linq;
using Prota;
using Prota.Unity;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Prota.Unity
{
	// 这个模块用来排序并行计算任务.
	// 大多数并行计算任务之间并没有依赖关系, 因此最好给它们先攒到一起, 然后一次性执行.
	// 用 IMultithreadTask 来定义任务.
	// 用 IMultithreadTask.GetDependencies() 来确认任务之间的依赖关系.
	// 用 IMultithreadTask.OnJobStart 来执行任务, 会传入其依赖关系.
	public class MultithreadManager : SingletonComponent<MultithreadManager>
	{
		public List<IMultithreadTask> tasks = new();
		public readonly List<IMultithreadTask> buildOrder = new();
		public readonly List<IMultithreadTask> completeOrder = new();
		public readonly List<MultithreadJobBase> jobs = new();
		public readonly Dictionary<IMultithreadTask, int> buildIndex = new();
		public readonly Dictionary<IMultithreadTask, int> completeIndex = new();
		readonly Dictionary<IMultithreadTask, Type[]> dependencies = new();
		readonly Dictionary<IMultithreadTask, Type[]> completDependencies = new();
		readonly Dictionary<Type, List<IMultithreadTask>> tasksByType = new();
		readonly Dictionary<IMultithreadTask, List<IMultithreadTask>> graph = new();
		readonly Dictionary<IMultithreadTask, int> indegree = new();
		readonly Stack<int> freeJobIds = new();
		bool isRunning;
		
		
		void Update()
		{
			RunAll();
		}
		
		public void AddTask(IMultithreadTask task)
		{
			if(task == null) throw new ArgumentNullException(nameof(task));
			if(isRunning) throw new InvalidOperationException("MultithreadManager is running jobs.");
			if(tasks.Any(x => x.GetType() == task.GetType())) throw new InvalidOperationException("Mutithread task adds same type " + task.GetType().Name);
			tasks.Add(task);
			RebuildGraphAndOrders();
		}
		
		public void RemoveTask(IMultithreadTask task)
		{
			if(isRunning) throw new InvalidOperationException("MultithreadManager is running jobs.");
			tasks.Remove(task);
			RebuildGraphAndOrders();
		}
		
		public void RunAll()
		{
			if(tasks.Count == 0) return;
			
			if(isRunning) throw new InvalidOperationException("MultithreadManager is running jobs.");
			isRunning = true;
			try
			{
				var finalHandle = ScheduleJobs();
				BeforeCompleteJobs();
				CompleteJobs(finalHandle);
			}
			finally
			{
				isRunning = false;
			}
		}
		
		void RebuildGraphAndOrders()
		{
			ResetOrders();
			BuildDependencies();
			BuildCompletDependencies();
			BuildTasksByType();
			BuildDependencyGraph();
			BuildOrders();
		}
		
		void ResetOrders()
		{
			buildOrder.Clear();
			completeOrder.Clear();
			buildIndex.Clear();
			completeIndex.Clear();
		}
		
		void BuildDependencies()
		{
			dependencies.Clear();
			for(int i = 0; i < tasks.Count; i++)
			{
				var task = tasks[i];
				var depTypes = task.GetDependencies() ?? Array.Empty<Type>();
				dependencies[task] = depTypes;
			}
		}
		
		void BuildCompletDependencies()
		{
			completDependencies.Clear();
			for(int i = 0; i < tasks.Count; i++)
			{
				var task = tasks[i];
				var depTypes = task.GetCompleteDependencies() ?? Array.Empty<Type>();
				completDependencies[task] = depTypes;
			}
		}
		
		void BuildTasksByType()
		{
			tasksByType.Clear();
			for(int i = 0; i < tasks.Count; i++)
			{
				var task = tasks[i];
				var type = task.GetType();
				if(!tasksByType.TryGetValue(type, out var list))
				{
					list = new List<IMultithreadTask>();
					tasksByType.Add(type, list);
				}
				list.Add(task);
			}
		}
		
		void BuildDependencyGraph()
		{
			graph.Clear();
			indegree.Clear();
			
			for(int i = 0; i < tasks.Count; i++)
			{
				var task = tasks[i];
				graph[task] = new List<IMultithreadTask>();
				indegree[task] = 0;
			}
			
			for(int i = 0; i < tasks.Count; i++)
			{
				var task = tasks[i];
				var depTypes = dependencies[task];
				for(int j = 0; j < depTypes.Length; j++)
				{
					var depType = depTypes[j];
					if(depType == null) continue;
					if(!tasksByType.TryGetValue(depType, out var depTasks)) continue;
					for(int k = 0; k < depTasks.Count; k++)
					{
						var depTask = depTasks[k];
						if(depTask == null) continue;
						if(depTask == task) continue;
						graph[depTask].Add(task);
						indegree[task] = indegree[task] + 1;
					}
				}
			}
		}
		
		void BuildOrders()
		{
			var queue = new Queue<IMultithreadTask>();
			foreach(var pair in indegree)
			{
				if(pair.Value == 0) queue.Enqueue(pair.Key);
			}
			
			while(queue.Count > 0)
			{
				var task = queue.Dequeue();
				buildOrder.Add(task);
				var nextList = graph[task];
				for(int i = 0; i < nextList.Count; i++)
				{
					var next = nextList[i];
					indegree[next] = indegree[next] - 1;
					if(indegree[next] == 0) queue.Enqueue(next);
				}
			}
			
			if(buildOrder.Count != tasks.Count)
			{
				buildOrder.Clear();
				buildOrder.AddRange(tasks);
			}
			
			for(int i = 0; i < buildOrder.Count; i++)
			{
				buildIndex[buildOrder[i]] = i;
			}
			
			completeOrder.AddRange(buildOrder);
			completeOrder.Reverse();
			
			for(int i = 0; i < completeOrder.Count; i++)
			{
				completeIndex[completeOrder[i]] = i;
			}
		}
		
		Dictionary<Type, JobHandle> handles = new();
		JobHandle ScheduleJobs()
		{
			handles.Clear();
			var finalHandle = default(JobHandle);
			
			for(int i = 0; i < buildOrder.Count; i++)
			{
				var task = buildOrder[i];
				task.isJobRunning = true;
				using(new ProfilerScope(scheduleJobNameCache[task]))
				{
					var totalDeps = ComposeDependencies(task, handles);
					var handle = task.OnJobStart(totalDeps);
					
					var type = task.GetType();
					handles[type] = handle;
					
					finalHandle = JobHandle.CombineDependencies(finalHandle, handle);
				}
			}
			
			return finalHandle;
		}
		
		JobHandle ComposeDependencies(IMultithreadTask type, Dictionary<Type, JobHandle> handles)
		{
			JobHandle deps = default;
			foreach(var handle in dependencies[type])
			{
				deps = JobHandle.CombineDependencies(deps, handles[handle]);
			}
			return deps;
		}
		
		void BeforeCompleteJobs()
		{
			for(int i = 0; i < buildOrder.Count; i++)
			{
				using(new ProfilerScope(beforeCompleteJobNameCache[buildOrder[i]]))
					buildOrder[i].OnBeforeJobComplete();
			}
		}
		
		void CompleteJobs(JobHandle finalHandle)
		{
			finalHandle.Complete();
			
			for(int i = 0; i < completeOrder.Count; i++)
			{
				using(new ProfilerScope(completeJobNameCache[completeOrder[i]]))
					completeOrder[i].OnJobComplete();
				completeOrder[i].isJobRunning = false;
			}
		}
		
		internal int RegisterJob(MultithreadJobBase job)
		{
			if(job == null) throw new ArgumentNullException(nameof(job));
			int id;
			if(freeJobIds.Count > 0)
			{
				id = freeJobIds.Pop();
				if(id < jobs.Count) jobs[id] = job;
				else
				{
					id = jobs.Count;
					jobs.Add(job);
				}
			}
			else
			{
				id = jobs.Count;
				jobs.Add(job);
			}
			return id;
		}
		
		internal void UnregisterJob(MultithreadJobBase job)
		{
			if(job == null) return;
			var id = job.id;
			if(id < 0 || id >= jobs.Count) return;
			if(jobs[id] != job) return;
			jobs[id] = null;
			freeJobIds.Push(id);
		}
		
		// ============================================================================
		// ============================================================================
		
		static ObjectStringCache<IMultithreadTask> scheduleJobNameCache
			= new ObjectStringCache<IMultithreadTask>(x => string.Intern($"ScheduleJobs: {x.GetType().Name}"));
		
		static ObjectStringCache<IMultithreadTask> beforeCompleteJobNameCache
			= new ObjectStringCache<IMultithreadTask>(x => string.Intern($"BeforeCompleteJobs: {x.GetType().Name}"));
		
		static ObjectStringCache<IMultithreadTask> completeJobNameCache
			= new ObjectStringCache<IMultithreadTask>(x => string.Intern($"CompleteJobs: {x.GetType().Name}"));
	}
}
