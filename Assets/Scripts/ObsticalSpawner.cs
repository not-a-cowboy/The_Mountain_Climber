using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Obstacle Prefabs")]
    public GameObject jumpObsPrefab;
    public GameObject duckObsPrefab;  
    public GameObject dodgeObsPrefab;  

    [Header("Spawn Settings")]
    public float[] laneXPositions = { -3f, 0f, 3f };
    public float[] zOffsets = { -0.2f, 0.2f };

    public void SpawnObstacles()
    {
        List<int> laneIndices = new List<int> { 0, 1, 2 };

        ObstacleType?[] plan = new ObstacleType?[3] { null, null, null };

        int lanesWithObstacles = Random.Range(0, 4); // 0, 1, 2, or 3

        ShuffleList(laneIndices);

        int dodgeCount = 0;

        for (int i = 0; i < lanesWithObstacles; i++)
        {
            int lane = laneIndices[i];
            ObstacleType picked = PickObstacleType(dodgeCount, lanesWithObstacles - i);
            plan[lane] = picked;
            if (picked == ObstacleType.Dodge) dodgeCount++;
        }

        if (AllDodge(plan))
        {
            plan[laneIndices[Random.Range(0, 3)]] = Random.value > 0.5f
                ? ObstacleType.Jump
                : ObstacleType.Duck;
        }

        for (int lane = 0; lane < 3; lane++)
        {
            if (plan[lane] == null) continue;

            float x = laneXPositions[lane];
            float z = zOffsets[Random.Range(0, zOffsets.Length)];
            Vector3 localPos = new Vector3(x, 0f, z);
            Vector3 worldPos = transform.TransformPoint(localPos);

            GameObject prefab = GetPrefab(plan[lane].Value);
            if (prefab != null)
                Instantiate(prefab, worldPos, Quaternion.identity, transform);
        }
    }

    ObstacleType PickObstacleType(int dodgeAlreadyPlaced, int remainingSlots)
    {
        bool canDodge = dodgeAlreadyPlaced < 2 || remainingSlots > 1;

        int range = canDodge ? 3 : 2;
        int roll = Random.Range(0, range);

        return roll switch
        {
            0 => ObstacleType.Jump,
            1 => ObstacleType.Duck,
            _ => ObstacleType.Dodge,
        };
    }

    bool AllDodge(ObstacleType?[] plan)
    {
        foreach (var slot in plan)
            if (slot != ObstacleType.Dodge) return false;
        return true;
    }

    GameObject GetPrefab(ObstacleType type) => type switch
    {
        ObstacleType.Jump => jumpObsPrefab,
        ObstacleType.Duck => duckObsPrefab,
        ObstacleType.Dodge => dodgeObsPrefab,
        _ => null
    };

    void ShuffleList(List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    enum ObstacleType { Jump, Duck, Dodge }
}