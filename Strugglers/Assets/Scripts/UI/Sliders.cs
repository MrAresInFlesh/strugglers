using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sliders : MonoBehaviour
{
    public Slider sliderInputNoise;
    public Slider sliderInputOctaves;
    public Slider sliderInputSeed;
    public Slider sliderInputHeight;
    public Slider sliderInputPersistance;
    public Slider sliderInputLacunarity;

    public Text valueNoise;
    public Text valueOctaves;
    public Text valueSeed;
    public Text valueHeight;
    public Text valuePersistance;
    public Text valueLacunarity;

    // Start is called before the first frame update
    void Start()
    {
        sliderInputNoise.onValueChanged.AddListener(delegate { setNoiseInput(sliderInputNoise.value); });
        sliderInputOctaves.onValueChanged.AddListener(delegate { setOctavesInput((int)sliderInputOctaves.value); });
        sliderInputSeed.onValueChanged.AddListener(delegate { setSeedInput((int)sliderInputSeed.value); });
        sliderInputHeight.onValueChanged.AddListener(delegate { setHeightInput(sliderInputHeight.value); });
        sliderInputPersistance.onValueChanged.AddListener(delegate { setPersistanceInput(sliderInputPersistance.value); });
        sliderInputLacunarity.onValueChanged.AddListener(delegate { setLacunarityInput(sliderInputLacunarity.value); });

        valueNoise.text = sliderInputNoise.value.ToString();
        valueSeed.text = sliderInputSeed.value.ToString();
        valueOctaves.text = sliderInputOctaves.value.ToString();
        valueHeight.text = sliderInputHeight.value.ToString();
        valuePersistance.text = sliderInputPersistance.value.ToString();
        valueLacunarity.text = sliderInputLacunarity.value.ToString();
    }

    public void Update()
    {
        if (WorldClock.timer != 0)
        {
            sliderInputNoise.enabled = false;
            sliderInputOctaves.enabled = false;
            sliderInputSeed.enabled = false;
            sliderInputHeight.enabled = false;
            sliderInputPersistance.enabled = false;
            sliderInputLacunarity.enabled = false;
        }
        if (WorldClock.timer != 0)
        {
            sliderInputNoise.enabled = true;
            sliderInputOctaves.enabled = true;
            sliderInputSeed.enabled = true;
            sliderInputHeight.enabled = true;
            sliderInputPersistance.enabled = true;
            sliderInputLacunarity.enabled = true;
        }
    }

    public void setNoiseInput(float noiseInput)
    {
        Simulation.mapGenerator.noise = noiseInput;
        valueNoise.text = noiseInput.ToString();
        if (Simulation.mapGenerator.autoUpdate) Simulation.mapGenerator.drawMap();
    }

    public void setSeedInput(int seedInput)
    {
        Simulation.mapGenerator.seed = seedInput;
        valueSeed.text = seedInput.ToString();
        if (Simulation.mapGenerator.autoUpdate) Simulation.mapGenerator.drawMap();
    }

    public void setOctavesInput(int octavesInput)
    {
        Simulation.mapGenerator.octaves = octavesInput;
        valueOctaves.text = octavesInput.ToString();
        if (Simulation.mapGenerator.autoUpdate) Simulation.mapGenerator.drawMap();
    }

    public void setHeightInput(float height)
    {
        Simulation.mapGenerator.meshHeightFactor = height;
        valueHeight.text = height.ToString();
        if (Simulation.mapGenerator.autoUpdate) Simulation.mapGenerator.drawMap();
    }

    private void setLacunarityInput(float lacunarityInput)
    {
        Simulation.mapGenerator.lacunarity = lacunarityInput;
        valueLacunarity.text = lacunarityInput.ToString();
        if (Simulation.mapGenerator.autoUpdate) Simulation.mapGenerator.drawMap();
    }

    private void setPersistanceInput(float persistanceInput)
    {
        Simulation.mapGenerator.persistance = persistanceInput;
        valuePersistance.text = persistanceInput.ToString();
        if (Simulation.mapGenerator.autoUpdate) Simulation.mapGenerator.drawMap();
    }
}
