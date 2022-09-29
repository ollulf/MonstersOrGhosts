using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using NaughtyAttributes;
using UnityEditor;

public class BreedingSpotsSpawner : MonoBehaviourPun
{
    [SerializeField] float spawningTime;

    private List<Transform> breedingSpots;
    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = new Timer();
        timer.SetStartTime(spawningTime, true);
        breedingSpots = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            breedingSpots.Add(transform.GetChild(i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer.Tick();
        if (timer.CurrentTime <= 0)
        {
            SpawningBreedingSpot();
            timer.ResetTimer();
        }
    }

    private void SpawningBreedingSpot()
    {
        foreach (Transform child in breedingSpots)
        {
            if (child.childCount == 0)
            {
                GameObject breedingSpot = PhotonNetwork.Instantiate("BirdGame/BirdBreedingSpot", child.transform.position, Quaternion.identity);
                breedingSpot.transform.parent = child.transform;
                return;
            }
        }
    }
#if UNITY_EDITOR

    [Button]
    private void CreatePlace()
    {
        GameObject place = new GameObject("Breeding Spot");
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
