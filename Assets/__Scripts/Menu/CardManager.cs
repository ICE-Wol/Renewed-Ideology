using UnityEngine;

public class CardManager : MonoBehaviour {
    public int maxCardCount = 5;
    public CardCtrl[] cards;
    public CardCtrl cardTemplate;
    
    public void InitCards() {
        cards = new CardCtrl[maxCardCount];
        for (int i = 0; i < maxCardCount; i++) {
            cards[i] = Instantiate(cardTemplate, transform);
            cards[i].cardCount = i;
            //cards[i].transform.localPosition = new Vector3(i * 2, 0, 0);
            //cards[i].transform.localScale = new Vector3(1 - i * 0.1f, 1 - i * 0.1f, 0);
        }
    }
    
    public void RefreshCards() {
        if (Input.GetKeyDown(KeyCode.D)) {
            if (cards[0].cardCount == 0) return;
            for (int i = 0; i < maxCardCount; i++) {
                cards[i].cardCount += 1;
            }
            for (int i = 0; i < maxCardCount; i++) {
                Debug.Log(cards[i].cardCount);
            }
            
        } else if (Input.GetKeyDown(KeyCode.A)) {
            if (cards[maxCardCount - 1].cardCount == 0) return;
            for (int i = 0; i < maxCardCount; i++) {
                cards[i].cardCount -= 1;
            }
            
        }
        
    }

    private void Start() {
        InitCards();
    }

    private void Update() {
        RefreshCards();
    }
}
