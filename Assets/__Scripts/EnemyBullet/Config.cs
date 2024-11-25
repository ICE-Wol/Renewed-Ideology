using System;
using System.Security.Cryptography.X509Certificates;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace _Scripts.EnemyBullet {
    
    public enum BulletType {
        JadeS,
        JadeR,
        Bacteria,
        Rice,
        Scale,
        StarS,
        Dart,
        Drop,
        Shot,
        Bubble,
        Ice,
        Spell,
        JadeM,
        JadeG,
        JadeL,
        Ellipse,
        StarM,
        Butterfly,
        Knife,
        Heart,
        Point,
        Sprinkle,
        Spark,
        SnowS,
        SnowM,
        Bottle,
        Tube
    }
    
    public enum BulletSize{
        Small,
        Medium,
        Large
    }
    
    [Serializable]
    public struct BulletBasics {
        public BulletType type;
        public Sprite sprite;
        public float checkRadius;
        public BulletSize size;
    }
    
    public enum MovementType {
        MultiSpeedApproach,
        MultiSpeedLinear,
        UniformLinear,
        ZigZagLinear
    }
    
    
    public class Config : MonoBehaviour {
        public EnemyBulletBasics basic;
        
        
        public BulletType type;
        public BulletSize size;
        public Color color;
        public Vector3 scale;
        public float collideRadius;
        public float selfRotSpdInDegree;
        public bool isGlowing;
        [Tooltip("Conflict with Self Rotation")]
        public bool faceMoveDirection;
        public bool moveWhenSpawning;
        public bool isOutOfBoundFree;
        public bool indestructible;
        
        [Header("激光相关")]
        public bool isLaserNode;
        //public CurveLaserHead laserParent;
        
        private void OnValidate() {
            basic = Resources.Load<EnemyBulletBasics>("EnemyBulletBasics");
            var spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (spriteRenderer == null) return;
            if (basic == null) return;
            
            spriteRenderer.sprite = basic.GetBulletSprite(type);
            spriteRenderer.color = Color.white;
            collideRadius = basic.bulletBasics[(int)type].checkRadius;
            
            //basic.bulletBasics[(int)type].sprite = spriteRenderer.sprite;
            //basic.bulletBasics[(int)type].size = size;
            //basic.bulletBasics[(int)type].checkRadius = collideRadius;
            
        }
        
    }
}
