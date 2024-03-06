using UnityEngine;
using NPBehave;

public class ChiefAI : MonoBehaviour
{
    private Blackboard blackboard;
    private Root behaviorTree;

    public MonoBehaviour wanderUnit;
    public MonoBehaviour seekUnit;
    public float distanceThreshold = 4f;

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
            new Service(0.1f, UpdateDistanceToThief,
                new Selector(

                    new BlackboardCondition("distanceToThief", Operator.IS_SMALLER, distanceThreshold, Stops.IMMEDIATE_RESTART,

                        new Sequence(
                            new Action(() => EnableSeek()) { Label = "Enable Seek" }
                        )
                    ),

                    new Sequence(
                        new Action(() => EnableWander()) { Label = "Enable Wander" }
                    )
                )
            )
        );
    }

    private void UpdateDistanceToThief()
    {
        GameObject thief = GameObject.FindGameObjectWithTag("Thief");
        float distance = Vector3.Distance(transform.position, thief.transform.position);
        blackboard["distanceToThief"] = distance;
    }

    private void EnableSeek()
    {
        wanderUnit.enabled = false;
        seekUnit.enabled = true;
    }

    private void EnableWander()
    {
        wanderUnit.enabled = true;
        seekUnit.enabled = false;
    }
}