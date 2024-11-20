using System;
using _Scripts.Item;
using UnityEngine;

namespace _Scripts {
    public class StarRingCtrl : MonoBehaviour
    {
        public RingParticleCtrl starParticlePrefab;
        public ItemType type;
    
        public RingParticleCtrl[] coloredStars;
        public RingParticleCtrl[] whiteStars;
        
        private bool _isLife;
        private bool _isFrag;
        private Color _selectedColor;
        private int _selectedNum;

        private int _timer;
        
        public void Start() {
            _isLife = (type == ItemType.Life || type == ItemType.LifeFrag);
            _isFrag = (type == ItemType.BombFrag || type == ItemType.LifeFrag);
        
            _selectedColor = _isLife ? Color.magenta : Color.green;
            _selectedNum = _isFrag ? 8 : 10;

            whiteStars = new RingParticleCtrl[_selectedNum * 2];
            for (int i = 0; i < _selectedNum * 2; i++) {
                whiteStars[i] = Instantiate(starParticlePrefab, transform);
                whiteStars[i].xScale = 0.5f;
                whiteStars[i].yScale = 0.3f;
                whiteStars[i].direction = 360f / _selectedNum / 2 * i + (_isFrag ? 0f : 54f);
                
            }
        
        
            coloredStars = new RingParticleCtrl[_selectedNum];
            for (int i = 0; i < _selectedNum; i++) {
                coloredStars[i] = Instantiate(starParticlePrefab, transform);
                coloredStars[i].spriteRenderer.color = _selectedColor;
                //Debug.Log("SelectedColor" + _selectedColor);
                coloredStars[i].xScale = (i % 2 == 0) ? 2.5f : 1.2f;
                coloredStars[i].yScale = 0.3f;
                coloredStars[i].direction = 360f / _selectedNum * i + (_isFrag ? 0f : 54f);
            }
        }

        private void Update() {
            for (int i = 0; i < _selectedNum * 2; i++) {
                if (i % 2 == 0)
                    whiteStars[i].radius = 0.4f + Mathf.Abs(0.1f * Mathf.Sin(_timer / 50f)) + (_isFrag ? 0f : 0.1f);
                else whiteStars[i].radius = _isFrag ? 0.5f : 0.6f;
            }
            for (int i = 0; i < _selectedNum; i++) {
                coloredStars[i].radius = 0.4f + Mathf.Abs(0.1f * Mathf.Sin(_timer / 50f)) + (_isFrag ? 0f : 0.1f);
            }
            _timer++;
        }
    }
}
