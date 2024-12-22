using UnityEngine;
using UnityEngine.PlayerLoop;

namespace _Scripts.Player {
    public class Shoot : MonoBehaviour{
        public MainBullet mainBullet;

        private int _timer;

        void Update() {
            if (Input.GetKey(KeyCode.Z) && _timer % 2 == 0) {
                AudioManager.Manager.PlaySound(AudioNames.SePlayerShoot);
                Instantiate(mainBullet, transform.position + 0.13f * Vector3.left + 0.5f * Vector3.up, Quaternion.Euler(0, 0, 90));
                Instantiate(mainBullet, transform.position - 0.13f * Vector3.left + 0.5f * Vector3.up, Quaternion.Euler(0, 0, 90));
                
                
            }
            
            _timer++;
        }
    }
}