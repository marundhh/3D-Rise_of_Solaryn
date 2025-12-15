using System.Collections;
using UnityEngine;

public class AI_Movement : MonoBehaviour
{
    Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 0.2f;            // Tốc độ di chuyển
    public float minWalkDuration = 10f;       // Thời gian đi bộ tối thiểu
    public float maxWalkDuration = 15f;       // Thời gian đi bộ tối đa

    [Header("Wait and Sit Settings")]
    public float minWaitTime = 5f;
    public float maxWaitTime = 7f;
    public float minSitTime = 5f;
    public float maxSitTime = 7f;

    [Header("Player Detection")]
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

    Transform player;

    private bool isReturning = false;
    private Vector3 returnTarget;

    Vector3 lastPosition;
    float stuckTime = 0f;
    float maxStuckDuration = 1f;

    float turnAroundCooldown = 0.5f;
    float turnAroundTimer = 0f;

    float obstacleCheckInterval = 0.5f;
    float obstacleCheckTimer = 0f;

    bool hasDetectedPlayer = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        initialPosition = transform.position;

        ChooseDirection();

        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
            player = foundPlayer.transform;

        lastPosition = transform.position;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (turnAroundTimer > 0)
            turnAroundTimer -= Time.deltaTime;

        obstacleCheckTimer -= Time.deltaTime;

        if (distanceToPlayer <= detectRange)
        {
            hasDetectedPlayer = true;
            isWalking = false;
            isReturning = false;

            animator.SetBool("isRunning", false);
            animator.SetBool("isSitting", false);

            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0f;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }

            return;
        }
        else if (hasDetectedPlayer)
        {
            hasDetectedPlayer = false;
            ReturnTo(initialPosition);
            return;
        }

        if (isReturning)
        {
            Vector3 dir = (returnTarget - transform.position);
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

        // Check obstacle
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

        // Check stuck
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

        // AI movement logic
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
}