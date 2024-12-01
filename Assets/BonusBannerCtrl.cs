using System.Globalization;
using _Scripts.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusBannerCtrl : MonoBehaviour
{
    public Image bottom;
    public Image patternBlue;
    public Image patternGreen;
    public Image scb;
    public Image nscb;
    public Image failed;
    public TMP_Text scbPoints;

    public enum BonusState
    {
        SpellCardBonus,
        NoneSpellBonus,
        BonusFailed,
    }

    public BonusState state;
    
    public float tarAlpha;
    public float curAlpha;

    public int appearTimer = 0;
    public int appearTime = 120;
    

    public void DisappearAll() {
        tarAlpha = 0f;
    }
    
    public void ActivateBonusState(bool isSpell,bool hasBonus,int bonusPoints) {
        tarAlpha = 1f;
        appearTimer = 1;
        if (!hasBonus) state = BonusState.BonusFailed;
        else state = isSpell ? BonusState.SpellCardBonus : BonusState.NoneSpellBonus;
        scbPoints.text = '+' + bonusPoints.ToString("#,0", CultureInfo.InvariantCulture);
    }

    private void Update() {
        curAlpha.ApproachRef(tarAlpha, 16f);
        switch (state) {
            case BonusState.SpellCardBonus:
                bottom.color = bottom.color.SetAlpha(curAlpha);
                patternBlue.color = patternBlue.color.SetAlpha(curAlpha);
                scbPoints.color = scbPoints.color.SetAlpha(curAlpha);
                var c = Color.Lerp(Color.white, Color.cyan, (float)(Mathf.Sin(10 * appearTimer * Mathf.Deg2Rad) + 1f) / 2f);
                scb.color = c.SetAlpha(curAlpha);
                break;
            case BonusState.NoneSpellBonus:
                patternGreen.color = patternGreen.color.SetAlpha(curAlpha);
                c = Color.Lerp(Color.white, Color.green, (float)(Mathf.Sin(10 * appearTimer * Mathf.Deg2Rad) + 1f) / 2f);
                nscb.color = c.SetAlpha(curAlpha);
                break;
            case BonusState.BonusFailed:
                c = Color.Lerp(Color.white, Color.gray, (float)(Mathf.Sin(10 * appearTimer * Mathf.Deg2Rad) + 1f) / 2f);
                failed.color = c.SetAlpha(curAlpha);
                break;
        }
        if (appearTimer > 0) appearTimer++;
        if (appearTimer >= appearTime) {
            appearTimer = 0;
            DisappearAll();
        }
    }
}
