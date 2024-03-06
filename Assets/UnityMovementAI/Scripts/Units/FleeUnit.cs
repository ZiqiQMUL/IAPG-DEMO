using UnityEngine;

namespace UnityMovementAI
{
    public class FleeUnit : MonoBehaviour
    {
        public Transform target;

        SteeringBasics steeringBasics;
        Flee flee;

        void Start()
        {
            GameObject targetObject = GameObject.FindGameObjectWithTag("Chief");
            if (targetObject != null)
            {
                target = targetObject.transform;
            }
            else
            {
                Debug.LogError("No GameObject with the tag was found in the scene.");
            }

            steeringBasics = GetComponent<SteeringBasics>();
            flee = GetComponent<Flee>();
        }

        void FixedUpdate()
        {
            Vector3 accel = flee.GetSteering(target.position);

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}