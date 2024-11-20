using _Scripts;
using _Scripts.Tools;
using UnityEngine;

namespace _Scripts {
    public class StripCtrl : MonoBehaviour {
        public SpriteRenderer stripParticlePrefab;
        public Color stripColor;
        public int stripParticleCount;
    
        private int _timer = 0;
        private SpriteRenderer[] _auraParticles;
        private float[] _approachSpeed;
        void Start() {
            _auraParticles = new SpriteRenderer[stripParticleCount];
            _approachSpeed = new float[stripParticleCount];
            _timer++;
        }
    
    

        // Update is called once per frame
        void Update()
        {
            if (_timer < stripParticleCount * 10) {
                if (_timer % 10 == 0) {
                    _auraParticles[_timer / 10] = Instantiate(stripParticlePrefab, this.transform).GetComponent<SpriteRenderer>();
                    _auraParticles[_timer / 10].color = stripColor;
                    _auraParticles[_timer / 10].color = _auraParticles[_timer / 10].color.SetAlpha(0.5f);
                    _auraParticles[_timer / 10].transform.localScale = new Vector3(Random.Range(0.2f, 0.6f),0.2f, 0);
                    _auraParticles[_timer / 10].transform.localPosition = new Vector3(Random.value / 5f - 0.1f,Random.value / 5f - 0.2f, -1f);
                    _approachSpeed[_timer / 10] = Random.Range(0.7f, 1f);
                }
            }

            for (int i = 0; i < stripParticleCount; i++) {
                if (_auraParticles[i] != null) {
                    //_auraParticles[i].transform.localScale -= 0.01f * Vector3.one;
                    _auraParticles[i].transform.localScale =
                        new Vector3(_auraParticles[i].transform.localScale.x,//,.ApproachValue(_xTarget[i], 96f),
                            _auraParticles[i].transform.localScale.y.ApproachValue(1.3f, 96f * _approachSpeed[i], 0));
                    _auraParticles[i].color = _auraParticles[i].color.Fade(16f);

                    if (_auraParticles[i].color.a.Equal(0f, 0.01f)||
                        _auraParticles[i].transform.localScale.y.Equal(1.3f,0.1f)) {
                        _auraParticles[i].color = _auraParticles[i].color.SetAlpha(0.5f);
                        _auraParticles[i].transform.localScale = new Vector3(Random.Range(0.2f, 0.6f),0.2f, 0);
                    }
                }
            }
        

            _timer++;
        }
    }
}