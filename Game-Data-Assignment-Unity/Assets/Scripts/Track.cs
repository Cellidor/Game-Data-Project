using UnityEngine;
using System.Collections;

public class Track : MonoBehaviour {

    public float fadeRate;
    Color trackColor;
    SpriteRenderer spriteRendererComponent;

    void Awake() {
        // get references to components
        spriteRendererComponent = GetComponent<SpriteRenderer>();
        // set the track tint to white
        trackColor = new Color(1,1,1,1);
    }

    void Update() {
        Fade();
    }

    void Fade() {
        // diminish the alpha of the track by the faderate
        trackColor.a -= (1 / fadeRate) * Time.deltaTime;

        if (trackColor.a > 0) {
            // assign the new track color
            spriteRendererComponent.color = trackColor;
        } else {
            // destroy the game object if invisible
            Destroy(gameObject);
        }
    }

}
