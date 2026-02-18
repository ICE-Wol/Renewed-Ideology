using System;
using _Scripts.Tools;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts {
    public class FairyBreakEffectCtrl : MonoBehaviour {
        public FairyBreakMaple maplePrefab;
        public FairyBreakStream streamPrefab;
        public Color initColor;
        
        private int _propHueID;
        private int _propSatID;
        private Color _color;
        public Color color {
            set {
                _color = value;
                float H, S, V = 0;
                Color.RGBToHSV(color,out H,out S,out V);

                for (int i = 0; i < 4; i++) {
                    spriteRings[i].material.SetFloat(_propHueID, H);
                    spriteRings[i].material.SetFloat(_propSatID, S);
                }

                for (int i = 0; i < 3; i++) {
                    spriteWaves[i].material.SetFloat(_propHueID, H);
                    spriteWaves[i].material.SetFloat(_propSatID, S);
                }
            }
            get => _color;
        }
        
        private void Awake() {
            _propHueID = Shader.PropertyToID("_Hue");
            _propSatID = Shader.PropertyToID("_Saturation");
            color = initColor;
        }

        public SpriteRenderer[] spriteRings;
        private float[] _curRingScale;
        private float[] _tarRingScale;
        
        public SpriteRenderer[] spriteWaves;
        private float[] _curWaveScaleX;
        private float[] _curWaveScaleY;
        private float[] _tarWaveScaleX;
        private float[] _tarWaveScaleY;

        /// <summary>
        /// Use to identify whether the break waves' alpha have reached 0.9f.
        /// if it does, the waves will begin to fade. 
        /// </summary>
        private bool _isAppeared;
        private void Start() {
            _curRingScale = new float[4];
            _tarRingScale = new float[4];
            _curWaveScaleX = new float[2];
            _curWaveScaleY = new float[2];
            _tarWaveScaleX = new float[2];
            _tarWaveScaleY = new float[2];
            Initialize();

            AudioManager.Manager.PlaySound(AudioNames.SeEnemyExplode0);
            //Random.value > 0.5f ? AudioNames.SeEnemyExplode0 : AudioNames.SeEnemyExplode1);
            
            

        }

        public void Initialize() {
            //Rings
            for (int i = 0; i < 4; i++) {
                _curRingScale[i] = 0;
                _tarRingScale[i] = Random.Range(0.6f, 1.2f);
                spriteRings[i].color = spriteRings[i].color.SetAlpha(0.8f);
                spriteRings[i].transform.localPosition
                    = new Vector3(Random.Range(-1f, 1f),Random.Range(-0.4f, 0.4f),0f);
            }
            
            //Waves
            for (int i = 0; i < 2; i++) {
                _curWaveScaleX[i] = 0.8f;
                _curWaveScaleY[i] = 1f;
                _tarWaveScaleX[i] = 0.2f + Random.Range(-0.05f,0.05f);
                _tarWaveScaleY[i] = 2.5f + Random.Range(-0.2f,0.2f);
                spriteWaves[i].color = spriteWaves[i].color.SetAlpha(0f);
            }
            spriteWaves[2].transform.localScale = Vector3.zero;
            spriteWaves[2].color = spriteWaves[2].color.SetAlpha(0.8f);

            var rand = Random.Range(0f,180f);
            spriteWaves[0].transform.rotation = Quaternion.Euler(0,0,rand);
            spriteWaves[1].transform.rotation = Quaternion.Euler(0,0,rand + Random.Range(20f,40f));

            _isAppeared = false;
            
            //Maple Leafs
            for (int i = 0; i <= 20; i++) {
                var p = Instantiate(maplePrefab, transform);
                p.spriteRenderer.color = initColor;
                p.transform.position = transform.position;
            }

            //Break Stream
            for (int i = 0; i < 3; i++) {
                var p = Instantiate(streamPrefab, transform);
                p.spriteRenderer.color = initColor.SetAlpha(0.1f);
                p.transform.position = transform.position +
                                      Random.Range(-0.1f, 0.1f) * Vector3.right +
                                      Random.Range(-0.1f, 0.1f) * Vector3.up;
                p.transform.localScale = Random.Range(0.8f, 1.2f) * Vector3.one ;
                p.transform.rotation = Quaternion.Euler(0,0,Random.Range(0f,360f));
            }

            _timer = 300;
        }

        private int _timer;
        private void Update() {
            _timer--;
            if (_timer <= 0) {
                Destroy(gameObject);
            }
            
            for (int i = 0; i < 4; i++) {
                _curRingScale[i] = _curRingScale[i].ApproachValue(_tarRingScale[i], 10f);
                spriteRings[i].transform.localScale = _curRingScale[i] * Vector3.one;
                if (_curRingScale[i] > 0.5f) {
                    spriteRings[i].color = spriteRings[i].color.Fade(10f);
                }
            }

            for (int i = 0; i < 2; i++) {
                _curWaveScaleX[i] = _curWaveScaleX[i].ApproachValue(_tarWaveScaleX[i], 6f);
                _curWaveScaleY[i] = _curWaveScaleY[i].ApproachValue(_tarWaveScaleY[i], 6f);
                spriteWaves[i].transform.localScale = new Vector3(_curWaveScaleX[i], _curWaveScaleY[i], 1f);
                if (!_isAppeared) spriteWaves[i].color = spriteWaves[i].color.Appear(6f);
                else spriteWaves[i].color = spriteWaves[i].color.Fade(8f);
            }
            
            spriteWaves[2].transform.localScale
                = spriteWaves[2].transform.localScale.ApproachValue(1.5f * Vector3.one, 6f * Vector3.one);
            spriteWaves[2].color = spriteWaves[2].color.Fade(8f);

            if (!_isAppeared && spriteWaves[0].color.a >= 0.7f) _isAppeared = true;
        }
    }
}