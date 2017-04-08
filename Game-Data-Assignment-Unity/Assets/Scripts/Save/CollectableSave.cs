using System;
using UnityEngine;

    [Serializable]
    // Note that "TankSave" is a more specific version of the base class "Save.cs". Save.cs is where "prefabName" is defined, along with the functions to be overridden.
    public class CollectableSave : Save
    {

        public Data data;
        private Collectable collectable;
        private string jsonString;

        [Serializable]
        public class Data : BaseData
        {
        // Relevant save data information. For the collectable, this is its position and motion/animation variables.
        public float lerpTime = 0;
        public bool invertMove;
        public bool collected = false;
        public float animationTime;
        public Vector3 position;
        }

        void Awake()
        {
            //Grab the needed Collectable.cs script as reference and new data on awake.
            collectable = GetComponent<Collectable>();
            data = new Data();
        }

        public override string Serialize()
        {
            // "Data" is a more finely tuned version of the "BaseData.cs" class. This is where "prefabName" comes from.
            data.prefabName = prefabName;
        // The rest of the data information comes from the class above, the position, angles, and destination.
            data.lerpTime = collectable.lerpTime;
            data.invertMove = collectable.invertMove;
            data.collected = collectable.collected;
            data.animationTime = collectable.animationTime;
            data.position = collectable.transform.position;
        // JsonUtility takes the information contained in "data" and converts it into a Json compatible string that gets passed by the return variable.
        jsonString = JsonUtility.ToJson(data);
            return (jsonString);
        }

        public override void Deserialize(string jsonData)
        {
            // the JsonUtility function pulls the original json data back, and puts it into data. From there, the position/rotation/destination/prefab name are all reset.
            JsonUtility.FromJsonOverwrite(jsonData, data);
            collectable.lerpTime = data.lerpTime;
            collectable.invertMove = data.invertMove;
            collectable.collected = data.collected;
            collectable.objAnimator.Play("Collectable", 0, data.animationTime);
            collectable.transform.position = data.position;
            collectable.name = "Collectable";
        }
    }