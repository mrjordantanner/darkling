using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(SpawnerController))]
public class SpawnerControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpawnerController controller = (SpawnerController)target;

        if (GUILayout.Button("Populate Groups"))
        {
            controller.PopulateGroups();
        }

        if (GUILayout.Button("Clear Groups"))
        {
            controller.ClearGroups();
        }

        if (GUILayout.Button("Spawn Enemies"))
        {
            controller.Spawn();
        }

        if (GUILayout.Button("Clear Enemies"))
        {
            controller.DestroyEnemyChildren();

        }

        /*
        if (GUILayout.Button("Spawn Treasure"))
        {
            controller.StartTreasureSpawn();
        }

        if (GUILayout.Button("Clear Treasure"))
        {
            controller.DestroyTreasureChildren();
            controller.TreasurePool.Clear();
        }
        */

    }




}
