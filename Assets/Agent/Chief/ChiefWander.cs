using UnityEngine;

namespace UnityMovementAI
{
    public class ChiefWander : MonoBehaviour
    {
        SteeringBasics steeringBasics;
        Wander1 wander;
        WallAvoidance wallAvoidance;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            wander = GetComponent<Wander1>();
            wallAvoidance = GetComponent<WallAvoidance>();
        }

        void FixedUpdate()
        {
            Vector3 accel = wander.GetSteering() + wallAvoidance.GetSteering();

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}