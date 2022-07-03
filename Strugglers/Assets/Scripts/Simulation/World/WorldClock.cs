/**
 * Auteurs 	: 	Simon Meier, Yasmine Margueron, Simon Porret
 * Date		: 	08.09.2021
 * Version	: 	4.1.2
 */

using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Lean.Gui;

public class WorldClock : MonoBehaviour
{
    [SerializeField] public Graphes graphicDataAnalysisHerbivorous, graphicDataAnalysisCarnivorous, graphicDataAnalysisPlants;

    public static bool active;
    public static bool paused;
    public float secondsPerDay = 30f;
    public float dayLengthHours = 24f;

    public static float timer = 0f;

    public int numberOfDays = 0;
    public int numberOfWeeks = 0;
    public int numberOfHours = 0;

    public Text timerWeeks;
    public Text timerDays;
    public Text timerHours;

    public LeanButton btnStart;
    public LeanButton btnPause;
    public LeanButton btnResume;
    public LeanButton btnStop;

    float offset;

    public WorldClock(float secPd, float dayLgt)
    {
        secondsPerDay = secPd;
        dayLengthHours = dayLgt;
        timer = 0;
    }

    private void Start()
    {
        offset = secondsPerDay / dayLengthHours;
        active = false;
        paused = true;

        btnStart.OnClick.AddListener(delegate { startTimer(); });
        btnPause.OnClick.AddListener(delegate { pauseTimer(); });
        btnResume.OnClick.AddListener(delegate { resumeTimer(); });
        btnStop.OnClick.AddListener(delegate { stopTimer(); });

        timerWeeks.text = numberOfWeeks.ToString();
        timerDays.text = numberOfDays.ToString();
        timerHours.text = numberOfHours.ToString();

        graphicDataAnalysisCarnivorous.updateGraph(Simulation.dataCarnivorous);
        graphicDataAnalysisHerbivorous.updateGraph(Simulation.dataHerbivorous);
        graphicDataAnalysisPlants.updateGraph(Simulation.dataPlants);
    }

    public void Update()
    {
        if (active && !paused)
        {
            timer += Time.deltaTime;
            if ((offset - 0.15) <= timer && timer <= (offset + 0.15))
            {
                timer = 0;
                numberOfHours += 1;
                timerHours.text = (numberOfHours).ToString();
                if (numberOfHours % 6 == 0)
                {
                    Simulation.dataCarnivorous.Add(Simulation.currentNumberOfCarnivorousPopulation);
                    Simulation.dataHerbivorous.Add(Simulation.currentNumberOfHerbivorousPopulation);
                    Simulation.dataPlants.Add(Simulation.currentNumberOfPlantPopulation);

                    graphicDataAnalysisCarnivorous.updateGraph(Simulation.dataCarnivorous);
                    graphicDataAnalysisHerbivorous.updateGraph(Simulation.dataHerbivorous);
                    graphicDataAnalysisPlants.updateGraph(Simulation.dataPlants);
                }
            }
            if (numberOfHours == 24)
            {
                numberOfHours = 0;
                timer = 0f;
                numberOfDays += 1;
                timerDays.text = numberOfDays.ToString();
            }
            if (numberOfDays == 7)
            {
                numberOfDays = 0;
                numberOfWeeks += 1;
                timerWeeks.text = numberOfWeeks.ToString();
            }
        }
        if(!active)
        {
            if (graphicDataAnalysisCarnivorous.valueList.Count != 0)
            {
                for (int i = graphicDataAnalysisCarnivorous.valueList.Count - 1; i >= 0; i--)
                {
                    graphicDataAnalysisCarnivorous.valueList.Remove(graphicDataAnalysisCarnivorous.valueList[i]);
                }
                graphicDataAnalysisCarnivorous.valueList.Clear();
                graphicDataAnalysisCarnivorous.updateGraph(graphicDataAnalysisCarnivorous.valueList);
            }
            if (graphicDataAnalysisHerbivorous.valueList.Count != 0)
            {
                for (int i = graphicDataAnalysisHerbivorous.valueList.Count - 1; i >= 0; i--)
                {
                    graphicDataAnalysisHerbivorous.valueList.Remove(graphicDataAnalysisHerbivorous.valueList[i]);
                }
                graphicDataAnalysisHerbivorous.valueList.Clear();
                graphicDataAnalysisHerbivorous.updateGraph(graphicDataAnalysisHerbivorous.valueList);
            }
            if (graphicDataAnalysisPlants.valueList.Count != 0)
            {
                for (int i = graphicDataAnalysisPlants.valueList.Count - 1; i >= 0; i--)
                {
                    graphicDataAnalysisPlants.valueList.Remove(graphicDataAnalysisPlants.valueList[i]);
                }
                graphicDataAnalysisPlants.valueList.Clear();
                graphicDataAnalysisPlants.updateGraph(graphicDataAnalysisPlants.valueList);
            }
        }
    }

    private void resumeTimer()
    {
        Time.timeScale = 1f;
        btnPause.enabled = true;
        btnStart.enabled = false;
        active = true;
        paused = false;
    }

    private void startTimer()
    {
        active = true;
        paused = false;
        btnPause.enabled = true;
        btnResume.enabled = false;
        btnStop.enabled = true;
        btnStart.enabled = false;
    }

    private void pauseTimer()
    {
        btnStart.enabled = false;
        btnPause.enabled = false;
        btnResume.enabled = true;
        paused = true;
    }

    public void stopTimer()
    {
        active = false;
        paused = true;
        btnPause.enabled = false;
        btnResume.enabled = false;
        btnStop.enabled = false;
        btnStart.enabled = true;

        timer = 0f;

        numberOfDays = 0;
        numberOfWeeks = 0;
        numberOfHours = 0;

        timerWeeks.text = "0";
        timerDays.text = "0";
        timerHours.text = "0";
    }

    public float PercentComplete
    {
        get { return timer / secondsPerDay; }
    }

    public bool IsNight
    {
        get { return PercentComplete > 0.5f; }
    }

    public System.TimeSpan WorldTime
    {
        get
        {
            float progress = dayLengthHours * PercentComplete;
            int hour = (int)progress;
            if (hour > 23) hour = 0;

            float fMinute = (progress - hour) * 60f;
            int   minute  = (int)fMinute;
            if (minute > 59) minute = 0;

            int second = (int)((fMinute - minute) * 60f);
            if (second > 59) second = 0;

            return new System.TimeSpan(hour, minute, second);
        }
    }
}
