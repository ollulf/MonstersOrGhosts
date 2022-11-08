using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSwitcher : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;

    [SerializeField] Texture2D[] textures;
    [SerializeField, ReadOnly] Material materialInstance;
    [ShowNonSerializedField] float currentSin;

    public float sinMin = -1, sinMax = 1;
    public int nextIndex = 2;
    private bool goesUp = true;
    // Start is called before the first frame update

    private void Awake() => materialInstance = meshRenderer.material;
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentSin = Time.time % 2 / 2;
        if (!goesUp) Mathf.Clamp(currentSin = 1 - currentSin , 0 , sinMax);

        materialInstance.SetFloat("_LerpValue", currentSin);

        if (currentSin <= sinMin && !goesUp)
        {
            Debug.LogWarning("Reached sin MIN");
            materialInstance.SetTexture("_Texture2", textures[nextIndex]);
            nextIndex++;
            goesUp = true;
        }
        else if (currentSin >= sinMax && goesUp)
        {
            Debug.LogWarning("Reached sin MAX");
            materialInstance.SetTexture("_Texture1", textures[nextIndex]);
            nextIndex++;
            goesUp = false;
        }

        if(nextIndex == textures.Length) nextIndex = textures.Length - 2;
    }
}