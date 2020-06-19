using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;
using Toggle = UnityEngine.UI.Toggle;

public class Mixer : MonoBehaviour
{
    // Connects to the Mixer
    public AudioMixer mixer;

    // Connect to the Slider
    private Slider Slider;

    // Connect to the Mute Toggle
    private Toggle Mute;
    
    // Saves the old Slider Value
    private float oldSliderValue;

    private void Start()
    {
        Mute = GameObject.Find("Toggle").GetComponent<Toggle>();
        Slider = GameObject.Find("Slider").GetComponent<Slider>();
    }

    // Sets the Mixer Value based on the Slider
    public void SetLevel (float sliderValue)
    {
        if (sliderValue > 0.0001f)
        {
            oldSliderValue = sliderValue;
        }
        
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
        
        if (sliderValue > 0.0001 && Mute.isOn)
        {
            Mute.isOn = false;
        }
        
        else if (sliderValue == 0.0001f && !Mute.isOn)
        {
            Mute.isOn = true;
        }
    }

    public void MuteAudio (bool enabled)
    {
        if (enabled == false)
        {
            SetLevel(oldSliderValue);

            Slider.value = oldSliderValue;
        }
        else
        {
            mixer.SetFloat("MasterVolume", Mathf.Log10(0.0001f) * 20);
            
            Slider.value = 0.0001f;
        }
    }
}
