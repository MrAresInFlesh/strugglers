using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Toggles : MonoBehaviour
{
    public Toggle autoUpdateToggle;
    public Toggle fallOffToggle;

    // Start is called before the first frame update
    void Start()
    {
        autoUpdateToggle.onValueChanged.AddListener(delegate { setUpdateMapGeneration(autoUpdateToggle.isOn); });
        fallOffToggle.onValueChanged.AddListener(delegate { setFalloffToggle(fallOffToggle.isOn); });
    }

    public void setUpdateMapGeneration(bool isOn)
    {
        Simulation.mapGenerator.autoUpdate = isOn;
        Simulation.mapGenerator.drawMap();
    }

    public void setFalloffToggle(bool isOn)
    {
        Simulation.mapGenerator.useFalloff = isOn;
        Simulation.mapGenerator.drawMap();
    }
}
