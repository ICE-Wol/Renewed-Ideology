using _Scripts.Tools;
using UnityEngine;

namespace _Scripts {
    public class AuraCtrl : MonoBehaviour {
        public SpriteRenderer auraParticlePrefab;
        public Color auraColor;
        public int auraParticleCount;

        public bool isSCBG;
        public bool isBG;

        private int _timer = 0;
        private SpriteRenderer[] _auraParticles;
        private float[] _auraSpeed;
        void Start() {
            _auraParticles = new SpriteRenderer[auraParticleCount];
            _auraSpeed = new float[auraParticleCount];
            _timer++;
        }
    
    

        // Update is called once per frame
        void Update()
        {
            if (_timer < auraParticleCount * 10) {
                if (_timer % 10 == 0) {
                    _auraParticles[_timer / 10] = Instantiate(auraParticlePrefab, this.transform).GetComponent<SpriteRenderer>();
                    if (isSCBG) _auraParticles[_timer / 10].gameObject.layer = 6;//layer 6 : SCBG
                    if (isBG) _auraParticles[_timer / 10].gameObject.layer = 3;//layer 3 : BG
                    _auraParticles[_timer / 10].color = auraColor;
                    _auraParticles[_timer / 10].transform.rotation = Quaternion.Euler(0,0,Random.Range(0f,360f));
                    _auraParticles[_timer / 10].transform.localScale = 1.5f * Vector3.one;
                    _auraSpeed[_timer / 10] = Random.Range(-1f, 1f);
                }
            }

            for (int i = 0; i < auraParticleCount; i++) {
                if (_auraParticles[i] != null) {
                    //_auraParticles[i].transform.localScale -= 0.01f * Vector3.one;
                    _auraParticles[i].transform.localScale =
                        _auraParticles[i].transform.localScale.x.ApproachValue(0f, 64f) * Vector3.one;
                    _auraParticles[i].color = _auraParticles[i].color.Appear(96f);/**/
                    if (!isBG) _auraParticles[i].transform.rotation = Quaternion.Euler(0, 0, _timer * _auraSpeed[i]);

                    if (_auraParticles[i].color.a.Equal(1f, 0.1f)||
                        _auraParticles[i].transform.localScale.x.Equal(0,0.1f)) {
                        _auraParticles[i].color = _auraParticles[i].color.SetAlpha(0f);
                        _auraParticles[i].transform.localScale = 1.5f * Vector3.one;
                        _auraParticles[i].transform.rotation = Quaternion.Euler(0,0,Random.Range(0f,360f));
                    }
                }
            }
            
            _timer++;
        }
    }
}
