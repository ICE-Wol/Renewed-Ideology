using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace _Scripts.Enemy {
    public class Movement : MonoBehaviour {
        private Vector3 _tarPos;
        private bool _hasArrivedTarPos;
        
        public bool hasFixedPos;
        public bool hasInitPos;
        public Vector3 initPos;
        public bool initPosFinished;

        public bool isAuto;
        public int stayFrames;
        public float approachRate;

        public bool isSteady;

        public int timer;

        private Vector3 GetNextTarPos() {
            Vector3 posPlayer = Player.PlayerCtrl.instance.transform.position;
            Vector3 curPos = transform.position;
            Vector3 movePos = Vector3.zero;
            
            movePos.x = Random.Range(0.3f, 1.8f) * ((posPlayer.x > curPos.x) ? 1 : -1);
            movePos.y = Random.Range(-0.5f, 1f);
            
            Vector3 tarPos = curPos + movePos;
            ClampPos(ref tarPos);
            
            return tarPos;
        }

        private void ClampPos(ref Vector3 tarPos) {
            if (tarPos.y >= 3.5f) tarPos.y = 3f;
            if (tarPos.y <= -0.5f) tarPos.y = 0.5f;
            if (tarPos.x >= 4f) tarPos.x = 3f;
            if (tarPos.x <= -4f) tarPos.x = -3f;
        }

        private void Start() {
            _tarPos = GetNextTarPos();
            approachRate = 24f;
            timer = 0;
        }
        
        

        private void Update() {
            if (hasInitPos) {
                ClampPos(ref initPos);
                _tarPos = initPos;
                transform.position = transform.position.ApproachValue
                    (_tarPos, approachRate * Vector3.one,0.01f);
                //transform.position = Vector3.MoveTowards(transform.position, _tarPos, 0.1f);
                if (transform.position.Equal(_tarPos, 0.01f)) {
                    transform.position = _tarPos;
                    hasInitPos = false;
                    initPosFinished = true;
                    isSteady = true;
                    stayFrames = 180;
                    if(!hasFixedPos) timer++;
                    //timer++以触发停留时间计时器，如果有固定位置则不计时
                }
                else {
                    return;
                }
            }

            if (!hasFixedPos) {

                if (timer <= 0) {
                    transform.position = transform.position.ApproachValue
                        (_tarPos, approachRate * Vector3.one, 0.01f);
                    //0. approaching tar pos when isn't staying
                }

                if (transform.position.Equal(_tarPos, 0.01f) && !isSteady && !hasFixedPos) {
                    timer++;
                    isSteady = true;
                    //1. arrived at tar pos. stay counter begin.
                }

                if (timer > 0) {
                    timer++;
                    //2. stay counter grow while staying
                }

                if (isAuto) {
                    if (timer >= stayFrames) {
                        timer = 0;
                        _tarPos = GetNextTarPos();
                        isSteady = false;
                        //3. counter reaches stay frame upper limit
                        //reset it to zero and get new tar pos
                    }
                }
            }
        }
        
        public void GoToNextPos() {
            _tarPos = GetNextTarPos();
            isSteady = false;
            timer = 0;
        }
        public void GoToNextPos(Vector3 pos) {
            ClampPos(ref pos);
            _tarPos = pos;
            isSteady = false;
            timer = 0;
        }
        
        public bool hasArrivedTarPos => transform.position.Equal(_tarPos, 0.5f);
    }
}