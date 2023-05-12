using UnityEngine;
using UnityEngine.Networking;

namespace MovementAlgorithms
{

    public class SteeringDataOutput
    {
        public Vector3 LinearAccel;
        public float AngularAccel;
    }

    public class kinematicData
    {
        public Vector3 Position;
        public float Orientation;
        public Vector3 Velocity;
        public float Rotation;
    }
    
    
    public static class Seek
    {
        public static SteeringDataOutput GetSteering(kinematicData seeker, kinematicData target, float maxAccel)
        {
            var result = new SteeringDataOutput();
            
            var dir = SteeringBehavior.GetNormalizedDirection(seeker.Position, target.Position);
            
            result.LinearAccel = dir * maxAccel;

            result.AngularAccel = 0;

            return result;
        }
    }

    public static class Flee
    {
        public static SteeringDataOutput GetSteering(kinematicData fleer, kinematicData target, float maxAccel)
        {
            var result = Seek.GetSteering(target, fleer, maxAccel);
            return result;
        }
    }

    public class ArriveSettings
    {
        public float MaxSpeed;
        public float MaxAccel;
        public float TargetRadius;
        public float SlowRadius;
        public float TimeToTarget;
    }
    
    public static class Arrive
    {
        public static SteeringDataOutput GetSteering(kinematicData arriver, kinematicData target, ArriveSettings settings)
        {
            var dir = SteeringBehavior.GetNormalizedDirection(arriver.Position, target.Position);
            var distance = Vector3.Distance(target.Position, arriver.Position);

            if (distance < settings.TargetRadius) return new SteeringDataOutput{LinearAccel = Vector3.zero, AngularAccel = 0};
            var targetSpeed = 0.0f;
            if (distance > settings.SlowRadius) targetSpeed = settings.MaxSpeed;
            else targetSpeed = settings.MaxSpeed * distance / settings.SlowRadius;

            target.Velocity = dir.normalized * targetSpeed;
            var result = VelocityMatch.GetSteering(arriver, target, settings.MaxAccel, settings.TimeToTarget);
            
            return result;
        }
    }

    public class LeaveSettings
    {
        public float MaxAccel;
        public float LeaveRadius;
        public float SafeRadius;
    }
    
    public static class Leave
    {
        public static SteeringDataOutput GetSteering(kinematicData leaver, kinematicData target, LeaveSettings settings)
        {
            var dir = SteeringBehavior.GetNormalizedDirection(target.Position,leaver.Position);
            var distance = Vector3.Distance(target.Position, leaver.Position);

            if (distance < settings.LeaveRadius) return Flee.GetSteering(leaver, target, settings.MaxAccel);
            var targetAccel = settings.MaxAccel/distance;
            if (distance > settings.LeaveRadius) targetAccel = settings.MaxAccel/distance;
            if (distance > settings.SafeRadius) targetAccel = 0.0f;

            var result = new SteeringDataOutput {LinearAccel = dir * targetAccel, AngularAccel = 0f};
            return result;
        }
    }


    public static class VelocityMatch
    {
        public static SteeringDataOutput GetSteering(kinematicData matcher, kinematicData target, float maxAccel, float timeToTarget)
        {

            var result = new SteeringDataOutput {LinearAccel = target.Velocity - matcher.Velocity};

            result.LinearAccel /= timeToTarget;

            result.LinearAccel = SteeringBehavior.LimitLinearAcceleration(result.LinearAccel, maxAccel);

            result.AngularAccel = 0;

            return result;
            
            return result;
        }
    }
    
    public class AlignSettings
    {
        public float MaxRotation;
        public float MaxAngularAccel;
        public float TargetRadius;
        public float SlowRadius;
        public float TimeToTarget;
    }
    
    public static class Align
    {
        public static SteeringDataOutput GetSteering(kinematicData Aligner, kinematicData target, AlignSettings settings)
        {
            var dir =SteeringBehavior.GetNormalizedDirection(Aligner.Position,target.Position); 
            var targetOrientation = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            var rotation = Mathf.DeltaAngle(Aligner.Orientation, targetOrientation);
            var rotationSize = Mathf.Abs(rotation);
            if (rotationSize < settings.TargetRadius) return new SteeringDataOutput();

            var targetRotation = 0.0f;
            if (rotationSize > settings.SlowRadius) targetRotation = settings.MaxRotation;
            else targetRotation = settings.MaxRotation * rotationSize / settings.SlowRadius;

            targetRotation *= rotation / rotationSize;

            var result = new SteeringDataOutput {AngularAccel = targetRotation - Aligner.Rotation};
            result.AngularAccel /= settings.TimeToTarget;

            var angAccel = Mathf.Abs(result.AngularAccel);
            if (angAccel > settings.MaxAngularAccel)
            {
                result.AngularAccel /= angAccel;
                result.AngularAccel *= settings.MaxAngularAccel;
            }
            
            result.LinearAccel = Vector3.zero;
            return result;

        }
    }

    public static class Pursue
    {
        public static SteeringDataOutput GetSteering(kinematicData pursuer, kinematicData target, float maxAccel, float maxPrediction =0.00001f)
        {
            var dir = target.Position - pursuer.Position;
            var distance = dir.magnitude;
            var speed = pursuer.Velocity.magnitude;
            var prediction = 0.0f;

            if (speed <= distance / maxPrediction)
                prediction = maxPrediction;
            else
            {
                prediction = distance / speed;
            }

            target.Position += target.Velocity * prediction;
            return Seek.GetSteering(pursuer, target, maxAccel);
        }
    }
    
    
    public static class SteeringBehavior 
    {
        
        // public static SteeringDataOutput Arrive(kinematicData arriver, kinematicData target, float maxSpeed, float maxAccel, float targetRadius,
        //     float slowRadius)
        // {
        //     var dir = GetNormalizedDirection(arriver.Position, target.Position);
        //     var distance = Vector3.Distance(target.Position, arriver.Position);
        //
        //     if (distance < targetRadius) return default;
        //     var targetSpeed = 0.0f;
        //     if (distance > slowRadius) targetSpeed = maxSpeed;
        //     else targetSpeed = maxSpeed * distance / slowRadius;
        //
        //     target.Velocity = dir.normalized * targetSpeed;
        //     var result = VelocityMatch(arriver, target, maxAccel);
        //     
        //     return result;
        //
        // }
        //
        // public static SteeringDataOutput VelocityMatch(kinematicData matcher, kinematicData target, float maxAccel)
        // {
        //     var timeToTarget = 0.1f;
        //
        //     var result = new SteeringDataOutput {LinearAccel = target.Velocity - matcher.Velocity};
        //
        //     result.LinearAccel /= timeToTarget;
        //
        //     result.LinearAccel = LimitLinearAcceleration(result.LinearAccel, maxAccel);
        //
        //     result.AngularAccel = 0;
        //
        //     return result;
        // }

        public static kinematicData GetKinematicData(Rigidbody rigidbody)
        {
            return new ()
            {
                Position = rigidbody.position,
                Orientation = rigidbody.rotation.eulerAngles.y,
                Rotation = rigidbody.angularVelocity.y,
                Velocity = rigidbody.velocity
            };
        }

        public static Vector3 GetNormalizedDirection(Vector3 pos1, Vector3 pos2)
        {
            return (pos2 - pos1).normalized;
        }

        public static Vector3 LimitLinearAcceleration(Vector3 currLinearAccel, float maxAccel)
        {
            if (!(currLinearAccel.sqrMagnitude > maxAccel)) return currLinearAccel;
            currLinearAccel.Normalize();
            currLinearAccel *= maxAccel;

            return currLinearAccel;
        }
    }
}
