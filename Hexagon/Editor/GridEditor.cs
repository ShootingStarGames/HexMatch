using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexGrid))]
public class GridEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Create"))
        {
            HexGrid.Instance.CreateCells();
        }
        if (GUILayout.Button("Remove"))
        {
            HexGrid.Instance.Clear();
        }
        if (GUILayout.Button("Save"))
        {
            GameDataManager.Instance.SaveData(HexGrid.Instance.GetBool());
        }
        if (GUILayout.Button("Load"))
        {
            HexGrid.Instance.CreateCells(GameDataManager.Instance.LoadData());
        }

    }


}
