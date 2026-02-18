using System;
using _Scripts.Tools;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

namespace _Scripts {
    public class SpellAnnouncement : MonoBehaviour {
        public static SpellAnnouncement instance;

        private void Awake() {
            if (instance == null) {
                instance = this;
            }
            else {
                Destroy(gameObject);
            }
        }

        public SpriteRenderer bossPortrait;
        public Transform spellNameObject;
        public TMP_Text scNameText;
        public SpellBannerCtrl spellBannerCtrl;
        public Renderer spellBgFull;
        public Renderer spellBgAppear;
        public FragManager spellBgFrag;

        public Vector2 pStart;
        public Vector2 pStay;
        public Vector2 pEnd;

        public Vector2 nStart;
        public Vector2 nStay;
        public Vector2 nEnd;
        public Vector2 nScale;

        public bool isAnnouncing;
        public bool isBreaking;
        public bool isAnnounceFinished;
        public int midTimerFlag;
        public int endTimerFlag;
        
        private float _appearFactor;
        private float _maxAppearFactor = 2.0f;
        private int _timer;
        
        private static readonly int Alpha = Shader.PropertyToID("_Alpha");
        private static readonly int AppearFactor = Shader.PropertyToID("_AppearFactor");

        public void ModifySpellCardBackgroundAlpha(float value)
        {
            spellBgFull.material.color =  spellBgFull.material.color.SetAlpha(value);
            
        }

        public void ResetAnnounce() {
            _timer = 0;
            isAnnouncing = false;
            isBreaking = false;
            isAnnounceFinished = false;
            
            spellNameObject.position = nStart;
            spellNameObject.localScale = nScale;
            bossPortrait.transform.position = pStart;
            
            
            ModifySpellCardBackgroundAlpha(0f);
            spellBgAppear.material.SetFloat(AppearFactor,_maxAppearFactor);
            for (int i = 1; i <= 4; i++) {
                var vec = new Vector2(Random.Range(-10, 10), Random.Range(-10, 10));
                spellBgAppear.material.SetVector("Offset" + i, vec);
            }
            
            _appearFactor = _maxAppearFactor;

            
            spellBannerCtrl.enabled = false;
            
        }

        public void StartAnnouncing() {
            isAnnouncing = true;
            spellBannerCtrl.enabled = true;
            spellBannerCtrl.BannerAppear();
            spellBgFull.gameObject.SetActive(true);
            spellBgAppear.gameObject.SetActive(true);
        }

        public void SpellBreak() {
            isBreaking = true;
            
            spellBgFrag.ResetFrag();
            spellBgFrag.EnableFrag();
        }
        
        private void Start() {
            ResetAnnounce();   
        }

        private void Update() {
            if (isAnnouncing) {
                if (_timer <= midTimerFlag) {
                    bossPortrait.transform.position =
                        bossPortrait.transform.position.ApproachValue(pStay, 16f * Vector3.one);
                    spellNameObject.position =
                        spellNameObject.position.ApproachValue(nStay, 16f * Vector3.one);
                    if (_timer == midTimerFlag) {
                        spellBannerCtrl.BannerDisappear();
                    }
                } else if (_timer <= endTimerFlag) {
                    bossPortrait.transform.position =
                        bossPortrait.transform.position.ApproachValue(pEnd, 16f * Vector3.one);
                    spellNameObject.position =
                        spellNameObject.position.ApproachValue(nEnd, 16f * Vector3.one);
                    spellNameObject.localScale =
                        spellNameObject.localScale.ApproachValue(Vector2.one, 16f * Vector3.one);
                    isAnnounceFinished = true;
                } else {
                    spellBannerCtrl.enabled = false;
                }

                if (isBreaking) {
                    spellNameObject.position =
                        spellNameObject.position.ApproachValue(nEnd + 300f * Vector2.up, 16f * Vector3.one);
                }

                //_alpha.ApproachRef(1f, 16f);
                _appearFactor.ApproachRef(-1f, 64f);
                spellBgAppear.material.SetFloat(AppearFactor,_appearFactor);
                //ModifySpellCardBackgroundAlpha(_alpha);
                if (_appearFactor.Equal(-1f,0.1f)) {
                    spellBgAppear.material.SetFloat(AppearFactor,_maxAppearFactor);
                    ModifySpellCardBackgroundAlpha(1f);
                }

                _timer++;
            }
        }
    }
}
