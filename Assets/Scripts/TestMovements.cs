using System;
using System.Collections;
using System.Collections.Generic;
using MovementAlgorithms;
using UnityEngine;

public class TestMovements : MonoBehaviour
{
    public Rigidbody ball;
    public Rigidbody cube;
    public Rigidbody bigBall;
    

    public float maxSpeed;
    public float maxAccel;
    public float targetRadius;
    public float slowRadius;
    
    public float speed = 5f; // a velocidade de movimento
    public float radius = 1000f; // o raio do círculo
    public float frequency = 1f; // a frequência do movimento
    public float forceMagnitude;
    public bool seek;

    private Vector3 center; // o centro do círculo
    private float angle = 0f;

    public Vector3 ballVel;
    
    // Start is called before the first frame update
    void Start()
    {
     ballVel = Vector3.left;
     //bigBall.velocity = ballVel;
    }

    
    private void FixedUpdate()
    {
        var cubeData = SteeringBehavior.GetKinematicData(cube);
        var ballData = SteeringBehavior.GetKinematicData(ball);
        moveCube();
        RotateCube();
        
        var arriveSettings = new LeaveSettings()
        {
            MaxAccel = maxAccel,
            LeaveRadius = 15,
            SafeRadius = 30,
            
        };
        
        var steeringOutput = Leave.GetSteering(ballData, cubeData, arriveSettings);
        ball.AddForce(steeringOutput.LinearAccel, ForceMode.Acceleration);
    }

    private void moveCube()
    {
        //angle += Time.fixedDeltaTime * speed;
        //float x = radius * Mathf.Cos(angle);
        //float y = radius * Mathf.Sin(angle);

        // cria um vetor na direção das coordenadas calculadas e aplica uma força
        //Vector3 forceDirection = new Vector3(x, 0f, y).normalized;
        //Vector3 force = forceDirection * forceMagnitude;
        bigBall.AddForce(bigBall.velocity.normalized*speed, ForceMode.Impulse);
        // angle += speed * Time.fixedDeltaTime;
        // Vector3 offset = new Vector3(Mathf.Sin(angle)*radius, 0f, Mathf.Cos(angle)*radius);
        // Vector3 newPosition = transform.position + offset;
        

        // calcula a velocidade necessária para alcançar a nova posição em um frame
        //Vector3 velocity = (newPosition - cube.position) / Time.fixedDeltaTime;
        
        var cubeData = SteeringBehavior.GetKinematicData(cube);
        var targetKinematicData = SteeringBehavior.GetKinematicData(bigBall);
       

        //bigBall.position = newPosition;
       // var steeringOutput = Pursue.GetSteering(cubeData, targetKinematicData, maxAccel, 1000f);
       SteeringDataOutput steeringOutput = default;
       if (seek)
       {
           steeringOutput =Seek.GetSteering(cubeData, targetKinematicData, maxAccel);
       }
       else
       {
           steeringOutput = Pursue.GetSteering(cubeData, targetKinematicData, maxAccel, 5f);
       }

        // aplica a velocidade no Rigidbody
        cube.AddForce(steeringOutput.LinearAccel, ForceMode.Acceleration);
    }

    private void RotateCube()
    {
        var alignSettings = new AlignSettings
        {
            MaxAngularAccel = 100f,
            MaxRotation = 180f,
            SlowRadius = 15f,
            TargetRadius = 1f,
            TimeToTarget = 1f
        };
        
        var cubeData = SteeringBehavior.GetKinematicData(cube);
        var bigBallData = SteeringBehavior.GetKinematicData(bigBall);

        var steeringOutput = Align.GetSteering(cubeData, bigBallData, alignSettings);
        //cube.angularVelocity = Vector3.up * (steeringOutput.AngularAccel * Time.fixedDeltaTime);
        cube.AddTorque(Vector3.up * (steeringOutput.AngularAccel ), ForceMode.Acceleration);

    }
}
