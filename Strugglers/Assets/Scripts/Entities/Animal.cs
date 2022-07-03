/**
 * Auteurs 	: 	Simon Meier, Yasmine Margueron, Simon Porret
 * Date		: 	08.09.2021
 * Version	: 	4.1.2
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : Entity
{
    FieldOfView fow;
    Transform target;
    enum States { searchingWater, searchingFood, eating, drinking, walking };
    States state;

    [SerializeField] public RectTransform longevityBar, hungerBar, thirstBar;

    public float moveSpeed;
    public int rotationDelay, movementDelay;

    public int nb;
    public float thirst, hunger;

    float visionOffset;
    float percentLife, percentHunger, percentThirst;
    float maxLife, maxThirst, maxHunger;
    float metabolism = 1000f;

    Vector3 fixedVelocity;

    Rigidbody rigidbody;

    private void Start()
    {
        init();
        GetComponent<CapsuleCollider>().radius = 0.75f;
        GetComponent<CapsuleCollider>().height = 1.2f;
        GetComponent<CapsuleCollider>().center = new Vector3(0, 0.5f, 0);

        maxThirst = thirst = Random.Range(1500, 2000);
        maxHunger = hunger = Random.Range(1500, 2000);
        
        rotationDelay = 0;
        movementDelay = 0;

        maxLife = life;

        rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = true;

        fow = GetComponent<FieldOfView>();
        fow.viewAngle = 90;
        fow.viewRadius = 20;
        fow.meshResolution = 1;
        fow.edgeResolveIterations = 10;
        fow.edgeDstThreshold = 2;
        visionOffset = 0;

        state = States.walking;
        nb = 0;
    }

    private void Update()
    {
        if (WorldClock.timer != 0)
        {
            if (life > 0)
            {
                updateStates();
                scanSurroundings();
                rigidbody.velocity = fixedVelocity;
                updateLife();
                move();
                updateNeedsOfEntity();
                updateEntityInterface();
                updateHeight();
            }
        }
    }

    private void updateHeight()
    {
        if (transform.position.y <= 0.1)
        {
            transform.position = new Vector3(transform.position.x, 0.2f, transform.position.z);
        }
    }

    private void scanSurroundings()
    {
        if (visionOffset < 20)
        {
            visionOffset++;
            transform.Find("ViewVisualisation").GetComponent<Transform>().Rotate(new Vector3(1, 0, 0), 1);
        }
        if (visionOffset == 20) visionOffset = 40;
        if (visionOffset > 20)
        {
            visionOffset--;
            transform.Find("ViewVisualisation").GetComponent<Transform>().Rotate(new Vector3(1, 0, 0), -1);
        }
        if (visionOffset == 20) visionOffset = 0;
    }

    private void updateStates()
    {
        switch (state)
        {
            case States.walking:
            {
                if (percentThirst < percentHunger)
                {
                    state = States.searchingWater;
                }
                else if (percentHunger < percentThirst)
                {
                    state = States.searchingFood;
                }
                break;
            }
            case States.drinking:
            {
                if (percentThirst >= 80f)
                {
                    state = States.walking;
                }
                break;
            }
            case States.eating:
            {
                if (percentHunger >= 80f)
                {
                    state = States.walking;
                }
                break;
            }
        }
    }

    void updateEntityInterface()
    {
        percentLife = (life * 100) / maxLife;
        percentThirst = (thirst * 100) / maxThirst;
        percentHunger = (hunger * 100) / maxHunger;
        longevityBar.sizeDelta = new Vector2(percentLife, longevityBar.sizeDelta.y);
        thirstBar.sizeDelta = new Vector2(percentThirst, thirstBar.sizeDelta.y);
        hungerBar.sizeDelta = new Vector2(percentHunger, hungerBar.sizeDelta.y);
    }

    void updateNeedsOfEntity()
    {
        thirst--;
        hunger--;
        if (thirst == 0 || hunger == 0)
        {
            die();
        }
    }

    bool checkForTarget()
    {
        if (fow.getHasSomethingInView())
        {
            if (fow.visibleTargets.Count > 0)
            {
                if (target != null)
                {
                    target = fow.visibleTargets[0];
                    if (target.name.Contains("WaterCollider") && state == States.searchingWater)
                    {
                        return true;
                    }
                    if (!target.name.Contains("WaterCollider") && state == States.searchingFood)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void move()
    {
        if (!isDead)
        {
            if (state == States.searchingWater || state == States.searchingFood)
            {
                if (checkForTarget())
                {
                    float min = Vector3.Distance(fow.visibleTargets[0].position, transform.position);
                    foreach (Transform t in fow.visibleTargets)
                    {
                        float dist = Vector3.Distance(t.position, transform.position);
                        if (dist < min)
                        {
                            min = dist;
                            if (target != null)
                            {
                                target = t;
                                Debug.Log(t.name);
                            }
                        }
                    }
                }
                if (target != null)
                {
                    fixedVelocity = rigidbody.velocity = new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y, target.transform.position.z - transform.position.z).normalized * moveSpeed;
                    transform.LookAt(target);
                }
                else
                {
                    wander();
                }
            }
            else
            {
                wander();
            }
        }
    }

    void wander()
    {
        if (movementDelay == 0)
        {
            fixedVelocity = rigidbody.velocity *= 0;
            rotationDelay = Random.Range(-180, 180);
            movementDelay = Random.Range(250, 400);
        }
        if (rotationDelay > 0)
        {
            rotationDelay--;
            transform.Rotate(0, 1, 0);
            if (rotationDelay == 0)
            {
                fixedVelocity = rigidbody.velocity = new Vector3(Mathf.Sin(transform.localRotation.eulerAngles.y * Mathf.Deg2Rad), 0, Mathf.Cos(transform.localRotation.eulerAngles.y * Mathf.Deg2Rad)).normalized * moveSpeed;
            }
        }
        else if (rotationDelay < 0)
        {
            transform.Rotate(0, -1, 0);
            rotationDelay++;
            if (rotationDelay == 0)
            {
                fixedVelocity = rigidbody.velocity = new Vector3(Mathf.Sin(transform.localRotation.eulerAngles.y * Mathf.Deg2Rad), 0, Mathf.Cos(transform.localRotation.eulerAngles.y * Mathf.Deg2Rad)).normalized * moveSpeed;
            }
        }
        movementDelay--;
    }

    void eat()
    {
        state = States.eating;
        rigidbody.velocity *= 0;

        hunger += (maxHunger / 100) * metabolism;
        if (hunger > maxHunger) hunger = maxHunger;
        fow.removeTarget(target);
    }

    void drink()
    {
        state = States.drinking;
        rigidbody.velocity *= 0;
        thirst += (maxThirst / 100) * metabolism;
        if (thirst > maxThirst) thirst = maxThirst;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isDead)
        {
            string currentName = gameObject.transform.parent.gameObject.name;
            Debug.Log(gameObject.transform.parent.gameObject.name);

            if (collision.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                rigidbody.velocity *= 0;
                if (state == States.searchingWater)
                {
                    drink();
                }
                state = States.walking;
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Plant") && currentName == "Herbivorous")
            {
                if (state == States.searchingFood)
                {
                    eat();
                }
                state = States.walking;
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Herbivorous") && currentName == "Carnivorous")
            {
                if (state == States.searchingFood)
                {
                    eat();
                }
                state = States.walking;
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Carnivorous") && currentName == "Herbivorous")
            {
                gameObject.GetComponent<Animal>().rigidbody.velocity *= 0;
                gameObject.GetComponent<Animal>().life = 40;
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Herbivorous") && currentName == "Plant")
            {
                gameObject.GetComponent<Plant>().life = 40;
            }
        }
    }

}
