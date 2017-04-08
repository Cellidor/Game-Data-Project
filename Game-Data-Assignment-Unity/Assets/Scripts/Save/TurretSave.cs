using System;
using UnityEngine;

[Serializable]
public class TurretSave : Save {

    public Data data;
    private Turret turret;
    private string jsonString;

    [Serializable]
    public class Data : BaseData {
        public Vector3 position;
        public Vector3 eulerAngles;
    }

    void Awake() {
        turret = GetComponent<Turret>();
        data = new Data();
    }

    public override string Serialize() {
        data.prefabName = prefabName;
        data.position = turret.transform.position;
        data.eulerAngles = turret.transform.eulerAngles;
        jsonString = JsonUtility.ToJson(data);
        return (jsonString);
    }

    public override void Deserialize(string jsonData) {
        JsonUtility.FromJsonOverwrite(jsonData, data);
        turret.transform.position = data.position;
        turret.transform.eulerAngles = data.eulerAngles;

        Transform turretParent = GameObject.Find("Turrets").transform;
        turret.transform.parent = turretParent;
    }
}
