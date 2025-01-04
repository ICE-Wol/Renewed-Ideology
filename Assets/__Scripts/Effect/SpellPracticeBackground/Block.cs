using _Scripts.Tools;
using UnityEngine;

public class Block : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Vector3 targetPos;
    public Vector3 approachRate;
    void Awake() {
        approachRate = new Vector3(Random.Range(32f, 64f), Random.Range(32f, 64f), 32f);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        transform.localPosition = transform.localPosition.ApproachValue(targetPos, approachRate);
    }
}
