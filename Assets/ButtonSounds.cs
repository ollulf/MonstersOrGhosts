using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent (typeof(AudioSource))]
[RequireComponent (typeof(EventTrigger))]
public class ButtonSounds : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] public AudioClip buttonHoverSound, buttonClickSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void ButtonHover()
    {
        audioSource.PlayOneShot(buttonHoverSound);
    }

    public void ButtonClick()
    {
        audioSource.PlayOneShot(buttonClickSound);
    }
}
