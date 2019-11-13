using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField] private GameObject shipPrefab;
    [SerializeField] private GameObject target;
    [SerializeField] private Text genCounter;
    private List<NeuralNetwork> brains;
    private List<Ship> shipList = null;

    private int[] layers = new int[] { 2, 10, 10, 2 };
    [SerializeField] private int populationSize = 20;
    [SerializeField] private float genLifespan = 15;
    private int currentGen = 0;
    private float genTimer;

    private bool training;

    void Awake()
    {
        genTimer = genLifespan;
    }

    void Update()
    {
        if (!training)
        {
            if (currentGen == 0)
            {
                InitializeShipBrains();
            }
            else
            {
                brains.Sort();

                for (int i = 0; i < populationSize / 2; i++)
                {
                    brains[i] = new NeuralNetwork(brains[i + (populationSize / 2)]);
                    brains[i].Mutate();
                    brains[i + (populationSize / 2)] = new NeuralNetwork(brains[i + (populationSize / 2)]);
                }

                for (int i = 0; i < populationSize; i++)
                {
                    brains[i].SetFitness(0f);
                }
            }

            currentGen++;
            genCounter.text = "Current gen: " + currentGen;
            training = true;
            genTimer = Time.time + genLifespan;
            target.transform.position = new Vector3(UnityEngine.Random.Range(-47f, 47f), UnityEngine.Random.Range(-26f, 26f), -2);
            CreateShipBodies();
        }
        else
        {
            if (Time.time > genTimer)
            {
                training = false;
            }
        }     
    }

    private void CreateShipBodies()
    {
        if (shipList != null)
        {
            for (int i = 0; i < shipList.Count; i++)
            {
                GameObject.Destroy(shipList[i].gameObject);
            }
        }

        shipList = new List<Ship>();

        for (int i = 0; i < populationSize; i++)
        {
            Ship newShip = ((GameObject)Instantiate(shipPrefab, new Vector3(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-10f, 10f), -3), shipPrefab.transform.rotation)).GetComponent<Ship>();
            newShip.InitializeShip(brains[i], target.transform);
            shipList.Add(newShip);
        }

    }

    private void InitializeShipBrains()
    {
        if (populationSize % 2 != 0)
        {
            populationSize--;
        }

        brains = new List<NeuralNetwork>();

        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork brain = new NeuralNetwork(layers);
            brain.Mutate();
            brains.Add(brain);
        }
    }
}
