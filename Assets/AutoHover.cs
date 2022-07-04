using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class AutoHover : MonoBehaviour
{
    [SerializeField] AnimationCurve curve;
    [SerializeField] float speed = 1f, hoverAmount = 1f;

    private float time = 0;
    private Vector3 localTransform;


    private void Start()
    {
        localTransform = gameObject.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        time = time % 1;

        transform.position = new Vector3(transform.position.x, localTransform.y + curve.Evaluate(time * speed) * hoverAmount, transform.position.z);
    }
}
