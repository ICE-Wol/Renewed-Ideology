using System;
using _Scripts.Enemy;
using _Scripts.Tools;
using UnityEngine;

namespace _Scripts {
    public class CircularHealthBar : MonoBehaviour {
        [SerializeField] private float radius;
        Damageable damageable => this.GetComponentInParent<Damageable>();
        int curPercent => Mathf.CeilToInt(damageable.curHealth / (float)damageable.maxHealth);
        int maxPercent => 200;
        private Vector3[] _defaultPoints;
        private LineRenderer _line;

        private void Start() {
            _line = GetComponent<LineRenderer>();
            GenerateLine();
        }

        private void GenerateLine() {
            _defaultPoints = new Vector3[maxPercent + 1];
            for (int i = 0; i <= maxPercent; i++) {
                var degree = 360f / maxPercent * i + 90f;
                _defaultPoints[i] = radius * degree.Deg2Dir();
            }
            _line.positionCount = _defaultPoints.Length;
            _line.SetPositions(_defaultPoints);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current">current HP in %</param>
        public void RefreshLine(int current) {
            
            _line.positionCount = curPercent + 1;
            for (int i = 0; i < curPercent + 1; i++) {
                _line.SetPosition(i, _defaultPoints[i]);
            }
        }

        private float _curScale = 0f;
        private float _curRotX = -90f;
        private float _curRotY = -90f;
        private float _curFillPercentage = 0;

        private void Update() {
            _curScale.ApproachRef(1f, 16f);
            _curRotX.ApproachRef(0f, 32f);
            _curRotY.ApproachRef(0f, 32f);
            //_curFillPercentage.ApproachRef(100f, 32f);
            //if (!_curFillPercentage.Equal(100f)) {
                //RefreshLine((int)_curFillPercentage);
            //}

            transform.localScale = _curScale * Vector3.one;
            transform.localRotation = Quaternion.Euler(_curRotX,_curRotY,0f);
        }

        //test
        /*private void OnDrawGizmos() {
            foreach (var point in _defaultPoints) {
                Gizmos.DrawSphere(point, 0.1f);
            }
        }*/
    }
}
