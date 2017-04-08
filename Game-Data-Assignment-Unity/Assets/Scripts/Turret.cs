using UnityEngine;

public class Turret : MonoBehaviour {

    public GameObject bulletPrefab;
    public GameObject player;
    public GameObject barrel;

    public float firingThreshold;
    public float angleToPlayer;
    public float firingDelay;
    float lastShotTime;

    void Start() {
        GetTank();
    }

    void GetTank() {
        player = GameObject.Find("Tank");
    }

    void Update() {
        if (player == null) {
            GetTank();
            return;
        }

        if (Vector3.Distance(transform.position, player.transform.position) < firingThreshold) {
            RotateBarrel();
            Fire();
        }
    }


    void RotateBarrel() {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        barrel.transform.eulerAngles = new Vector3(0, 0, angleToPlayer);
    }

    void Fire() {
        if (Time.time - lastShotTime > firingDelay) {

            GameObject bullet = Instantiate(bulletPrefab, transform.position, barrel.transform.localRotation) as GameObject;
            bullet.transform.parent = transform;
            lastShotTime = Time.time;
        }

    }
}
