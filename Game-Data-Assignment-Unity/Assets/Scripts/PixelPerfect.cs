using UnityEngine;

public class PixelPerfect : MonoBehaviour {

    public float pixelsToUnits = 64f;      // the sprite pixel to units setting

    void Awake() {
        // set the camera's orthographic size to achieve a 1-for-1 pixel scale
        Camera.main.orthographicSize = Screen.height / pixelsToUnits / 2.0f;
    }
}
