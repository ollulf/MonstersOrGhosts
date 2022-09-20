using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataVisualisationController : MonoBehaviour
{
    [SerializeField] Texture2D transparent;
    [SerializeField] Material material;
    [SerializeField, Range(0,10f)] float transitionDuration = 0.4f;

    DataLayer activeLayer;
    internal void Toggle(DataLayer layer)
    {
        if (activeLayer == layer)
            layer = null;

        PlayTransition(activeLayer, layer);
        activeLayer = layer;
    }
    private void PlayTransition(DataLayer before, DataLayer after)
    {
        Texture2D beforeTex = before == null ? transparent : before.Texture;
        Texture2D afterTex = after == null ? transparent : after.Texture;

        UpdateMaterialTextures(beforeTex, afterTex);

        StopAllCoroutines();
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        float lerp = 0f;

        while (lerp < 1f)
        {
            lerp += Time.deltaTime / transitionDuration;
            UpdateMaterialBlend(lerp);
            yield return null;
        }

        UpdateMaterialBlend(1f);
    }

    private void UpdateMaterialBlend(float lerp)
    {
        material.SetFloat("_blend", lerp);
    }
    private void UpdateMaterialTextures(Texture2D before, Texture2D after)
    {
        material.SetTexture("_MainTex", before);
        material.SetTexture("_BlendTex", after);
    }
}
