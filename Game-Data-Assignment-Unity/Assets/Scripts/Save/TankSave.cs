using System;
using UnityEngine;

[Serializable]
// Note that "TankSave" is a more specific version of the base class "Save.cs". Save.cs is where "prefabName" is defined, along with the functions to be overridden.
public class TankSave : Save {

    public Data data;
    private Tank tank;
    private string jsonString;

    [Serializable]
    public class Data : BaseData {
        // Relevant save data information. For the tank, this is its position, its angle, and its destination.
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 destination;
    }

    void Awake() {
        //Grab the needed Tank.cs script as reference and new data on awake.
        tank = GetComponent<Tank>();
        data = new Data();
    }

    public override string Serialize() {
        // "Data" is a more finely tuned version of the "BaseData.cs" class. This is where "prefabName" comes from.
        data.prefabName = prefabName;
        // The rest of the data information comes from the class above, the position, angles, and destination.
        data.position = tank.transform.position;
        data.eulerAngles = tank.transform.eulerAngles;
        data.destination = tank.destination;
        // JsonUtility takes the information contained in "data" and converts it into a Json compatible string that gets passed by the return variable.
        jsonString = JsonUtility.ToJson(data);
        return (jsonString);
    }

    public override void Deserialize(string jsonData) {
        // the JsonUtility function pulls the original json data back, and puts it into data. From there, the position/rotation/destination/prefab name are all reset.
        JsonUtility.FromJsonOverwrite(jsonData, data);
        tank.transform.position = data.position;
        tank.transform.eulerAngles = data.eulerAngles;
        tank.destination = data.destination;
        tank.name = "Tank";
    }
}