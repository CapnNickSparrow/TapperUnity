using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

    private void Awake()
    {
        // Gets the components which we need to use
        Mute = GameObject.Find("Toggle").GetComponent<Toggle>();
        Slider = GameObject.Find("Slider").GetComponent<Slider>();
        
        // Setting a new Slidervalue based on the last saved one and then setting the music as well
        Slider.value = PlayerPrefs.GetFloat("MasterVolume", Constants.MAX_VOLUME);
        mixer.SetFloat("MasterVolume", Mathf.Log10(Slider.value) * Constants.DECIBEL_CONVERT);
    }

    private void Start()
    {
        // Checks if the Slidervalue is higher then the minimum value so it will give the toggle the proper function
        if (Slider.value > Constants.MIN_VOLUME)
        {
            MuteAudio(!enabled);
        }
        else
        {
            MuteAudio(enabled);
        }
        
        // Sets the Menu Inactive so the volume is loaded from start but the menu isn't active right away
        GameManager.instance.Menu.SetActive(false);
    }

    // Sets the Mixer Value based on the Slider
    public void SetLevel (float sliderValue)
    {
        print("Trying to setting the slider to " + sliderValue);
        // Saves the old slider value to be reused if double tick the toggle
        if (sliderValue > Constants.MIN_VOLUME)
        {
            oldSliderValue = sliderValue;
        }
        
        // Converts the Slider Value to a Logarithm Scale and then uses it to be equal like decibels 
        PlayerPrefs.SetFloat("MasterVolume", sliderValue);
        mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * Constants.DECIBEL_CONVERT);

        // Checks if the Slidervalue is higher then the minimum value and if it is indeed muted to unmute it 
        if (sliderValue > Constants.MIN_VOLUME && Mute.isOn)
        {
            Mute.isOn = false;
        }
        
        // Checks if the Slidervalue is equal to the minimum value and if it is hasn't been muted to mute it 
        else if (sliderValue == Constants.MIN_VOLUME && !Mute.isOn)
        {
            Mute.isOn = true;
        }
    }

    // Checks if the audio has been muted
    public void MuteAudio (bool enabled)
    {
        // If it was muted use the old slider value as current value
        if (enabled == false)
        {
            Slider.value = oldSliderValue;
            PlayerPrefs.SetFloat("MasterVolume", Slider.value);
            mixer.SetFloat("MasterVolume", Mathf.Log10(Slider.value) * Constants.DECIBEL_CONVERT);
        }
        
        // If it wasn't muted set the value to the minimum
        else
        {
            mixer.SetFloat("MasterVolume", Mathf.Log10(Constants.MIN_VOLUME) * Constants.DECIBEL_CONVERT);
            
            Slider.value = Constants.MIN_VOLUME;
            
            PlayerPrefs.SetFloat("MasterVolume", Slider.value);
        }
    }
}
