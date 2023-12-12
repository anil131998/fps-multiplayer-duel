using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceSettings : MonoBehaviour
{
    private float joySensitivity_L;
    private float joySensitivity_R;

    [SerializeField] private Slider joySlider_L;
    [SerializeField] private Slider joySlider_R;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        joySensitivity_L = 0;
        joySensitivity_R = 0;

        InitializeValues();
        UpdateUI();
    }

    private void InitializeValues()
    {
        if (!PlayerPrefs.HasKey("LJOY_SENSITIVITY"))
        {
            PlayerPrefs.SetFloat("LJOY_SENSITIVITY", joySensitivity_L);
        }
        else
        {
            joySensitivity_L = PlayerPrefs.GetFloat("LJOY_SENSITIVITY");
        }

        if (!PlayerPrefs.HasKey("RJOY_SENSITIVITY"))
        {
            PlayerPrefs.SetFloat("RJOY_SENSITIVITY", joySensitivity_R);
        }
        else
        {
            joySensitivity_R = PlayerPrefs.GetFloat("RJOY_SENSITIVITY");
        }
    }

    private void UpdateUI()
    {
        joySlider_L.value = joySensitivity_L;
        joySlider_R.value = joySensitivity_R;
    }

    public void LJoy_SensitivityChanged(float value)
    {
        joySensitivity_L = value;
    }

    public void RJoy_SensitivityChanged(float value)
    {
        joySensitivity_R = value;
    }

    private void SaveValues()
    {
        PlayerPrefs.SetFloat("LJOY_SENSITIVITY", joySensitivity_L);
        PlayerPrefs.SetFloat("RJOY_SENSITIVITY", joySensitivity_R);
    }


    public void CloseSettings()
    {
        SaveValues();
        settingsPanel.SetActive(false);
    }

    private void OnDisable()
    {
        SaveValues();
    }
}
