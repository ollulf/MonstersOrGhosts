using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using NaughtyAttributes;

public class FishFoodSpawner : MonoBehaviourPun
{
    [SerializeField] float spawningTime;

    private List<Transform> fishFoodSpawner;
    private Timer timer;

    void Start()
    {
        timer = new Timer();
        timer.SetStartTime(spawningTime, true);
        fishFoodSpawner = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            fishFoodSpawner.Add(transform.GetChild(i));
        }
    }

    void Update()
    {
        timer.Tick();
        if (timer.CurrentTime <= 0)
        {
            SpawningFood();
            timer.ResetTimer();
        }
    }

    private void SpawningFood()
    {
        foreach (Transform child in fishFoodSpawner)
        {
            if (child.childCount == 0)
            {
                GameObject food = PhotonNetwork.Instantiate("FishGame/FishFood", child.transform.position, Quaternion.identity);
                food.transform.parent = child.transform;
                return;
            }
        }
    }
#if UNITY_EDITOR

    [Button]
    private void CreateFishFoodPlace()
    {
        GameObject place = new GameObject("FoodSpawnPlace");
        place.transform.parent = this.gameObject.transform;

        DrawIcon(place, 4);
        UnityEditorInternal.ComponentUtility.CopyComponent(this);
        UnityEditorInternal.ComponentUtility.PasteComponentValues(this);
    }


    private void DrawIcon(GameObject gameObject,int idx)
    {
        GUIContent[] icons = GetTextures("sv_label_", string.Empty, 0, 8);
        GUIContent icon = icons[idx];
        EditorGUIUtility.SetIconForObject(gameObject, (Texture2D)icon.image);
    }

    private GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
    {
        GUIContent[] Iconarray = new GUIContent[count];
        for (int i = 0; i < count; i++)
        {
            Iconarray[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
        }
        return Iconarray;
    }
#endif
}
