using UnityEngine;
using NPBehave;

public class ThiefAI : MonoBehaviour
{
    private Blackboard blackboard;
    private Root behaviorTree;

    public MonoBehaviour wanderUnit;
    public MonoBehaviour fleeUnit;
    public MonoBehaviour arriveUnit;
    public float enemyDistanceThreshold = 5f;
    public float gemDistanceThreshold = 4f;

    void Start()
    {
        behaviorTree = CreateBehaviourTree();
        blackboard = behaviorTree.Blackboard;

#if UNITY_EDITOR
        Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = behaviorTree;
#endif

        behaviorTree.Start();
    }

    private Root CreateBehaviourTree()
    {
        return new Root(
            new Service(0.1f, UpdateDistances,
                new Selector(

                    new BlackboardCondition("distanceToEnemy", Operator.IS_SMALLER, enemyDistanceThreshold, Stops.IMMEDIATE_RESTART,

                        new Sequence(
                            new Action(() => EnableFlee()) { Label = "Enable Flee" }
                        )
                    ),

                    new BlackboardCondition("distanceToGem", Operator.IS_SMALLER, gemDistanceThreshold, Stops.IMMEDIATE_RESTART,

                        new Sequence(
                            new Action(() => EnableArrive()) { Label = "Enable Arrive" }
                        )
                    ),

                    new Sequence(
                        new Action(() => EnableWander()) { Label = "Enable Wander" }
                    )
                )
            )
        );
    }

    private void UpdateDistances()
    {
        GameObject enemy = FindClosestWithTag("Chief", "Troll");
        GameObject gem = FindClosestWithTag("Gem");

        if (enemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            blackboard["distanceToEnemy"] = distanceToEnemy;
        }
        else
        {
            blackboard["distanceToEnemy"] = float.MaxValue;
        }

        if (gem != null)
        {
            float distanceToGem = Vector3.Distance(transform.position, gem.transform.position);
            blackboard["distanceToGem"] = distanceToGem;
        }
        else
        {
            blackboard["distanceToGem"] = float.MaxValue;
        }
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

    private void EnableWander()
    {
        wanderUnit.enabled = true;
        fleeUnit.enabled = false;
        arriveUnit.enabled = false;
    }

    private void EnableFlee()
    {
        wanderUnit.enabled = false;
        fleeUnit.enabled = true;
        arriveUnit.enabled = false;
    }

    private void EnableArrive()
    {
        wanderUnit.enabled = false;
        fleeUnit.enabled = false;
        arriveUnit.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Gem")
        {
            Destroy(collision.gameObject);
        }
    }

}