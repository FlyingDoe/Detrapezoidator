using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Calibrator))]
public class CalibratorEditor : Editor
{

    public override void OnInspectorGUI()
    {
        Calibrator myScript = (Calibrator)target;

        if (GUILayout.Button("Empty calibrator"))
        {
            myScript.CleanCalibrator();
        }

        DrawDefaultInspector();

        if (GUILayout.Button("Setup screen"))
        {
            //Undo.RecordObject(target, "test");
            myScript.SetupCalibrator();
        }
    }
}
