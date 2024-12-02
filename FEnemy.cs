using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FEnemy : MonoBehaviour
{
    public float Speed;
    public bool chase = false;
    public Transform startingPoint;
    public GameObject player;

    // 퍼지 로직 관련 변수
    public FuzzyInferenceSystem evasionFuzzySystem;
    public FuzzyInferenceSystem speedFuzzySystem;
    public FuzzyInferenceSystem attackFuzzySystem;

    // 퍼지 집합
    private FuzzySet close;
    private FuzzySet medium;
    private FuzzySet far;
    private FuzzySet lowHealth;
    private FuzzySet mediumHealth;
    private FuzzySet highHealth;
    private FuzzySet lowEvasion;
    private FuzzySet mediumEvasion;
    private FuzzySet highEvasion;
    private FuzzySet slowSpeed;
    private FuzzySet mediumSpeed;
    private FuzzySet fastSpeed;
    private FuzzySet slowAttack;
    private FuzzySet mediumAttack;
    private FuzzySet fastAttack;

    public Coroutine attackCoroutine;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        InitializeFuzzySystems();
    }

    void InitializeFuzzySystems()
    {
        // 퍼지 시스템 초기화
        evasionFuzzySystem = new FuzzyInferenceSystem();
        speedFuzzySystem = new FuzzyInferenceSystem();
        attackFuzzySystem = new FuzzyInferenceSystem();

        // 퍼지 집합 정의
        close = new FuzzySet(0, 0, 5);
        medium = new FuzzySet(3, 7, 10);
        far = new FuzzySet(8, 15, 20);
        lowHealth = new FuzzySet(0, 0, 30);
        mediumHealth = new FuzzySet(20, 50, 80);
        highHealth = new FuzzySet(60, 100, 100);
        lowEvasion = new FuzzySet(0, 0, 0.3f);
        mediumEvasion = new FuzzySet(0.2f, 0.5f, 0.8f);
        highEvasion = new FuzzySet(0.7f, 1, 1);
        slowSpeed = new FuzzySet(0, 0, 2);
        mediumSpeed = new FuzzySet(1, 3, 5);
        fastSpeed = new FuzzySet(4, 6, 10);
        slowAttack = new FuzzySet(1.5f, 2f, 2.5f);
        mediumAttack = new FuzzySet(1f, 1.5f, 2f);
        fastAttack = new FuzzySet(0.5f, 1f, 1.5f);

        // 퍼지 규칙 추가
        evasionFuzzySystem.AddRule(close, lowHealth, highEvasion);
        evasionFuzzySystem.AddRule(close, mediumHealth, mediumEvasion);
        evasionFuzzySystem.AddRule(close, highHealth, lowEvasion);
        evasionFuzzySystem.AddRule(medium, lowHealth, mediumEvasion);
        evasionFuzzySystem.AddRule(medium, mediumHealth, mediumEvasion);
        evasionFuzzySystem.AddRule(medium, highHealth, lowEvasion);
        evasionFuzzySystem.AddRule(far, lowHealth, lowEvasion);
        evasionFuzzySystem.AddRule(far, mediumHealth, lowEvasion);
        evasionFuzzySystem.AddRule(far, highHealth, lowEvasion);

        speedFuzzySystem.AddRule(close, lowHealth, fastSpeed);
        speedFuzzySystem.AddRule(close, mediumHealth, mediumSpeed);
        speedFuzzySystem.AddRule(close, highHealth, slowSpeed);
        speedFuzzySystem.AddRule(medium, lowHealth, fastSpeed);
        speedFuzzySystem.AddRule(medium, mediumHealth, mediumSpeed);
        speedFuzzySystem.AddRule(medium, highHealth, slowSpeed);
        speedFuzzySystem.AddRule(far, lowHealth, mediumSpeed);
        speedFuzzySystem.AddRule(far, mediumHealth, slowSpeed);
        speedFuzzySystem.AddRule(far, highHealth, slowSpeed);

        attackFuzzySystem.AddRule(close, lowHealth, fastAttack);
        attackFuzzySystem.AddRule(close, mediumHealth, mediumAttack);
        attackFuzzySystem.AddRule(close, highHealth, slowAttack);
        attackFuzzySystem.AddRule(medium, lowHealth, mediumAttack);
        attackFuzzySystem.AddRule(medium, mediumHealth, mediumAttack);
        attackFuzzySystem.AddRule(medium, highHealth, slowAttack);
        attackFuzzySystem.AddRule(far, lowHealth, slowAttack);
        attackFuzzySystem.AddRule(far, mediumHealth, slowAttack);
        attackFuzzySystem.AddRule(far, highHealth, slowAttack);
    }

    void Update()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        float health = player.GetComponent<Player>().nowHp;
        float evasionProbability = evasionFuzzySystem.Defuzzify(distanceToPlayer, health);
        float moveSpeed = speedFuzzySystem.Defuzzify(distanceToPlayer, health);
        float attackRate = attackFuzzySystem.Defuzzify(distanceToPlayer, health);


        if (Random.value < evasionProbability)
        {
            PerformEvasion();
        }

        Speed = moveSpeed;

        if (chase)
        {
            Chase();
        }
        else
        {
            ReturnStartPoint();
        }

        Flip();

        if (attackCoroutine == null)
        {
            attackCoroutine = StartCoroutine(AttackPlayer(attackRate));
        }

    }

    private void Chase()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Speed * Time.deltaTime);
    }

    public void Flip()
    {
        if (transform.position.x > player.transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    private void ReturnStartPoint()
    {
        transform.position = Vector2.MoveTowards(transform.position, startingPoint.position, Speed * Time.deltaTime);
    }

    public void PerformEvasion()
    {
        // 회피 로직 구현 (예: 특정 방향으로 빠르게 이동)
    }

    public IEnumerator AttackPlayer(float attackRate)
    {
        while (chase)
        {
            yield return new WaitForSeconds(attackRate);
            if (Vector2.Distance(transform.position, player.transform.position) <= 0.5f)
            {
                player.GetComponent<Player>().OnDamaged(transform.position);
            }
        }
        attackCoroutine = null;
    }
}