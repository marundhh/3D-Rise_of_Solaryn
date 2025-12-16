using System.Collections;
using UnityEngine;

public class AI_Movement : MonoBehaviour
{
    Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 0.2f;
    public float minWalkDuration = 10f;
    public float maxWalkDuration = 15f;

    [Header("Wait and Sit Settings")]
    public float minWaitTime = 5f;
    public float maxWaitTime = 7f;
    public float minSitTime = 5f;
    public float maxSitTime = 7f;

    [Header("Player/Minion Detection")]
    public float detectRange = 5f;

    Vector3 stopPosition;
    Vector3 initialPosition;

    float walkTime;
    public float walkCounter;
    float waitTime;
    public float waitCounter;
    public float sitTime;
    public float sitCounter;

    int WalkDirection;
    public bool isWalking;

    Transform currentTarget;
    bool hasDetectedTarget = false;

    private bool isReturning = false;
    private Vector3 returnTarget;

    Vector3 lastPosition;
    float stuckTime = 0f;
    float maxStuckDuration = 1f;

    float turnAroundCooldown = 0.5f;
    float turnAroundTimer = 0f;

    float obstacleCheckInterval = 0.5f;
    float obstacleCheckTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        ChooseDirection();
        lastPosition = transform.position;
    }

    void Update()
    {
        turnAroundTimer -= Time.deltaTime;
        obstacleCheckTimer -= Time.deltaTime;

        currentTarget = FindNearestTarget(new string[] { "Player", "Minion" });

        if (currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget <= detectRange)
            {
                hasDetectedTarget = true;
                isWalking = false;
                isReturning = false;

                animator.SetBool("isRunning", false);
                animator.SetBool("isSitting", false);

                Vector3 dir = (currentTarget.position - transform.position).normalized;
                dir.y = 0f;

                if (dir != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
                }

                return;
            }
        }

        if (hasDetectedTarget)
        {
            hasDetectedTarget = false;
            ReturnTo(initialPosition);
            return;
        }

        if (isReturning)
        {
            Vector3 dir = returnTarget - transform.position;
            dir.y = 0f;

            if (dir.magnitude <= 0.2f)
            {
                isReturning = false;
                ChooseDirection();
                return;
            }

            Quaternion rot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 5f);
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            animator.SetBool("isRunning", true);
            animator.SetBool("isSitting", false);

            lastPosition = transform.position;
            stuckTime = 0f;

            return;
        }

        if (isWalking && obstacleCheckTimer <= 0f)
        {
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
            float rayDistance = 0.5f;

            if (Physics.Raycast(rayOrigin, transform.forward, rayDistance))
            {
                if (turnAroundTimer <= 0f)
                {
                    TurnAroundRandom();
                    turnAroundTimer = turnAroundCooldown;
                    obstacleCheckTimer = obstacleCheckInterval;
                    return;
                }
            }
            else
            {
                obstacleCheckTimer = obstacleCheckInterval;
            }
        }

        if (Vector3.Distance(transform.position, lastPosition) < 0.01f && isWalking)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime > maxStuckDuration)
            {
                if (turnAroundTimer <= 0f)
                {
                    TurnAroundRandom();
                    turnAroundTimer = turnAroundCooldown;
                    stuckTime = 0f;
                    return;
                }
            }
        }
        else
        {
            stuckTime = 0f;
        }

        lastPosition = transform.position;

        // Movement logic
        if (isWalking)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isSitting", false);

            walkCounter -= Time.deltaTime;

            switch (WalkDirection)
            {
                case 0: transform.localRotation = Quaternion.Euler(0f, 0f, 0f); break;
                case 1: transform.localRotation = Quaternion.Euler(0f, 90f, 0f); break;
                case 2: transform.localRotation = Quaternion.Euler(0f, -90f, 0f); break;
                case 3: transform.localRotation = Quaternion.Euler(0f, 180f, 0f); break;
            }

            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            if (walkCounter <= 0)
            {
                stopPosition = transform.position;
                isWalking = false;
                animator.SetBool("isRunning", false);
                waitCounter = waitTime;
                sitCounter = 0;
            }
        }
        else
        {
            if (waitCounter > 0)
            {
                waitCounter -= Time.deltaTime;
                animator.SetBool("isRunning", false);
                animator.SetBool("isSitting", false);

                if (waitCounter <= 0)
                    sitCounter = sitTime;
            }
            else if (sitCounter > 0)
            {
                sitCounter -= Time.deltaTime;
                animator.SetBool("isRunning", false);
                animator.SetBool("isSitting", true);

                if (sitCounter <= 0)
                {
                    animator.SetBool("isSitting", false);
                    ChooseDirection();
                }
            }
            else
            {
                animator.SetBool("isSitting", false);
                ChooseDirection();
            }
        }
    }

    public void ChooseDirection()
    {
        walkTime = Random.Range(minWalkDuration, maxWalkDuration);
        waitTime = Random.Range(minWaitTime, maxWaitTime);
        sitTime = Random.Range(minSitTime, maxSitTime);

        WalkDirection = Random.Range(0, 4);
        isWalking = true;

        walkCounter = walkTime;
        waitCounter = 0;
        sitCounter = 0;
    }

    public void TurnAround()
    {
        WalkDirection = (WalkDirection + 2) % 4;
        walkCounter = Random.Range(minWalkDuration, maxWalkDuration);
        isWalking = true;
        isReturning = false;
        animator.SetBool("isRunning", true);
        animator.SetBool("isSitting", false);
    }

    public void TurnAroundRandom()
    {
        int turn = Random.value > 0.5f ? 1 : 3;
        WalkDirection = (WalkDirection + turn) % 4;
        walkCounter = Random.Range(minWalkDuration, maxWalkDuration);
        isWalking = true;
        isReturning = false;
        animator.SetBool("isRunning", true);
        animator.SetBool("isSitting", false);
    }

    public void ReturnTo(Vector3 target)
    {
        isReturning = true;
        returnTarget = target;
        isWalking = false;
        animator.SetBool("isRunning", true);
        animator.SetBool("isSitting", false);
    }

    private Transform FindNearestTarget(string[] tags)
    {
        float closestDistance = Mathf.Infinity;
        Transform nearest = null;

        foreach (string tag in tags)
        {
            GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject target in targets)
            {
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    nearest = target.transform;
                }
            }
        }

        return nearest;
    }
}
