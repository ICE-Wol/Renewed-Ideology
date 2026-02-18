using System;
using UnityEngine;

public class ShardCircleCtrl : MonoBehaviour
{
    public TransformApproacher tfApproacher;
    public SpriteRenderer sprRenderer;

    public void Awake() {
        tfApproacher = GetComponent<TransformApproacher>();
        sprRenderer = GetComponent<SpriteRenderer>();
    }
    
    
}
