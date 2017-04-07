using UnityEngine;

public class Bullet : MonoBehaviour {

    public float speed;
    Vector3 forward;

    public float lifetime;
    public float birthTime;

    void Start() {
        birthTime = Time.time;
        forward = new Vector3(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad), 0);
    }

    void Update() {
        Debug.DrawRay(transform.position, forward, Color.blue);
        transform.Translate(forward * speed * Time.deltaTime, Space.World);

        if (Time.time - birthTime > lifetime) {
            Destroy(gameObject);
        }
    }
}
