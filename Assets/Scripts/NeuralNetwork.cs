using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NeuralNetwork : IComparable<NeuralNetwork>
{
    private int[] neuronsPerLayer;
    private float[][] neurons;
    private float[][][] weights;
    private float[][][] previousWeights;

    private float fitness = 0;

    public NeuralNetwork(int[] _neuronAmount)
    {
        this.neuronsPerLayer = new int[_neuronAmount.Length];
        for (int i = 0; i < _neuronAmount.Length; i++)
        {
            this.neuronsPerLayer[i] = _neuronAmount[i];
        }

        InitializeNeurons();
        InitializeWeights();
    }

    public NeuralNetwork(NeuralNetwork _copyNetwork)
    {
        this.neuronsPerLayer = new int[_copyNetwork.neuronsPerLayer.Length];
        for (int i = 0; i < _copyNetwork.neuronsPerLayer.Length; i++)
        {
            this.neuronsPerLayer[i] = _copyNetwork.neuronsPerLayer[i];
        }

        InitializeNeurons();
        InitializeWeights();
        CopyWeights(_copyNetwork.weights);
    }

    private void InitializeNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();

        for (int i = 0; i < neuronsPerLayer.Length; i++)
        {
            neuronsList.Add(new float[neuronsPerLayer[i]]);
        }

        neurons = neuronsList.ToArray();
    }

    private void InitializeWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();

        for (int i = 1; i < neuronsPerLayer.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = neuronsPerLayer[i - 1];

            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                layerWeightsList.Add(neuronWeights);
            }

            weightsList.Add(layerWeightsList.ToArray());
        }

        weights = weightsList.ToArray();
    }

    private void CopyWeights()
    {
        List<float[][]> weightsCopy = new List<float[][]>();

        for (int i = 0; i < weights.Length; i++)
        {
            List<float[]> currentLayerWeights = new List<float[]>();

            for (int j = 0; j < weights[i].Length; j++)
            {
                float[] weightsFound = new float[weights[i].Length];

                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weightsFound[i] = weights[i][j][k];
                }

                currentLayerWeights.Add(weightsFound);
            }

            weightsCopy.Add(currentLayerWeights.ToArray());
        }

        previousWeights = weightsCopy.ToArray();
    }

    private void CopyWeights(float[][][] copyWeights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    public float[] FeedForward(float[] inputs)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        for (int i = 1; i < neuronsPerLayer.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float weightSum = 0f;

                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    weightSum += weights[i - 1][j][k] * neurons[i - 1][k];
                }

                neurons[i][j] = (float)Math.Tanh((double)weightSum);
            }
        }

        return neurons[neurons.Length - 1];
    }

    public void Mutate()
    {
        for (int i = 0; i < weights.Length; i++)
        {           
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float currentWeight = weights[i][j][k];
                    float rmd = UnityEngine.Random.Range(0f, 100f);

                    if (rmd <= 4)
                    {
                        currentWeight *= -1;
                    }
                    else if (rmd <= 8)
                    {
                        currentWeight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if (rmd <= 12)
                    {
                        float increase = UnityEngine.Random.Range(0f, 1f) + 1f;
                        currentWeight *= increase;
                    }
                    else if (rmd <= 16)
                    {
                        float decrease = UnityEngine.Random.Range(0f, 1f);
                        currentWeight *= decrease;
                    }

                    weights[i][j][k] = currentWeight;
                }
            }
        }
    }

    public void SetFitness(float amount)
    {
        fitness = amount;
    }

    public void AddFitness(float amount)
    {
        fitness += amount;
    }

    public float GetFitness()
    {
        return fitness;
    }

    public int CompareTo(NeuralNetwork other)
    {
        if (other == null)
        {
            return 1;
        }

        if (fitness > other.fitness)
        {
            return 1;
        }   
        else if (fitness < other.fitness)
        {
            return -1;
        }
        else
        {
            return 0;
        }   
    }
}
