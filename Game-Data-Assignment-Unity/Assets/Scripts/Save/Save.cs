using UnityEngine;

public abstract class Save : MonoBehaviour {
    public string prefabName;
    public abstract string Serialize();
    public abstract void Deserialize(string jsonData);
}
