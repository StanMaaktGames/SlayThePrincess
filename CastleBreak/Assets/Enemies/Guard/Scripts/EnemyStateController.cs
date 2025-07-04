using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    EnemyMovement movement;
    RaycastHit hit;
    Animator animator;

    public GameObject player;

    Vector3 lastSeenPlayerPosition;
    float distance;
    int state = 0; // 0 idle, 1 searching, 2 chasing
    public float suspicion = 0;
    float patience = 0;
    bool seesPlayer = false;

    public float requiredSuspicion = 1;
    public float searchPatience = 10; // time spent searching
    public float chasingPatience = 10; // time spent chasing
    public float sightDistance = 25f;

    void Start()
    {
        movement = GetComponent<EnemyMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= sightDistance && Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, sightDistance))
        {
            seesPlayer = true;
            lastSeenPlayerPosition = player.transform.position;
        }
        else
        {
            seesPlayer = false;
        }

        if (state == 2)
        {
            // chase
            GetComponent<EnemyMovement>().Chase(lastSeenPlayerPosition);
        }
        // else if (state == 1)
        // {
        //     // search
        //     GetComponent<EnemyMovement>().Searching(lastSeenPlayerPosition);
        // }
        else if (state == 0)
        {
            Idle();
        }
    }

    void Idle()
    {
        GetComponent<EnemyMovement>().Idle();

        if (seesPlayer)
        {
            suspicion += Time.deltaTime;
            lastSeenPlayerPosition = player.transform.position;
        }
        else
        {
            suspicion -= Time.deltaTime;
        }

        if (suspicion >= requiredSuspicion)
        {
            state = 2;
        }
    }
}
