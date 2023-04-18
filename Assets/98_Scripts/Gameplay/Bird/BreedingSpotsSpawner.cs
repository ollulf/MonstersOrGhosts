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
        timer = new Timer(spawningTime, true);
        breedingSpots = new List<Transform>();
        foreach (Transform child in transform)
        {
            breedingSpots.Add(child);
        }
        if((Charakter)PhotonNetwork.LocalPlayer.CustomProperties["PlayerCharakter"] != Charakter.ArcticTern)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
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
        int rand = Random.Range(1, breedingSpots.Count);

        for (int i = 0; i < rand; i++)
        {
            int index = Random.Range(0, breedingSpots.Count);

            if (breedingSpots[index].childCount == 0)
            {
                Transform tempTrans = breedingSpots[index].transform;
                GameObject breedingSpot = PhotonNetwork.Instantiate("BirdGame/BirdBreedingSpot", tempTrans.position,
                    Quaternion.Euler( tempTrans.rotation.eulerAngles.x, Random.Range(0f,360f) , tempTrans.rotation.eulerAngles.z));
                breedingSpot.transform.parent = tempTrans;
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
