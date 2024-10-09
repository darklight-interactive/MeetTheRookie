using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioSliderController : MonoBehaviour
{
    private SliderInt music;
    private SliderInt sfx;
    private SliderInt dialogue;
    private List<SliderInt> volumeSliders;

    private PauseMenuController pauseMenuController;

    void OnEnable()
    {
        // PauseMenuController
        pauseMenuController = GetComponent<PauseMenuController>();
        if (pauseMenuController == null)
        {
            Debug.LogWarning("Cannot find the PauseMenuController, volume sliders are likely broken");
            return;
        }

        // Sliders
        volumeSliders = new List<SliderInt>();
        //music = gameUIController.root.Q<SliderInt>("Music");
        music = pauseMenuController.root.Q<VisualElement>("Settings").Q<SliderInt>("Music");
        volumeSliders.Add(music);
        //sfx = gameUIController.root.Q<SliderInt>("Sounds");
        sfx = pauseMenuController.root.Q<VisualElement>("Settings").Q<SliderInt>("Sounds");
        volumeSliders.Add(sfx);
        //dialogue = gameUIController.root.Q<SliderInt>("Dialogue");
        dialogue = pauseMenuController.root.Q<VisualElement>("Settings").Q<SliderInt>("Dialogue");
        volumeSliders.Add(dialogue);
        foreach (SliderInt slider in volumeSliders)
        {
            //Debug.Log($"[AudioSliderController] Loaded in the slider: {slider}");
            slider.value = 10; // Originally 100
        }

        // Register change event listeners
        music.RegisterValueChangedCallback(evt => SetSliderVolume(evt.newValue, 1));
        sfx.RegisterValueChangedCallback(evt => SetSliderVolume(evt.newValue, 2));
        dialogue.RegisterValueChangedCallback(evt => SetSliderVolume(evt.newValue, 3));
    }

    private void SetSliderVolume(int value, int sliderIndex)
    {
        // Normalize the value to a range suitable for FMOD
        float value2 = (float)value;
        float FMODValue = value2 / 10.0f; // Value needs to be between 0 and 1
        //float FMODValue = (0.9f * value2) - 80.0f; // Value is between -80 and 10 for FMODExt_Bus `volume` variable
        //Debug.Log($"Slider {sliderIndex} Integer Value: {FMODValue}");

        // Set the relevant FMOD bus volume
        // Currently updates the bus directly, other option is to update the FMODExt_Bus `volume` variable and let its (currently disaled) update function set the volume
        if (sliderIndex == 1)
        {
            FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
            bus.setVolume(FMODValue);
        }
        else if (sliderIndex == 2)
        {
            FMOD.Studio.Bus bus1 = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
            FMOD.Studio.Bus bus2 = FMODUnity.RuntimeManager.GetBus("bus:/Ambience");
            bus1.setVolume(FMODValue);
            bus2.setVolume(FMODValue);
        }
        else if (sliderIndex == 3)
        {
            FMOD.Studio.Bus bus = FMODUnity.RuntimeManager.GetBus("bus:/Dialogue");
            bus.setVolume(FMODValue);
        }
    }
}