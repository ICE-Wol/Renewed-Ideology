using System;
using System.Collections.Generic;
using _Scripts.EnemyBullet;
using MEC;
using TMPro;
using UnityEngine;
using State = _Scripts.EnemyBullet.State;

namespace _Scripts {
    public class GameManager : MonoBehaviour {
        /*
         * 只存储运行时数据
         */
        [Header("基本数据")]
        public SpellPracticeManager.Difficulty difficulty;
        public int stageIndex;
        public TimeLine curTimeLine;
        
        [Header("作弊信息")]
        
        public bool isCheatModeOn;
        public bool isHitEffectOn;
        public int hits;
        public static GameManager Manager;

        //public BossCtrl curBoss;
        
        public Material[] glowMaterials;
        public Item.Item[] items;
        public BulletBreakCtrl bulletBreakPrefab;
        public ReverseColorCtrl reverseColorCtrl;
        public TextMeshProUGUI cheatAlertTest;
        public Transform bulletSortingGroup;
        public Transform itemSortingGroup;
        public Transform bulletBreakPrefabGroup;
        public UIManager uiManager;

        public Vector2 bulletEraseCenter;
        public float bulletEraseRadius;
        
        private void Awake() {
            if (!Manager) {
                Manager = this;
            }
            else {
                Destroy(gameObject);
            }
            Application.targetFrameRate = 60;
        }

        private void Start() {
            cheatAlertTest.enabled = isCheatModeOn;
            cheatAlertTest.text = (isCheatModeOn ? "CheatModeOn" : "CheatModeOff") + "\nHits: " + hits;
        }


        public int timer = 0;
        private void Update() {
            timer++;
            if (Input.GetKeyDown(KeyCode.C)) {
                isCheatModeOn = !isCheatModeOn;
                //cheatAlertTest.enabled = isCheatModeOn;
            }

            if (isCheatModeOn) {
                cheatAlertTest.text = (isCheatModeOn ? "CheatModeOn" : "CheatModeOff") + "\nHits: " + hits;
                cheatAlertTest.text += "\nTimer: " + timer;
            }

            if (isCheatModeOn) {
                if (Input.GetKey(KeyCode.E)) {
                    StartEraseBullets(new Vector2(0f,0f));
                }
            }
            
            IncreaseEraseRadius();
        }

        public static IEnumerator<float> WaitForFrames(int frames) {
            for (int i = 0; i < frames; i++) {
                yield return Timing.WaitForOneFrame;
            }
        }
        
        #region EraseBullet
        public void StartEraseBullets(Vector2 pos) {
            bulletEraseCenter = pos;
            //print(bulletEraseCenter);
            bulletEraseRadius = 0.1f;
        }
        
        private void IncreaseEraseRadius() {
            if (bulletEraseRadius > 0f) {
                EraseBulletsWithinRange();
                bulletEraseRadius += 0.1f;
            }

            if (bulletEraseRadius >= 10f)
                bulletEraseRadius = 0;
        }
        
        public bool IsEraseFinished() {
            return bulletEraseRadius == 0;
        }
        private void EraseBulletsWithinRange() {
            var copySet = new HashSet<State>();
            foreach (var b in State.bulletSet) {
                copySet.Add(b);
            }
            foreach (var b in copySet) {
                if (b == null) {
                    State.bulletSet.Remove(b);
                    continue;
                }
                if (b.GetState() == EBulletStates.Template) continue;
                if (Vector2.Distance(b.transform.position, bulletEraseCenter) < bulletEraseRadius) {
                    b.SetState(EBulletStates.Destroying);
                    State.bulletSet.Remove(b);
                }
            }
        }
        #endregion
        public void CreateBulletDestroyParticle(Vector3 pos, Color c, BulletSize size, int sortingOrder) {
            //pos = pos.SetZ(-5);
            var particle = Instantiate(bulletBreakPrefab, pos, Quaternion.identity);
            particle.transform.SetParent(bulletBreakPrefabGroup);
            particle.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
            particle.SetColor(c);
            particle.SetScale(size);
            //print("CreateBulletDestroyParticle" + size);
        }
        
    }
}

