using System.Collections;
using UnityEngine;

public class NPCWalk : MonoBehaviour
{
    [Header("Di chuyển")]
    [HideInInspector] public Animator animator;
    public float moveSpeed = 0.2f;

    private Vector3 stopPosition;
    private float walkTime;
    public float walkCounter;
    private float waitTime;
    public float waitCounter;

    private int WalkDirection;
    public bool isWalking;

    [Header("Tương tác")]
    public GameObject chatCanvas;                // Canvas chat nằm trong NPC
    public float interactionDuration = 5f;       // Thời gian dừng khi tương tác

    private Coroutine lookCoroutine;
    private float originalSpeed;
    private bool isInteracting = false;          // THÊM: cờ kiểm soát trạng thái tương tác

    void Start()
    {
        animator = GetComponent<Animator>();
        walkTime = Random.Range(3, 6);
        waitTime = Random.Range(5, 7);
        waitCounter = waitTime;
        walkCounter = walkTime;

        originalSpeed = moveSpeed;

        if (chatCanvas != null)
            chatCanvas.SetActive(false);

        ChooseDirection();
    }

    void Update()
    {
        if (isInteracting) return; // NGĂN cập nhật nếu đang tương tác

        if (isWalking)
        {
            animator.SetBool("isRunning", true);
            walkCounter -= Time.deltaTime;

            Vector3 moveDir = Vector3.zero;

            switch (WalkDirection)
            {
                case 0: transform.localRotation = Quaternion.Euler(0f, 0f, 0f); moveDir = transform.forward; break;
                case 1: transform.localRotation = Quaternion.Euler(0f, 90f, 0f); moveDir = transform.forward; break;
                case 2: transform.localRotation = Quaternion.Euler(0f, -90f, 0f); moveDir = transform.forward; break;
                case 3: transform.localRotation = Quaternion.Euler(0f, 180f, 0f); moveDir = transform.forward; break;
            }

            transform.position += moveDir * moveSpeed * Time.deltaTime;

            if (walkCounter <= 0)
            {
                stopPosition = transform.position;
                isWalking = false;
                animator.SetBool("isRunning", false);
                waitCounter = waitTime;
            }
        }
        else
        {
            waitCounter -= Time.deltaTime;
            if (waitCounter <= 0)
            {
                ChooseDirection();
            }
        }
    }

    public void ChooseDirection()
    {
        if (chatCanvas != null)
            chatCanvas.SetActive(false); // Ẩn canvas khi bắt đầu đi lại

        WalkDirection = Random.Range(0, 4);
        isWalking = true;
        walkCounter = walkTime;
    }

    public void TurnAroundRandom()
    {
        WalkDirection = WalkDirection switch
        {
            0 => 3,
            3 => 0,
            1 => 2,
            2 => 1,
            _ => WalkDirection
        };

        walkCounter = walkTime;
        isWalking = true;
        animator.SetBool("isRunning", true);
    }

    public void TurnToNewDirection()
    {
        int newDirection;
        do
        {
            newDirection = Random.Range(0, 4);
        } while (newDirection == WalkDirection);

        WalkDirection = newDirection;
        walkCounter = walkTime;
        isWalking = true;
        animator.SetBool("isRunning", true);
    }

    public void ReturnTo(Vector3 center)
    {
        Vector3 direction = (center - transform.position).normalized;
        direction.y = 0;

        transform.forward = direction;
        WalkDirection = GetClosestDirection(direction);
        walkCounter = walkTime;
        isWalking = true;
        animator.SetBool("isRunning", true);
    }

    private int GetClosestDirection(Vector3 dir)
    {
        Vector3[] directions = {
            Vector3.forward,
            Vector3.right,
            Vector3.left,
            Vector3.back
        };

        float maxDot = -1f;
        int bestDirection = 0;

        for (int i = 0; i < directions.Length; i++)
        {
            float dot = Vector3.Dot(dir, directions[i]);
            if (dot > maxDot)
            {
                maxDot = dot;
                bestDirection = i;
            }
        }

        return bestDirection;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Avoid"))
        {
            TurnToNewDirection();
        }

        if ((other.CompareTag("Player") || (other.CompareTag("NPCWalk") && other.gameObject != gameObject)))
        {
            if (lookCoroutine != null)
                StopCoroutine(lookCoroutine);

            lookCoroutine = StartCoroutine(HandleInteraction(other.transform));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Player") || (other.CompareTag("NPCWalk") && other.gameObject != gameObject)))
        {
            if (lookCoroutine != null)
            {
                StopCoroutine(lookCoroutine);
                lookCoroutine = null;
            }

            if (chatCanvas != null)
                chatCanvas.SetActive(false);

            moveSpeed = originalSpeed;
            isInteracting = false; // RESET trạng thái tương tác
            ChooseDirection();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Avoid"))
        {
            TurnToNewDirection();
        }
    }

    private IEnumerator HandleInteraction(Transform target)
    {
        isInteracting = true;

        if (chatCanvas != null)
            chatCanvas.SetActive(true);

        isWalking = false;
        animator.SetBool("isRunning", false);
        moveSpeed = 0f;

        float timer = interactionDuration;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            Vector3 dir = (target.position - transform.position).normalized;
            dir.y = 0f;

            if (dir != Vector3.zero)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
            }

            yield return null;
        }

        if (chatCanvas != null)
            chatCanvas.SetActive(false);

        moveSpeed = originalSpeed;
        isInteracting = false;
        ChooseDirection();
    }
}
