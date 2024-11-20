using UnityEngine;

namespace _Scripts.Tools {
    public class ColorSetter : MonoBehaviour {
        public SpriteRenderer spriteRenderer;
        public Color color;
        private int _propHueID;
        private int _propSatID;
        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            
            _propHueID = Shader.PropertyToID("_Hue");
            _propSatID = Shader.PropertyToID("_Saturation");
        }
        
        public void SetAppearance() {
            float H = 0, S = 0, V = 0;
            Color.RGBToHSV(color, out H, out S, out V);
            spriteRenderer.material.SetFloat(_propHueID, H);
            spriteRenderer.material.SetFloat(_propSatID, S);
        }

        private void Start() {
            SetAppearance();
        }

    }
}