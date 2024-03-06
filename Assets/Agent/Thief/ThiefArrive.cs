using UnityEngine;

namespace UnityMovementAI
{
    public class ThiefArrive : MonoBehaviour
    {

        public Vector3 targetPosition;

        SteeringBasics steeringBasics;
        WallAvoidance wallAvoidance;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            wallAvoidance = GetComponent<WallAvoidance>();
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

        void FixedUpdate()
        {
            targetPosition = FindClosestWithTag("Gem").transform.position;
            Vector3 accel = steeringBasics.Arrive(targetPosition) + wallAvoidance.GetSteering();

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}