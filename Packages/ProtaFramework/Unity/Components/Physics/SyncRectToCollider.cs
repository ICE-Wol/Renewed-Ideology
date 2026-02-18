using UnityEngine;

namespace Prota.Unity
{
    [ExecuteAlways]
    public class SyncRectToCollider : MonoBehaviour
    {
        public void Sync()
        {
            var rect = GetComponent<RectTransform>();
            var collider = GetComponent<BoxCollider2D>();
            if (rect == null || collider == null) return;

            var size = rect.sizeDelta;
            collider.size = size;

            // Adjust the offset based on the pivot
            var pivotOffset = new Vector2((0.5f - rect.pivot.x) * size.x, (0.5f - rect.pivot.y) * size.y);
            collider.offset = pivotOffset;
        }
        
        void Awake()
        {
            Sync();
        }
        
        void Update()
        {
            Sync();
        }
    }
}
