using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FChaseControl : MonoBehaviour
{
    public FEnemy[] enemyArray;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (FEnemy enemy in enemyArray)
            {
                enemy.chase = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            foreach (FEnemy enemy in enemyArray)
            {
                enemy.chase = false;
            }
        }
    }

    void Update()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            float playerHealth = player.GetComponent<Player>().nowHp;
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            foreach (FEnemy enemy in enemyArray)
            {
                float evasionProbability = enemy.evasionFuzzySystem.Defuzzify(distanceToPlayer, playerHealth);
                float moveSpeed = enemy.speedFuzzySystem.Defuzzify(distanceToPlayer, playerHealth);
                float attackRate = enemy.attackFuzzySystem.Defuzzify(distanceToPlayer, playerHealth);

                if (Random.value < evasionProbability)
                {
                    enemy.PerformEvasion();
                }

                enemy.Speed = moveSpeed;

                if (enemy.chase)
                {
                    if (enemy.attackCoroutine == null)
                    {
                        enemy.attackCoroutine = enemy.StartCoroutine(enemy.AttackPlayer(attackRate));
                    }
                }
            }
        }
    }
}