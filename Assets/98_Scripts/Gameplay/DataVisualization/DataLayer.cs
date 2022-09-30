using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLayer : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;

    [SerializeField, ReadOnly] Material materialInstance;

    [SerializeField, ReadOnly] float currentOpacity = 0f;
    [SerializeField, ReadOnly] bool isActive;

    private const float TRANSITION_DURATION = 0.3f;
    private void Awake() => materialInstance = meshRenderer.material;
    public void UIToggle()
    {
        PlayTransition(isActive ? 0f : 1f);
        isActive = !isActive;
    }
    private void PlayTransition(float targetOpacity)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionRoutine(targetOpacity));
    }

    private IEnumerator TransitionRoutine(float targetOpacity)
    {
        meshRenderer.enabled = true;

        while (Mathf.Abs(currentOpacity - targetOpacity) > 0)
        {
            currentOpacity = Mathf.MoveTowards(currentOpacity, targetOpacity, Time.deltaTime / TRANSITION_DURATION);
            UpdateMaterialBlend(currentOpacity);
            yield return null;
        }

        UpdateMaterialBlend(targetOpacity);

        if (targetOpacity == 0)
            meshRenderer.enabled = false;
    }

    private void UpdateMaterialBlend(float lerp)
    {
        materialInstance.SetFloat("_blend", lerp);
    }
}
