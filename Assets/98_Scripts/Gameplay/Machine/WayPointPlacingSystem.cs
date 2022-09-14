using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;

public class WayPointPlacingSystem : MonoBehaviour
{
    [SerializeField] private int numberOfWayPoints, remove;
    [SerializeField] private List<GameObject> wayPoints;

    public List<GameObject> WayPoints { get => wayPoints; }


    public List<GameObject> GetWayPoints()
    {
        return WayPoints;
    }

    public Transform GetStartPoint()
    {
        return WayPoints[0].transform;
    }

#if UNITY_EDITOR

    [Button]
    private void CreateWayPoints()
    {
        if (numberOfWayPoints >= 2)
        {
            ClearAllWayPoints();
            for (int i = 0; i < numberOfWayPoints; i++)
            {
                GameObject wP = new GameObject();
                if (i == 0)
                {
                    wP.name = "Start";
                    DrawIcon(wP, 3);
                }
                else if (i > 0 && i < numberOfWayPoints - 1)
                {
                    wP.name = "WayPoint" + (i);
                    DrawIcon(wP, 5);
                }
                else
                {
                    wP.name = "End";
                    DrawIcon(wP, 6);
                }
                wP.transform.parent = this.gameObject.transform;

                wayPoints.Add(wP);
            }
            UnityEditorInternal.ComponentUtility.CopyComponent(this);
            UnityEditorInternal.ComponentUtility.PasteComponentValues(this);
        }
        else
        {
            Debug.LogError("!You need 2 or more Waypoints!");
        }
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

    [Button]
    private void RemoveOneWayPoint()
    {
        List<GameObject> copy = new List<GameObject>(wayPoints);

        if (remove > 0 && remove < wayPoints.Count - 1)
        {
            DestroyImmediate(wayPoints[remove]);
            copy.RemoveAt(remove);
            wayPoints.Clear();
            wayPoints = new List<GameObject>(copy);
            RearangeWaypoints();
        }
        else
        {
            Debug.LogWarning("No Way Points to remove!");
        }
    }

    private void RearangeWaypoints()
    {
        for (int i = 0; i < wayPoints.Count; i++)
        {
            if (i == 0)
            {
                wayPoints[i].name = "Start";
                DrawIcon(wayPoints[i], 3);
            }
            else if (i > 0 && i < wayPoints.Count - 1)
            {
                wayPoints[i].name = "WayPoint" + (i);
                DrawIcon(wayPoints[i], 5);
            }
            else
            {
                wayPoints[i].name = "End";
                DrawIcon(wayPoints[i], 6);
            }
        }
        UnityEditorInternal.ComponentUtility.CopyComponent(this);
        UnityEditorInternal.ComponentUtility.PasteComponentValues(this);
    }

    [Button]
    private void ClearAllWayPoints()
    {
        foreach (GameObject wP in wayPoints)
        {
            DestroyImmediate(wP.gameObject);
        }
        wayPoints.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (wayPoints.Count > 0)
        {
            for (int i = 0; i < wayPoints.Count; i++)
            {
                if (i < wayPoints.Count - 1)
                {
                    Gizmos.DrawLine(wayPoints[i].transform.position, wayPoints[i + 1].transform.position);
                }
            }
        }
    }
#endif
}
