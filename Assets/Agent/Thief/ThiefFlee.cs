using UnityEngine;

namespace UnityMovementAI
{
    public class ThiefFlee : MonoBehaviour
    {
        public Transform target;
        SteeringBasics steeringBasics;
        Flee flee;
        WallAvoidance wallAvoidance;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            flee = GetComponent<Flee>();
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
            target = FindClosestWithTag("Chief", "Troll").transform;
            Vector3 accel = flee.GetSteering(target.position) + wallAvoidance.GetSteering();

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}