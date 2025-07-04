using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class EnemyMovement : MonoBehaviour
{
    Animator animator;
    
    public UnityEngine.AI.NavMeshAgent agent;

    public float speed, sprintSpeed, requiredInterest;
    public Transform[] path;

    float interest, moveVelocityFloat, idleWalkDelay;
    int pathPointIndex = 0;
    Vector3 moveVelocity, lastPosition;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent.SetDestination(transform.position);
    }

    public void Idle()
    {
        agent.updateRotation = true;
        agent.isStopped = false;
        agent.speed = speed;
        if (agent.remainingDistance < 1.0f)
        {
            if (pathPointIndex > path.Length - 2)
            {
                pathPointIndex = 0;
            }
            else
            {
                pathPointIndex += 1;
            }
            agent.SetDestination(path[pathPointIndex].position);
        }

        animator.SetInteger("movingState", AnimateMovingState());
        lastPosition = transform.position;
    }

    // public void Searching(Vector3 lastSeenPlayerPosition)
    // {
    //     agent.isStopped = false;
    //     agent.speed = speed;
    //     agent.SetDestination(lastSeenPlayerPosition);
    // }

    public void Chase(Vector3 lastSeenPlayerPosition)
    {
        agent.isStopped = false;
        agent.speed = sprintSpeed;
        agent.SetDestination(lastSeenPlayerPosition);

        agent.updateRotation = false;
        transform.rotation = RotateTowardsPlayer(lastSeenPlayerPosition, 50f);
        animator.SetInteger("movingState", 2);
    }



    Quaternion RotateTowardsPlayer(Vector3 lastSeenPlayerPosition, float rotationSpeed)
    {
        Vector3 direction = lastSeenPlayerPosition - transform.position;
        direction.y = 0;
        if (direction == Vector3.zero)
            return transform.rotation;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        return Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    int AnimateMovingState() //determine whether the idle, walking or running animation should be playing
    {
        moveVelocity = ((transform.position - lastPosition)) / Time.deltaTime;
        moveVelocityFloat = new Vector2(Mathf.Abs(moveVelocity.x), Mathf.Abs(moveVelocity.z)).magnitude;
        if (moveVelocityFloat > 0)
        {
            return 1;
        }
        return 0;
    }
}
