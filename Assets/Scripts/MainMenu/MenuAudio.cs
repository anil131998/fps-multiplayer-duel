using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundTheme;

    private void Start()
    {
        backgroundTheme.Play();
    }
}
