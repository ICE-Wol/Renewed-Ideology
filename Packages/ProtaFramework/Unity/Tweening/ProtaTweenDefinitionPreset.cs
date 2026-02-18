using UnityEngine;
using System;

namespace Prota.Unity
{
    [CreateAssetMenu(menuName = "Prota Framework/Tweener Def", fileName = "ProtaTweenerDefinition")]
    public class ProtaTweenDefinitionPreset : ScriptableObject
    {
        [SerializeReference] public TweenDefinition definition = new TweenDefinition();
    }
}
