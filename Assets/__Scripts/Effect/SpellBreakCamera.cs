using UnityEngine;

public class SpellBreakCamera : MonoBehaviour {
    public Camera cam => GetComponent<Camera>();
    public void Shot() => cam.Render();
}
