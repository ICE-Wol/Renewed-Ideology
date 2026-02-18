using UnityEngine;

namespace Prota.Unity
{

	public class TimerManagerComponent : SingletonComponent<TimerManagerComponent>
	{
		public bool enableLog = false;	
		public string logFilePath = "timer.log.txt";
		public bool enableConsole = false;
		public TimerManager timerManager = null!;
		
		protected override void Awake()
		{
			base.Awake();
			if(TimerManager.instance != null)
			{
				Debug.LogError("TimerManagerComponent already exists!");
			}
			
			timerManager = new TimerManager(enableLog, logFilePath, enableConsole);
			TimerManager.instance = timerManager;
		}
		
		void Update()
		{
			timerManager.Update(Time.time, Time.unscaledTime);
		}
		
		protected override void OnDestroy()
		{
			base.OnDestroy();
			timerManager.ClearAllQueues();
			if(TimerManager.instance == timerManager)
			{
				TimerManager.instance = null!;
			}
		}
	}
}
