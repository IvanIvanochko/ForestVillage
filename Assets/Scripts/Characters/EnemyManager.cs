using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    List<EnemyController> enemies = new List<EnemyController>();

    private void Start()
    {
        foreach(Transform t in transform)
        {
            enemies.Add(t.GetComponent<EnemyController>());
        }
    }

    public bool IsEnemiesNull() => (enemies.Count == 0);

    public float GetClosestEnemy()
    {
        if(enemies.Count == 0) return 0;

        float closest = enemies[0].GetPlayerDistance();

        foreach (EnemyController enemy in enemies)
        {
            if(closest > enemy.GetPlayerDistance())
            {
                closest = enemy.GetPlayerDistance();
            }
        }

        return closest;
    }

    public EnemyController GetClosestEnemyController()
    {
        if (enemies.Count == 0) return null;

        float closest = enemies[0].GetPlayerDistance();
        EnemyController enemyController = enemies[0];

        foreach (EnemyController enemy in enemies)
        {
            if (closest > enemy.GetPlayerDistance())
            {
                closest = enemy.GetPlayerDistance();
                enemyController = enemy;
            }
        }

        return enemyController;
    }
}
