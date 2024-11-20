using System;
using _Scripts;
using _Scripts.EnemyBullet;
using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects {
    [Serializable]
    public struct BulletBasics {
        public BulletType type;
        public Sprite sprite;
        public float checkRadius;
        public BulletSize size;
    }

    [CreateAssetMenu(fileName = "EnemyBulletBasics", menuName = "EnemyBulletBasics", order = 1)]
    public class EnemyBulletBasics : ScriptableObject {
        public BulletBasics[] bulletBasics;
        public Material[] glowMaterials;
        public Sprite enemyBulletGenerateSprite;
        
        public Sprite GetBulletSprite(BulletType type) {
            return bulletBasics[(int)type].sprite;
        }
        
        public Material GetBulletMaterial(bool isGlowing) {
            return isGlowing ? glowMaterials[1] : glowMaterials[0];
        }
        
        
        /*public Sprite[] enemyBulletSprites;


        private void OnEnable() {
            bulletBasics = new BulletBasics[27];
            for (int i = 0; i < 27; i++ ) {
                bulletBasics[i].type = (BulletType) i;
                bulletBasics[i].sprite = enemyBulletSprites[i];
            }
        }*/
    }
}