using System;
using UnityEngine;

[Serializable]
public class BulletSave : Save {

    public Data data;
    private Bullet bullet;
    private string jsonString;

    [Serializable]
    public class Data : BaseData {
        public Vector3 position;
        public Vector3 eulerAngles;
        public float speed;
        public float lifetime;
        public float birthTime;
    }

    void Awake() {
        bullet = GetComponent<Bullet>();
        data = new Data();
    }

    public override string Serialize() {
        data.prefabName = prefabName;
        data.position = bullet.transform.position;
        data.eulerAngles = bullet.transform.eulerAngles;
        data.speed = bullet.speed;
        data.lifetime = bullet.lifetime;
        data.birthTime = bullet.birthTime;
        jsonString = JsonUtility.ToJson(data);
        return (jsonString);
    }

    public override void Deserialize(string jsonData) {
        JsonUtility.FromJsonOverwrite(jsonData, data);
        bullet.speed = data.speed;
        bullet.lifetime = data.lifetime;
        bullet.birthTime = data.birthTime;
        bullet.transform.position = data.position;
        bullet.transform.eulerAngles = data.eulerAngles;
    }
}
