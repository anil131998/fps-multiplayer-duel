using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FramerateManager : MonoBehaviour
{
    private float count;
    [SerializeField] TMP_Text fpsCounter;

    private void Start()
    {
        Application.targetFrameRate = 120;
        InvokeRepeating(nameof(GetFPS), 1f, 0.5f);
    }

    private void GetFPS()
    {
        count = (int)(1f / Time.unscaledDeltaTime);
        fpsCounter.text = count + "";
    }

}
