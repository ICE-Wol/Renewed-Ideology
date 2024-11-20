using System;
using _Scripts.Tools;
using UnityEngine;

namespace _Scripts.Player {
    public class BackEffectCtrl : MonoBehaviour {
        public Transform followObject;
        
        public MeshRenderer distortRenderer;  
        public MeshRenderer magicSquareRenderer;

        private void Update() {
            transform.position.SetXY(followObject.position);
            //distortRenderer.transform.position.SetXY(transform.position);
            //magicSquareRenderer.transform.position.SetXY(transform.position);
        }
    }
}
