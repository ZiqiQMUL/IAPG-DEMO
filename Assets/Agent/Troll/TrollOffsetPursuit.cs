using UnityEngine;

namespace UnityMovementAI
{
    public class TrollOffsetPursuit : MonoBehaviour
    {
        public GameObject target;

        public Vector3 offset;
        public float groupLookDist = 1.5f;

        SteeringBasics steeringBasics;
        OffsetPursuit offsetPursuit;
        Separation separation;
        WallAvoidance wallAvoidance;

        NearSensor sensor;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            offsetPursuit = GetComponent<OffsetPursuit>();
            separation = GetComponent<Separation>();
            wallAvoidance = GetComponent<WallAvoidance>();

            sensor = transform.Find("SeparationSensor").GetComponent<NearSensor>();
        }

        private GameObject FindClosestWithTag(params string[] tags)
        {
            GameObject closest = null;
            float minDistance = float.MaxValue;

            foreach (string tag in tags)
            {
                GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag(tag);
                foreach (GameObject obj in taggedObjects)
                {
                    float distance = Vector3.Distance(transform.position, obj.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closest = obj;
                    }
                }
            }

            return closest;
        }
        void LateUpdate()
        {
            target = FindClosestWithTag("Chief");
            MovementAIRigidbody targetRb = target.GetComponent<MovementAIRigidbody>();

            Vector3 targetPos;
            Vector3 offsetAccel = offsetPursuit.GetSteering(targetRb, offset, out targetPos);
            Vector3 sepAccel = separation.GetSteering(sensor.targets);
            Vector3 wallAvoidAccel = wallAvoidance.GetSteering();
            steeringBasics.Steer(offsetAccel + sepAccel + wallAvoidAccel);

            /* If we are still arriving then look where we are going, else look the same direction as our formation target */
            if (Vector3.Distance(transform.position, targetPos) > groupLookDist)
            {
                steeringBasics.LookWhereYoureGoing();
            }
            else
            {
                steeringBasics.LookAtDirection(targetRb.Rotation);
            }
        }
    }
}