using _Scripts.EnemyBullet;
using UnityEngine;

namespace _Scripts.Enemy.Boss {
    public class NoneSpell0001 : AttackPattern {
        public Generator000101 generatorPrefab;
        public Generator000101 generator;
        
        void Update()
        {
            if (!GetComponent<Movement>().isSteady && !generator) {
                generator = Instantiate(generatorPrefab, transform);
            }
        }
    }
}
