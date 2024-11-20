using _Scripts.Tools;
using UnityEngine;

namespace _Scripts {
    public class RingParticleCtrl : MonoBehaviour {
        public SpriteRenderer spriteRenderer;
        public float xScale;
        public float yScale;
        public float direction;
        public float radius;

        public bool isCollected;

        private float _curXScale;
        private float _curYScale;
        private float _curRadius;

        public void Start() {
            transform.localPosition = Vector3.zero;
            spriteRenderer = GetComponent<SpriteRenderer>();
            _curXScale = 0f;
            _curYScale = 0f;
            _curRadius = 0f;
            
        }
        public void Update() {
            if (!isCollected) {
                _curXScale.ApproachRef(xScale,16f);
                _curYScale.ApproachRef(yScale,16f);
                _curRadius.ApproachRef(radius, 8f);
                
                transform.localPosition = _curRadius * direction.Deg2Dir3();
                transform.localRotation = Quaternion.Euler(0,0,direction);
                transform.localScale = new Vector3(_curXScale, _curYScale, 1);

            }
        }
    }
}