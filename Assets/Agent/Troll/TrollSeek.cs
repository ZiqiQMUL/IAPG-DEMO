using UnityEngine;

namespace UnityMovementAI
{
    public class TrollSeek : MonoBehaviour
    {
        public Transform target;

        SteeringBasics steeringBasics;
        WallAvoidance wallAvoidance;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            wallAvoidance = GetComponent<WallAvoidance>();
        }

        void FixedUpdate()
        {
            target = GameObject.FindGameObjectWithTag("Thief").transform;
            Vector3 accel = steeringBasics.Seek(target.position) + wallAvoidance.GetSteering();

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}