using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using NaughtyAttributes;

public class FoodSpotHandler : MonoBehaviourPun
{
    [SerializeField] private int minFoodCapacity = 500, maxFoodCapacity = 1000; 
    [SerializeField] private float spawningTime;
    [SerializeField] private int onStartSpawnAmount = 10;

    private List<Transform> foodSpots;

    private Timer timer;

    private float currentTempIncrease;

    [ShowNonSerializedField] private int foodCapacity;

    private void Awake()
    {
        if ((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] != Charakter.ArcticTern)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        timer = new Timer(0, true);
        foodSpots = new List<Transform>();
        foodCapacity = Random.Range(minFoodCapacity, maxFoodCapacity);
        foreach (Transform child in transform)
        {
            foodSpots.Add(child);
        }
        
        for(int i = 0; i < onStartSpawnAmount; i++)
        {
            SpawningFeedSpots();
        }
    }

    void Update()
    {
        GetCurrentTemp();

        timer.Tick();
        if (timer.CurrentTime <= 0)
        {
            SpawningFeedSpots();
            foodCapacity = Random.Range(minFoodCapacity, maxFoodCapacity);
            timer.SetNewTime(60 / 1 + currentTempIncrease, true);
            timer.ResetTimer();
        }
    }

    private void SpawningFeedSpots()
    {
        int rand = Random.Range(1, foodSpots.Count);

        for (int i = 0; i < rand; i++)
        {
            int index = Random.Range(0, foodSpots.Count);

            if (foodSpots[index].childCount == 0)
            {
                GameObject foodSpot = PhotonNetwork.Instantiate("BirdGame/BirdFood", foodSpots[index].transform.position, Quaternion.identity);
                foodSpot.transform.parent = foodSpots[index].transform;
                foodSpot.GetComponent<BirdFood>().SetFoodAmount(foodCapacity);
            }
        }
    }

    private void GetCurrentTemp() => currentTempIncrease = TempretureHandler.Tempreture;

#if UNITY_EDITOR

    [Button]
    private void CreatePlace()
    {
        GameObject place = new GameObject("ArcticTern Food Spot");
        place.transform.parent = this.gameObject.transform;

        DrawIcon(place, 4);
        UnityEditorInternal.ComponentUtility.CopyComponent(this);
        UnityEditorInternal.ComponentUtility.PasteComponentValues(this);
    }


    private void DrawIcon(GameObject gameObject, int idx)
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
