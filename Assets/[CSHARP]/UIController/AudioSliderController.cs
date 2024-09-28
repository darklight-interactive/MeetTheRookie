using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AudioSliderController : MonoBehaviour
{
    private SliderInt music;
    private SliderInt sfx;
    private SliderInt dialogue;
    private List<SliderInt> volumeSliders;

    private GameUIController gameUIController;

    void OnEnable()
    {
        // GameUIController
        gameUIController = GetComponent<GameUIController>();
        if (gameUIController == null)
        {
            Debug.LogWarning("Cannot find the GameUIController, volume sliders are broken");
            return;
        }

        // Sliders
        volumeSliders = new List<SliderInt>();
        //music = gameUIController.root.Q<SliderInt>("Music");
        music = gameUIController.root.Q<VisualElement>("Settings").Q<SliderInt>("Music");
        volumeSliders.Add(music);
        //sfx = gameUIController.root.Q<SliderInt>("Sounds");
        sfx = gameUIController.root.Q<VisualElement>("Settings").Q<SliderInt>("Sounds");
        volumeSliders.Add(sfx);
        //dialogue = gameUIController.root.Q<SliderInt>("Dialogue");
        dialogue = gameUIController.root.Q<VisualElement>("Settings").Q<SliderInt>("Dialogue");
        volumeSliders.Add(dialogue);
        foreach (SliderInt slider in volumeSliders)
        {
            Debug.Log($"[AudioSliderController] Loaded in the slider: {slider}");
            slider.value = 100;
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
        //float FMODValue = (0.9f * value2) - 80.0f; // Value is between -80 and 10 for FMODExt_Bus `volume` variable
        float FMODValue = value2 / 100.0f; // Value is between 0 and 1

        Debug.Log($"Slider {sliderIndex} Integer Value: {FMODValue}");

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