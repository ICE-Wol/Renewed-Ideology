using _Scripts.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts {
    public class Particle : MonoBehaviour {
        public SpriteRenderer spriteRenderer;
        public Sprite[] anim;
        public int spritePointer;
        public int timer;
        
        public int frameSpeed;
        public float direction;
        public float rotation;
        public float speed;
        public Vector3 scale;

        /// <summary>
        /// Whether the particle sprite will be played repeatedly.
        /// </summary>
        public bool isLoop;
        
        /// <summary>
        /// Whether the particle sprite has animation.
        /// </summary>
        public bool isAnimated;

        public Color color;
        public int propHueID;
        public int propSatID;
        

        public void SetColor(Color value) {
            color = value;
            float H, S, V = 0;
            Color.RGBToHSV(value, out H, out S, out V);
            spriteRenderer.material.SetFloat(propHueID, H);
            spriteRenderer.material.SetFloat(propSatID, S);
        }

        public void Awake() {
            //if put this into start() will cause some problem
            //the initial color will not be convert to the material properly
            //and the particle will sometimes be red
            timer = 0;
            propHueID = Shader.PropertyToID("_Hue");
            propSatID = Shader.PropertyToID("_Saturation");

            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        
        public void Update() {
            //_type = 0, single image sprite to self identify.
            if (isAnimated && (timer % frameSpeed == 0)) { 
                if (spritePointer >= anim.Length) {
                    if (isLoop) {
                        spritePointer = 0;
                        spriteRenderer.sprite = anim[spritePointer];
                    } else {
                        Destroy(gameObject);
                        //Note: this update will still be going shortly after the release function.
                        return;
                    }
                }
                else spriteRenderer.sprite = anim[spritePointer];
                spritePointer++;
            }

            timer++;
        }
    }
} 