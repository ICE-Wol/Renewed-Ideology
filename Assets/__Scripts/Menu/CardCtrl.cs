using _Scripts.Tools;
using UnityEditor;
using UnityEngine;


public class CardCtrl : MonoBehaviour {
    public SpriteRenderer spriteRenderer;
    public int cardCount;
    
    public float tarRot;
    public float curRot;

    public float tarX;

    public float tarScale;
    public float curScale;
    
    public float tarAlpha;
    public float curAlpha;
    
    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update() {

        tarRot = 90f / 5 * cardCount;
        curRot.ApproachRef(tarRot,16f);
        transform.rotation = Quaternion.Euler(0, curRot, 0);

        if (cardCount == 0) tarX = 0;
        else if(cardCount < 0) tarX = -2.5f + 2 * cardCount;
        else if(cardCount > 0) tarX = 2.5f + 2 * cardCount;
        
        var x = transform.position.x;
        x = x.ApproachRef(tarX, 16f);
        transform.position = transform.position.SetX(x);
        
        if (cardCount == 0) tarScale = 1.5f;
        else if (cardCount < 0) tarScale = 1 - Mathf.Abs(cardCount) * 0.1f;
        else if (cardCount > 0) tarScale = 1 - Mathf.Abs(cardCount) * 0.1f;
        curScale.ApproachRef(tarScale, 16f);
        
        if(Mathf.Abs(cardCount) > 3) tarAlpha = 0f;
        else tarAlpha = 1f;
        curAlpha.ApproachRef(tarAlpha, 16f);
        spriteRenderer.color = spriteRenderer.color.SetAlpha(curAlpha);

        transform.localScale = curScale * Vector3.one;

        //y = Mathf.Sin(Mathf.Deg2Rad * Time.time * speedMultiplier)
    }


    private void OnDrawGizmos() {
        
        Gizmos.color = Color.red;
        //Handles.Label(transform.position, cardCount.ToString());
    }
}
