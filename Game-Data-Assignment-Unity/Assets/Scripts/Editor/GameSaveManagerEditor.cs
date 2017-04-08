using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameSaveManager))]
public class GameSaveManagerEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GameSaveManager myScript = (GameSaveManager)target;
        if (GUILayout.Button("Save Tank")) {
            myScript.Save();
        }

        if (GUILayout.Button("Load Tank")) {
            myScript.Load();
        }
    }
}
