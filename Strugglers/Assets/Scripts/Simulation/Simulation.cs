/**
 * Auteurs 	: 	Simon Meier, Yasmine Margueron, Simon Porret
 * Date		: 	29.08.2021
 * Version	: 	4.1.2
 */

/**
 * Core class of the program.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System;

public class Simulation : MonoBehaviour
{
    /*____________________|*______________________________*|_____________________*|
    |#####################|*		   VARIABLES          *|#####################*|
    /*____________________________________________________________________________________________________________________________________________*/

    /*____________________________________________________________________________|
    |                                   PUBLIC                                   */
    [HideInInspector] public WorldClock worldClock;

    /*____________________________________________________________________________|
    |       STATIC       */

    // Map
    public static MapGenerator mapGenerator;

    // Data used for graphs
    public static int currentNumberOfHerbivorousPopulation, currentNumberOfCarnivorousPopulation, currentNumberOfPlantPopulation;
    public static List<int> dataHerbivorous, dataCarnivorous, dataPlants;

    /*____________________________________________________________________________|
    |          UI        */

    public Dropdown herbivorousPopulationChoices, carnivorousPopulationChoices, plantsPopulationChoices, terrainComponentsChoice;
    public Slider carnivorousPopulationSlider, herbivorousPopulationSlider, plantsPopulationSlider, terrainComponentsSlider;
    public Slider numberOfDaysSlider, timeSpeedSlider;
    public Text textValueCarnivorousPopulation, textValueHerbivorousPopulation, textValuePlantsPopulation, textValueTerrainComponents;
    public Text dataDisplayValueCarnivorousPopulation, dataDisplayValueHerbivorousPopulation, dataDisplayValuePlantsPopulation;
    public Text textNumberOfDaysInput, textTimeSpeedSimulation;

    /*____________________________________________________________________________|
    |        SERIALIZE FIELD      */
    // Animals
    [SerializeField] GameObject chickenWhite, chickenBrown, cow, lion, doggo, penguin, whale;
    // Plants
    [SerializeField] GameObject grass1, grass2, flowers, tree1, tree2, tree3, tree4, bush1, bush2;
    // Terrain Components
    [SerializeField] GameObject rock1, rock2, rock3, rock4, rock5, rock6, rock7;
    // Type
    [SerializeField] GameObject herbivorous, carnivorous, plants, components;
    // Ocean
    [SerializeField] GameObject water;

    /*____________________________________________________________________________|
    |                                   PRIVATE                                  */
    
    // Collections of species
    List<GameObject> herbivorousCollections, carnivorousCollections, plantsCollections, terrainComponentsCollection;

    // Current game objects when dropdown in list change
    GameObject currentGo, currentCarnivorousGo, currentHerbivorousGo, currentPlantGo, currentTerrainGo;
    List<String> herbivorousSpecies, carnivorousSpecies, plantsSpecies, terrainComponents;

    // PS : population size
    int carnPS, herbPS, plantsPS, terrainPS;

    // IS : In Simulation
    List<GameObject> terrainComponentsIS, plantsIS, herbivorousIS, carnivorousIS;
    
    // Spawns : 
    public static List<Vector3> waterPositions, spawnPositions;

    // Animal characteristics :
    public float carnivorousSpeed = 5f, herbivorousSpeed = 2f;

    // Time variables : 
    public int numberOfDays;
    public float timeSpeedSimulation;

    /*____________________|*______________________________*|_____________________*|
    |#####################|*		     CORE             *|#####################*|
    /*____________________________________________________________________________________________________________________________________________*/

    private void Start()
    {
        /*      INITIALIZE      */
        /*_______________________________________________________________________*/

        herbivorousCollections = new List<GameObject> { chickenWhite, chickenBrown, cow };
        carnivorousCollections = new List<GameObject> { lion, doggo, penguin };
        plantsCollections = new List<GameObject> { grass1, grass2, flowers, tree1, tree2, tree3, tree4, bush1, bush2 };
        terrainComponentsCollection = new List<GameObject> { rock1, rock2, rock3, rock4, rock5, rock6, rock7 };

        herbivorousSpecies = new List<String>{ "ChickenBrown", "ChickenWhite", "Cow" };
        carnivorousSpecies = new List<String> { "Lion", "Doggo", "Penguin" };
        plantsSpecies = new List<String> { "Grass1", "Grass2", "Flowers", "Tree1", "Tree2", "Tree3", "Tree4","Bush1", "Bush2" };
        terrainComponents = new List<String> { "Rock1", "Rock2", "Rock3", "Rock4", "Rock5", "Rock6", "Rock7" };

        currentHerbivorousGo = Instantiate(chickenBrown);
        currentCarnivorousGo = Instantiate(lion);
        currentPlantGo = Instantiate(grass1);
        currentTerrainGo = Instantiate(rock1);

        mapGenerator = FindObjectOfType<MapGenerator>();
        mapGenerator.drawMap();
        mapGenerator.autoUpdate = false;

        herbivorousIS = new List<GameObject>();
        carnivorousIS = new List<GameObject>();
        plantsIS = new List<GameObject>();
        terrainComponentsIS = new List<GameObject>();

        waterPositions = mapGenerator.meshData.waterPositions;
        spawnPositions = mapGenerator.meshData.spawnPositions;

        dataHerbivorous = new List<int>();
        dataCarnivorous = new List<int>();
        dataPlants = new List<int>();

        numberOfDays = 3;
        textNumberOfDaysInput.text = numberOfDays.ToString();

        /*      LISTENERS      */
        /*_______________________________________________________________________*/

        // Sliders
        carnivorousPopulationSlider.onValueChanged.AddListener(delegate { setPopulationSizeOfCarnivorous((int)carnivorousPopulationSlider.value); });
        herbivorousPopulationSlider.onValueChanged.AddListener(delegate { setPopulationSizeOfHerbivorous((int)herbivorousPopulationSlider.value); });
        plantsPopulationSlider.onValueChanged.AddListener(delegate { setPopulationSizeOfPlants((int)plantsPopulationSlider.value); });
        terrainComponentsSlider.onValueChanged.AddListener(delegate { setPopulationSizeOfTerrainComponents((int)terrainComponentsSlider.value); });
        numberOfDaysSlider.onValueChanged.AddListener(delegate { setNumberOfDaysOfSimulation((int)numberOfDaysSlider.value); });
        timeSpeedSlider.onValueChanged.AddListener(delegate { setTimeSpeedOfSimulation(timeSpeedSlider.value); });

        // Dropdowns
        herbivorousPopulationChoices.onValueChanged.AddListener(delegate { handleInputHerbivorousPopulation(herbivorousPopulationChoices.value); });
        carnivorousPopulationChoices.onValueChanged.AddListener(delegate { handleInputCarnivorousPopulation(carnivorousPopulationChoices.value); });
        plantsPopulationChoices.onValueChanged.AddListener(delegate { handleInputPlantsPopulation(plantsPopulationChoices.value); });
        terrainComponentsChoice.onValueChanged.AddListener(delegate { handleInputTerrainComponents(terrainComponentsChoice.value); });

        foreach (String specie in herbivorousSpecies) { herbivorousPopulationChoices.options.Add(new Dropdown.OptionData(specie)); }
        foreach (String specie in carnivorousSpecies) { carnivorousPopulationChoices.options.Add(new Dropdown.OptionData(specie)); }
        foreach (String specie in plantsSpecies) { plantsPopulationChoices.options.Add(new Dropdown.OptionData(specie)); }
        foreach (String component in terrainComponents) { terrainComponentsChoice.options.Add(new Dropdown.OptionData(component)); }

        worldClock = new WorldClock(24f, 30f);
    }

    public void Update()
    {
        if (WorldClock.active || !WorldClock.paused)
        {
            numberOfDaysSlider.enabled = false;
            timeSpeedSlider.enabled = false;

            updateDeathInSimulation();
            if (numberOfDays == worldClock.numberOfDays)
            {
                Debug.Log(worldClock.numberOfDays);
                WorldClock.timer = 0;
                WorldClock.active = false;
            }
        }

        if (!WorldClock.active)
        {
            numberOfDaysSlider.enabled = true;
            timeSpeedSlider.enabled = true;
        }
    }


    /*____________________|*______________________________*|_____________________*|
    |#####################|*		   METHODS            *|#####################*|
    /*____________________________________________________________________________________________________________________________________________*/

    private void updateDeathInSimulation()
    {
        currentNumberOfCarnivorousPopulation = carnivorousIS.Count;
        EntityManager.destroyEntityWhenDead(carnivorousIS);
        dataDisplayValueCarnivorousPopulation.text = currentNumberOfCarnivorousPopulation.ToString();

        currentNumberOfHerbivorousPopulation = herbivorousIS.Count;
        EntityManager.destroyEntityWhenDead(herbivorousIS);
        dataDisplayValueHerbivorousPopulation.text = currentNumberOfHerbivorousPopulation.ToString();

        currentNumberOfPlantPopulation = plantsIS.Count;
        EntityManager.destroyEntityWhenDead(plantsIS);
        dataDisplayValuePlantsPopulation.text = currentNumberOfPlantPopulation.ToString();
    }

    // LOGOS : List of game objects
    private List<GameObject> generateLOGOS(int populationSize, GameObject parent, GameObject togo)
    {
        List<GameObject> gos = new List<GameObject>();
        for (int i = 0; i < populationSize; i++)
        {
            gos.Add(Instantiate(togo));
            gos[i] = Instantiate(togo);
            gos[i].SetActive(true);
        }
        gos = setParentOfGameObjects(gos, parent);
        return gos;
    }
    private List<GameObject> setParentOfGameObjects(List<GameObject> gos, GameObject parent)
    {
        foreach (GameObject go in gos) { go.transform.SetParent(parent.transform); }
        return gos;
    }

    /*_____________________|*   USER INTERFACE INPUTS    *|_____________________*/
    private void setPopulationSizeOfCarnivorous(int value)
    {
        carnPS = value;
        textValueCarnivorousPopulation.text = value.ToString();
    }
    private void setPopulationSizeOfHerbivorous(int value)
    {
        herbPS = value;
        textValueHerbivorousPopulation.text = value.ToString();
    }
    private void setPopulationSizeOfPlants(int value)
    {
        plantsPS = value;
        textValuePlantsPopulation.text = value.ToString();
    }
    private void setPopulationSizeOfTerrainComponents(int value)
    {
        terrainPS = value;
        textValueTerrainComponents.text = value.ToString();
    }
    private void setNumberOfDaysOfSimulation(int value)
    {
        numberOfDays = value;
        textNumberOfDaysInput.text = value.ToString();
    }
    private void setTimeSpeedOfSimulation(float value)
    {
        timeSpeedSimulation = value;
        textTimeSpeedSimulation.text = value.ToString();
    }

    private void handleInputHerbivorousPopulation(int value)
    {
        for (int i = 0; i < herbivorousCollections.Count; i++)
        {
            if (value == i)
            {
                currentHerbivorousGo = herbivorousCollections[i];
            }
        }
    }
    private void handleInputCarnivorousPopulation(int value)
    {
        for (int i = 0; i < carnivorousCollections.Count; i++)
        {
            if (value == i)
            {
                currentCarnivorousGo = carnivorousCollections[i];
            }
        }
    }
    private void handleInputPlantsPopulation(int value)
    {
        for (int i = 0; i < plantsCollections.Count; i++)
        {
            if (value == i)
            {
                currentPlantGo = plantsCollections[i];
            }
        }
    }
    private void handleInputTerrainComponents(int value)
    {
        for (int i = 0; i < terrainComponentsCollection.Count; i++)
        {
            if (value == i)
            {
                currentTerrainGo = terrainComponentsCollection[i];
            }
        }
    }

    /*_____________________|*		   GENERATORS         *|_____________________*/
    public void generateCarnivorousPopulation()
    {
        List<GameObject> tmpList = generateLOGOS(carnPS, carnivorous, currentCarnivorousGo);
        for (int i = tmpList.Count - 1; i > 0; i--)
        {
            carnivorousIS.Add(tmpList[i]);
        }
        for (int i = carnivorousIS.Count - 1; i > 0; i--)
        {
            carnivorousIS[i].transform.SetParent(carnivorous.transform);
            carnivorousIS[i].GetComponent<Animal>().moveSpeed = carnivorousSpeed;
        }
        if (carnivorousIS.Count != 0) SpawnEntities.generateSpawnFor3DMapEarthlingAnimal(mapGenerator.meshData, carnivorousIS, mapGenerator.meshHeightFactor);
        currentNumberOfCarnivorousPopulation = carnivorousIS.Count;
        dataCarnivorous.Add(currentNumberOfCarnivorousPopulation);
    }
    public void generateHerbivorousPopulation()
    {
        List<GameObject> tmpList = generateLOGOS(herbPS, herbivorous, currentHerbivorousGo);
        for (int i = tmpList.Count - 1; i > 0; i--)
        {
            herbivorousIS.Add(tmpList[i]);
        }
        for (int i = herbivorousIS.Count - 1; i > 0; i--)
        {
            herbivorousIS[i].transform.SetParent(herbivorous.transform);
            herbivorousIS[i].GetComponent<Animal>().moveSpeed = herbivorousSpeed;
        }
        if (herbivorousIS.Count != 0) SpawnEntities.generateSpawnFor3DMapEarthlingAnimal(mapGenerator.meshData, herbivorousIS, mapGenerator.meshHeightFactor);
        currentNumberOfHerbivorousPopulation = herbivorousIS.Count;
        dataHerbivorous.Add(currentNumberOfHerbivorousPopulation);
    }
    public void generatePlantsPopulation()
    {
        List<GameObject> tmpList = generateLOGOS(plantsPS, plants, currentPlantGo);
        for (int i = tmpList.Count - 1; i > 0; i--)
        {
            plantsIS.Add(tmpList[i]);
        }
        for (int i = plantsIS.Count - 1; i > 0; i--)
        {
            plantsIS[i].transform.SetParent(plants.transform);
        }
        if (plantsIS.Count != 0) SpawnEntities.generateSpawnFor3DMapVegetation(mapGenerator.meshData, plantsIS, mapGenerator.meshHeightFactor);
        dataPlants.Add(currentNumberOfPlantPopulation);
    }
    public void generateTerrainComponents()
    {
        List<GameObject> tmpList = generateLOGOS(terrainPS, components, currentTerrainGo);
        for (int i = tmpList.Count - 1; i > 0; i--)
        {
            terrainComponentsIS.Add(tmpList[i]);
        }
        for (int i = terrainComponentsIS.Count - 1; i > 0; i--)
        {
            terrainComponentsIS[i].transform.SetParent(components.transform);
        }
        if (terrainComponentsIS.Count != 0) SpawnEntities.generateSpawnFor3DMapVegetation(mapGenerator.meshData, terrainComponentsIS, mapGenerator.meshHeightFactor);
    }

    /*_____________________|*		   DESTROYERS         *|_____________________*/
    public void destroyHerbivorousPopulation()
    {
        currentNumberOfHerbivorousPopulation = herbivorousIS.Count;
        for (int i = 0; i < currentNumberOfHerbivorousPopulation; i++)
        {
            GameObject.Destroy(herbivorousIS[i]);
        }
        dataDisplayValueHerbivorousPopulation.text = currentNumberOfHerbivorousPopulation.ToString();
        herbivorousIS.Clear();
        currentNumberOfHerbivorousPopulation = 0;
    }
    public void destroyCarnivorousPopulation()
    {
        currentNumberOfCarnivorousPopulation = carnivorousIS.Count;
        for (int i = 0; i < currentNumberOfCarnivorousPopulation; i++)
        {
            GameObject.Destroy(carnivorousIS[i]);
        }
        dataDisplayValueCarnivorousPopulation.text = currentNumberOfCarnivorousPopulation.ToString();
        carnivorousIS.Clear();
        currentNumberOfCarnivorousPopulation = 0;
    }
    public void destroyPlantsPopulation()
    {
        currentNumberOfPlantPopulation = plantsIS.Count;
        for (int i = 0; i < currentNumberOfPlantPopulation; i++)
        {
            GameObject.Destroy(plantsIS[i]);
        }
        dataDisplayValuePlantsPopulation.text = currentNumberOfPlantPopulation.ToString();
        plantsIS.Clear();
        currentNumberOfPlantPopulation = 0;
    }
    public void destroyTerrainComponents()
    {
        for (int i = 0; i < terrainComponentsIS.Count; i++)
        {
            GameObject.Destroy(terrainComponentsIS[i]);
        }
        terrainComponentsIS.Clear();
    }
}

