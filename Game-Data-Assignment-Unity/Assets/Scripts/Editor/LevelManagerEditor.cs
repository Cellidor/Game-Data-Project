using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor {

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        LevelManager myScript = (LevelManager)target;
        if (GUILayout.Button("Load Level")) {
            myScript.LoadLevel();
        }
    }
}