namespace Prota.Unity
{
	public abstract class MultithreadJobBase
	{
		public int id = -1;
		
		protected MultithreadJobBase()
		{
			id = MultithreadManager.instance.RegisterJob(this);
		}
	}
}
