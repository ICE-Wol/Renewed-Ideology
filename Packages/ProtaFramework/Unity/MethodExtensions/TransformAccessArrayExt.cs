using UnityEngine.Jobs;

namespace Prota.Unity
{
    public static class TransformAccessArrayExt
    {
        public static TransformAccessArray Resize(this TransformAccessArray original, int newCapacity) 
        { 
            TransformAccessArray newArray = new TransformAccessArray(newCapacity); 
            for (int i = 0; i < original.length; i++) 
                newArray.Add(original[i]); 
            original.Dispose(); 
            return newArray; 
        }
		
		public static TransformAccessArray EnsureSize(this TransformAccessArray original, int newCapacity)
        {
            if(original.capacity >= newCapacity)
                return original;
            return original.Resize(newCapacity);
        }
    }
}
