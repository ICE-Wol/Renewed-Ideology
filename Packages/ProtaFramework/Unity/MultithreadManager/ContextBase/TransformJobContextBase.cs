
using UnityEngine;
using UnityEngine.Jobs;

namespace Prota.Unity
{

	public abstract class TransformJobContextBase<T> : IMultithreadJobTransformsContext
		where T : Component
	{
		public TransformIndexedCollectionComponent<T> transforms = null!;

		public virtual void Setup(TransformIndexedCollectionComponent<T> transforms)
		{
			this.transforms = transforms;
		}

		public abstract void Execute(int elementIndex, ref TransformAccess transform, int threadIndex);

		public TransformAccessArray GetTransformAccessArray()
		{
			return transforms.transforms;
		}
	}

}