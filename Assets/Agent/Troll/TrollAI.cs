using UnityEngine;
using NPBehave;

public class TrollAI : MonoBehaviour
{
    private Blackboard blackboard;
    private Root behaviorTree;

    public MonoBehaviour offsetPursueUnit;
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
                        new Action(() => EnableOffsetPursue()) { Label = "Enable OffsetPursue" }
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
        offsetPursueUnit.enabled = false;
        seekUnit.enabled = true;
    }

    private void EnableOffsetPursue()
    {
        offsetPursueUnit.enabled = true;
        seekUnit.enabled = false;
    }
}