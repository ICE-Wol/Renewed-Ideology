using System;
using _Scripts.Tools;
using UnityEngine;

namespace _Scripts {
    public class MagicSquareController : MonoBehaviour
    {
        public float scaleMultiplier;
        private float _curScale;
        private float _timer;

        private void Start() {
            _timer = 0;
            _curScale = 0;
            transform.localScale = Vector3.zero;
        }

        void Update() {
            //_timer += transform.localScale.x.Equal(1f, 0.1f) ? 1 : 2;
            if (_timer <= 1150) {
                _timer.ApproachRef(1200, 64f);
            }
            else {
                _timer += 1;
            }
            _curScale.ApproachRef(1.3f, 32f);
            transform.localScale = scaleMultiplier * _curScale * Vector3.one;
            transform.rotation = Quaternion.Euler(
                45f * Mathf.Abs(Mathf.Sin(0.15f * _timer * Mathf.Deg2Rad)),
                -45f * Mathf.Sin(0.3f * _timer * Mathf.Deg2Rad),
                0.6f * _timer);
        }
    }
}