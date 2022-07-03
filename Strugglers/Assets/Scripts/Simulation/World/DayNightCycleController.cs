/**
 * Auteurs 	: 	Simon Meier, Yasmine Margueron, Simon Porret
 * Date		: 	27.08.2021
 * Version	: 	2.1.2
 */

using UnityEngine;

public class DayNightCycleController : MonoBehaviour
{
    private WorldClock worldTime;
    private Light sun;

    public void Awake()
    {
        worldTime = GameObject.Find("/WorldClock").GetComponent<WorldClock>();
        sun   = GameObject.Find("/Sun").GetComponent<Light>();
    }

    public void Update()
    {
        if (WorldClock.active && !WorldClock.paused)
        {
            UpdateSunIntensity();

            transform.RotateAround(Vector3.zero, Vector3.right, 360f / worldTime.secondsPerDay * Time.deltaTime);
            transform.LookAt(Vector3.zero);
        }
        if (!WorldClock.active)
        {
            transform.position = new Vector3(0, 0, -500);
            transform.rotation = new Quaternion(0, 0, 0, 0);
            transform.LookAt(Vector3.zero);
        }
    }

    private void UpdateSunIntensity()
    {
        if (worldTime.IsNight)
        {
            if (sun.intensity > 0.2f)
            {
                sun.intensity -= 0.05f;
            }
        }
        else
        {
            if (sun.intensity < 0.8f)
            {
                sun.intensity += 0.05f;
            }
        }
    }
}
