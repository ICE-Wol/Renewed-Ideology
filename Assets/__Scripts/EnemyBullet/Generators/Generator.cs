using _Scripts.EnemyBullet.MoveMethod;
using _Scripts.Player;
using UnityEngine;

namespace _Scripts.EnemyBullet {
    public class Generator : MonoBehaviour {
        public GameObject[] EBPrefabs;

        public int intervalFrame;
        public int initWays;
        public float initSpeed;
        public float initRadius;
        public float rotateSpeed;
        public float rotateAcc;

        private int _timer;
        private int _ways;
        private float _speed;
        private float _rotSpeed;
        private float _radius;
        private float _initDeg;

        //public float Snip() =>
        //    Vector2.SignedAngle(Ctrl.Player.transform.position - transform.position, Vector2.right);


        public _Scripts.Enemy.Movement movement;
        private void Start() {
            _ways = initWays;
            _speed = initSpeed;
            _radius = initRadius;
            _rotSpeed = rotateSpeed;
            //_initDeg = Snip();

            /*for (int i = -5; i <= 5; i++) {
                for (int j = -5; j <= 5; j++) {
                    var bullet = Instantiate(EBPrefab, new Vector3(i, j, 0), transform.rotation);
                }
            }*/
        }

        private void Update() {
            if (movement.isSteady) {
                if (_timer % intervalFrame == 0) {
                    var _ways = 12;
                    for (int i = 0; i < _ways; i++) {
                        var bullet = Instantiate(EBPrefabs[0], transform.position, transform.rotation);
                        //bullet.bulletMovement = new ZigzagLinear(_speed,
                         //   _initDeg + _rotSpeed + 360f / _ways * i, 0.03f, 60);
                    }

                }
            }

            if (_timer % intervalFrame == 0 && _timer <= 90) {
                for (int i = 0; i < _ways; i++) {
                    var bullet = Instantiate(EBPrefabs[1], transform.position, transform.rotation);
                    //bullet.bulletMovement = new ZigzagLinear(_speed,
                    //    _initDeg - _rotSpeed + 360f / _ways * i, 0.05f, 30);
                }

            }

            _rotSpeed += 0.05f;
            _timer++;

            if (_timer >= 150) {
                _timer = 0;
                _rotSpeed *= -1;
               // _initDeg = Snip();
            }

            if (_timer % 30 == 0) {
                var deg = Random.Range(0f, 360f);
                for (int i = 0; i < _ways; i++) {
                    var bullet = Instantiate(EBPrefabs[2], transform.position, transform.rotation);
                    //bullet.bulletMovement = new UniformLinear(_speed, deg + 360f / _ways * i);
                }
            }

        }
    }
}
