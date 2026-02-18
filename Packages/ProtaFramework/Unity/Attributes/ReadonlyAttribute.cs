using UnityEngine;
using UnityEditor;

namespace Prota.Unity
{
    public class Inspect : PropertyAttribute
    {
        public bool whenPlaying = true;
        public bool whenEditing = true;
        public bool hideWhenEditing = false;
        public bool hideWhenPlaying = false;
    }
 
}
