using UnityEngine;

namespace _Scripts.Player {
    public class Config : MonoBehaviour {
        public int type;
        
        public int maxPower;
        public int maxScore;
        
        public float hitRadius;
        public float grazeRadius;
        public float itemRadius;
        
        public float slowRate;
        public float moveSpeed;
        
        private void Start() {
            type = 0;
            maxPower = 400;
            maxScore = 0;
            slowRate = 0.7f;
            moveSpeed = 4f;

            hitRadius = 0.1f;
            grazeRadius = 0.5f;
            itemRadius = 1f;
        }
    }
}