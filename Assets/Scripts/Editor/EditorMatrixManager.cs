using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MatrixManager))]
public class EditorMatrixManager : Editor
{
    MatrixManager myTarget;

    private void OnEnable()
    {
        myTarget = (MatrixManager)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawSaveButton();
    }

    void DrawSaveButton()
    {
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save Level", GUILayout.Height(50), GUILayout.MinWidth(20)))
        {
            if (myTarget.Storage != null)
            {
                myTarget.Storage.SaveLevel(myTarget.LG);
            }
        }


        GUILayout.EndHorizontal();
    }
}
