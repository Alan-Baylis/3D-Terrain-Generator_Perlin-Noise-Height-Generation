using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*Creates a editor window in the Unity Inspector for the Noise Map Generator Script*/

[CustomEditor(typeof (NoiseMapGenerator))] 
public class MapEditor : Editor {

    public override void OnInspectorGUI()
    {
        NoiseMapGenerator mapGen = (NoiseMapGenerator)target;
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.DrawMapInEditor();
            }
        }

        if (GUILayout.Button("Generate Map"))
        {
            mapGen.DrawMapInEditor();
        }
    }
}
