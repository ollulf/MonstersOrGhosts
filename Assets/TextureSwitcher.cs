using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSwitcher : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;

    [SerializeField] Texture2D[] textures;
    [SerializeField, ReadOnly] Material materialInstance;
    [ShowNonSerializedField] float currentSin;
    [SerializeField] WorldTime worldTime;

    public int yearToStart = 1990;

    public float sinMin = -1, sinMax = 1;
    public int nextIndex = 2;
    private bool goesUp = true;
    // Start is called before the first frame update

    private void Awake() => materialInstance = meshRenderer.material;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (StartAnimating())
        {
            currentSin += goesUp ? 1 : (-1) * Time.deltaTime * (1 / 2.725f);
            materialInstance.SetFloat("_LerpValue", currentSin);
            if (goesUp)
            {
                if (currentSin >= sinMax)
                {
                    currentSin = 2 * sinMax - currentSin;
                    goesUp = false;
                    materialInstance.SetTexture("_Texture1", textures[nextIndex]);
                    nextIndex++;
                }
            }
            else if (currentSin <= sinMin)
            {
                currentSin = 2 * sinMin - currentSin;
                materialInstance.SetTexture("_Texture2", textures[nextIndex]);
                nextIndex++;
                goesUp = true;
            }

            if (nextIndex == textures.Length) nextIndex = textures.Length - 2;
        }

    }

    private bool StartAnimating() => yearToStart >= worldTime.IceYear;
}
