using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSpawner : MonoBehaviour
{
    [SerializeField]
    private CellularAutomataCaveGenerator caveGenerator;

    [SerializeField]
    private GameObject TrollPrefab, TrollChiefPrefab, ThiefPrefab;

    [SerializeField]
    private int nTroll, nTrollChief, nThief;

    void Start()
    {
        SpawnAgents(TrollPrefab, nTroll);
        SpawnAgents(TrollChiefPrefab, nTrollChief);
        SpawnAgents(ThiefPrefab, nThief);
    }

    void SpawnAgents(GameObject agentPrefab, int numberOfAgents)
    {
        System.Random random = new System.Random((caveGenerator.seed + agentPrefab.name).GetHashCode());

        for (int i = 0; i < numberOfAgents; i++)
        {
            int x, y;
            do
            {
                x = random.Next(1, caveGenerator.width - 1);
                y = random.Next(1, caveGenerator.height - 1);
            } while (caveGenerator.map[x, y] == 1); // Ensure the agent is not spawned inside a wall

            Instantiate(agentPrefab, new Vector3(x, y, 0), Quaternion.identity);
        }
    }
}