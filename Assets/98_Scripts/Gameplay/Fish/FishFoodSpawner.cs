using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using NaughtyAttributes;

public class FishFoodSpawner : Singleton<FishFoodSpawner>
{
    private List<Transform> fishFoodSpawner;
    private List<GameObject> fishFood;

    [MinValue(0), MaxValue(160)]
    [SerializeField] private int spawningPointsActive;

    private void Awake()
    {
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] != Charakter.Fish)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        fishFoodSpawner = new List<Transform>();
        fishFood = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            fishFoodSpawner.Add(transform.GetChild(i));
        }
    }

    void Update()
    {
        if(fishFood.Count < spawningPointsActive)
        {
            SpawningFood();
        }
    }

    private void SpawningFood()
    {
        List<Transform> temp = fishFoodSpawner;

        while(fishFood.Count < spawningPointsActive)
        {
            Transform tempTransform = temp[Random.Range(0, temp.Count - 1)];
            temp.Remove(tempTransform);
            GameObject food = PhotonNetwork.Instantiate("FishGame/FishFood", tempTransform.position, Quaternion.identity);
            food.transform.parent = tempTransform.transform;
            fishFood.Add(food);
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
