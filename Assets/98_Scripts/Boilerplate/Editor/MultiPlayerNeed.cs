using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


public class MultiPlayerNeed : Editor
{
    [MenuItem("Level/Needed")]
    private static void SetupLevel()
    {
        GameObject MultiplayerTools = new GameObject("-------MultiplayerTools-------");

        PrefabUtility.InstantiatePrefab(Resources.Load("MultiplayerNeeded/BalancingInterface"),MultiplayerTools.transform);
        PrefabUtility.InstantiatePrefab(Resources.Load("MultiplayerNeeded/BirdContent"), MultiplayerTools.transform);
        PrefabUtility.InstantiatePrefab(Resources.Load("MultiplayerNeeded/DeerContent"), MultiplayerTools.transform);
        PrefabUtility.InstantiatePrefab(Resources.Load("MultiplayerNeeded/FishFoodSpawner"), MultiplayerTools.transform);
        PrefabUtility.InstantiatePrefab(Resources.Load("MultiplayerNeeded/MultiPlayerInstatiate"), MultiplayerTools.transform);
        PrefabUtility.InstantiatePrefab(Resources.Load("MultiplayerNeeded/OilCraterGenerator"), MultiplayerTools.transform);
        PrefabUtility.InstantiatePrefab(Resources.Load("MultiplayerNeeded/OverviewPosition"), MultiplayerTools.transform);
        PrefabUtility.InstantiatePrefab(Resources.Load("MultiplayerNeeded/PlayerBaseDataHandler"), MultiplayerTools.transform);
        PrefabUtility.InstantiatePrefab(Resources.Load("MultiplayerNeeded/PlayerSpawner"), MultiplayerTools.transform);
        PrefabUtility.InstantiatePrefab(Resources.Load("MultiplayerNeeded/RefineryHandler"), MultiplayerTools.transform);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}
