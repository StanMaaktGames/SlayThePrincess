using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyStateController : MonoBehaviour
{
    EnemyMovement movement;
    RaycastHit hit;
    Animator animator;

    public GameObject player;

    Vector3 lastSeenPlayerPosition, directionToPlayer;
    float distance;
    int state = 0; // 0 idle, 1 searching, 2 chasing
    public float suspicion = 0;
    float patience = 0;
    bool seesPlayer = false;

    public float requiredSuspicion = 1;
    public float searchPatience = 10; // time spent searching
    public float chasingPatience = 10; // time spent chasing
    public float sightDistance = 25f;
    public string playerColliderName;
    public float viewAngle = 180;
    public string sceneToLoad = "EnemyTestScene";

    void Start()
    {
        movement = GetComponent<EnemyMovement>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 5f, Color.green);
        Debug.DrawLine(transform.position, player.transform.position, Color.red);


        distance = Vector3.Distance(transform.position, player.transform.position);

        directionToPlayer = (player.transform.position - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, directionToPlayer);

        Debug.DrawRay(transform.position, (player.transform.position - transform.position).normalized * sightDistance, Color.red);
        if (dot > Mathf.Cos((viewAngle / 2) * Mathf.Deg2Rad) && distance <= sightDistance && Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, sightDistance))
        {
            if (hit.collider.name == playerColliderName)
            {
                seesPlayer = true;
                lastSeenPlayerPosition = player.transform.position;
            }
        }
        else
        {
            seesPlayer = false;
        }

        if (state == 2)
        {
            if (seesPlayer)
            {
                lastSeenPlayerPosition = player.transform.position;
            }
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
            if (player.GetComponent<PlayerController>().crouching)
            {
                suspicion += Time.deltaTime / 3;
            }
            else
            {
                suspicion += Time.deltaTime;
            }
            lastSeenPlayerPosition = player.transform.position;
        }
        else if (suspicion > 0)
        {
            suspicion -= Time.deltaTime;
        }

        if (suspicion >= requiredSuspicion)
        {
            state = 2;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Hit the player");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
