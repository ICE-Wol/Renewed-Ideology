using System;
using Unity.Jobs;

namespace Prota.Unity
{
	/// <summary>
	/// 一个可以接受MultithreadManager管理的任务对象.
	/// 每帧按照执行 OnJobStart.
	/// GetDependneies查询所有类型, 使得MultithreadManager可以正确填入 dependsOn.
	/// GetCompleteDependencies查询所有类型, 使得MultithreadManager可以按照正确顺序执行 OnJobComplete.
	/// </summary>
	public interface IMultithreadTask
	{
		bool isJobRunning { get; set; }
		
		JobHandle OnJobStart(JobHandle dependsOn);
		
		void OnBeforeJobComplete();
		
		void OnJobComplete();
		
		Type[] GetDependencies();
		
		Type[] GetCompleteDependencies();
	}
}
