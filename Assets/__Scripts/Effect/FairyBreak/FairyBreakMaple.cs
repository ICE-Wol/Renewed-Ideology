using _Scripts.Tools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts {
    public class FairyBreakMaple : Particle {
        public float randomBase;
        public void Start() {
            spriteRenderer.color = spriteRenderer.color.SetAlpha(0.6f);
            randomBase = Random.Range(0, 360f);
            speed = Random.Range(3f, 6f);
            rotation = Random.Range(0f, 360f);
            direction = rotation;
        }

        public new void Update() {
            speed = speed.ApproachValue(0, 24f);
            transform.position += speed * Time.fixedDeltaTime * direction.Deg2Dir3();
            transform.rotation = Quaternion.Euler(0, 0, rotation + timer * rotation / 360f);
            
            var xScale = Mathf.Sin((timer + randomBase) * 3f * Mathf.Deg2Rad);
            transform.localScale = new Vector3(xScale, transform.localScale.y, 1f);
            
            spriteRenderer.color = spriteRenderer.color.Fade(16f);
            
            if (speed.Equal(0f, 0.01f)) {
                Destroy(gameObject);
            }
        }
    }
}