using _Scripts.Tools;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SingleLineLaserCtrl : MonoBehaviour
{
    public LaserPoint laserHead;
    public LaserPoint laserRoot;

    public float length;
    public float dir;
    
    public void Update() {                                                                                                                                                                               
        laserHead.transform.position = laserRoot.transform.position + length * dir.Deg2Dir3();
        
    }

    public void SetHeadActive(bool active) {
        laserHead.isNodeActive = active;
    }
    
    public void SetRootActive(bool active) {
        laserRoot.isNodeActive = active;
    }

    public void SetHeadState(LaserState state) {
        laserHead.SetState(state);
    }
    public void SetRootState(LaserState state) {
        laserRoot.SetState(state);
    }
    
    public void SetColor(Color color) {
        laserHead.SetColor(color);
        laserRoot.SetColor(color);
    }
    
    public void SetWidth(float width) {
        laserHead.tarWidth = width;
        laserRoot.tarWidth = width;
    }
    
    public void SetAlpha(float alpha) {
        laserHead.SetAlpha(alpha);
        laserRoot.SetAlpha(alpha);
    }
}
