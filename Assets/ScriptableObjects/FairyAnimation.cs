using System;
using UnityEngine;


[CreateAssetMenu(fileName = "FairyAnimation", menuName = "FairyAnimation", order = 1)]
public class FairyAnimation : ScriptableObject
{
    [Serializable]
    public struct FairyAnimSeq
    {
        public FairyAnimator.FairyType type;
        public Sprite[] animSequence;
    }
        
    [SerializeField]
    private FairyAnimSeq[] fairyAnimSequences;
    public Sprite[] GetFairyAnimSequences(FairyAnimator.FairyType type) {
        return fairyAnimSequences[(int) type].animSequence;
    }
}
