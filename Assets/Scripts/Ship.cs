using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private Transform target;
    private NeuralNetwork brain;
    private Rigidbody2D self;

    private bool initialized = false;

    void Start()
    {
        self = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (initialized)
        {
            float distance = Vector2.Distance(transform.position, target.position);
            float[] inputs = new float[1];
            float angle = transform.eulerAngles.z % 360f;

            if (angle < 0f)
            {
                angle += 360f;
            }
            
            Vector2 deltaVector = (target.position - transform.position).normalized;
            float rad = Mathf.Atan2(deltaVector.y, deltaVector.x);
            rad *= Mathf.Rad2Deg;
            rad = rad % 360;

            if (rad < 0)
            {
                rad = 360 + rad;
            }

            rad = 90f - rad;

            if (rad < 0f)
            {
                rad += 360f;
            }

            rad = 360 - rad;
            rad -= angle;

            if (rad < 0)
            {
                rad = 360 + rad;
            }
                
            if (rad >= 180f)
            {
                rad = 360 - rad;
                rad *= -1f;
            }

            rad *= Mathf.Deg2Rad;
            inputs[0] = rad / (Random.Range(1, 100));
            float[] output = brain.FeedForward(inputs);
            self.velocity = 8f * transform.up;
            self.angularVelocity = 1000f * output[0];
            brain.AddFitness((1f - Mathf.Abs(inputs[0])));
        }
    }

    public void InitializeShip(NeuralNetwork _newBrain, Transform _newTarget)
    {
        target = _newTarget;
        brain = _newBrain;
        initialized = true;
    }
}
