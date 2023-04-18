using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamera : MonoBehaviour
{

    [SerializeField] private LayerMask mask;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.yellow, Mathf.Infinity);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
            {
                Debug.Log(hit.collider.gameObject);

                if (hit.collider.gameObject.layer == 8)
                {
                    hit.collider.gameObject.GetComponent<ShowBuildUI>().CanvasShow();
                }
            }
        }
    }
}