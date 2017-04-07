using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {



    public GameObject bulletPrefab;
    public GameObject player;

    public GameObject barrel;

    public float firingThreshold;

    public float angleToPlayer;

    public float firingDelay;

    float lastShotTime;

    // Use this for initialization
    void Start() {
        player = GameObject.Find("Tank");
    }

    // Update is called once per frame
    void Update() {
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
