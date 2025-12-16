using UnityEngine;
using System.Collections;

public class BuffaloAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float lifeTime = 8f; // tự hủy tránh rác

    [Header("Damage Settings")]
    public float damage = 15f; // Damage mặc định
    private EnemyStats owner;  // Boss hoặc enemy tạo ra Buffalo

    private Vector3 moveDirection;
    private bool hasDirection = false;

    [Header("Impact Settings")]
    public float pullDuration = 0.7f; // thời gian kéo theo Player
    public float pullStrength = 6f;   // tốc độ kéo

    private bool isPulling = false;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetMoveDirection(Vector3 dir)
    {
        if (dir.sqrMagnitude < 0.0001f) return;
        moveDirection = dir.normalized;
        hasDirection = true;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    public void SetOwner(EnemyStats enemy)
    {
        owner = enemy;
        if (owner != null)
            damage = owner.damage;
    }

    private void Update()
    {
        if (hasDirection && !isPulling)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPulling && other.CompareTag("Player"))
        {
            Debug.Log("[BuffaloAI] Đâm trúng Player!");
            StartCoroutine(PullPlayer(other));
        }

        if (!isPulling && other.CompareTag("Minion"))
        {
            MinionStats minionStats = other.GetComponent<MinionStats>();
            if (minionStats != null)
            {
                minionStats.TakeDamage(damage);
                Debug.Log($"Buffalo gây {damage} damage lên Minion.");
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator PullPlayer(Collider player)
    {
        isPulling = true;
        CharacterController controller = player.GetComponent<CharacterController>();

        // Khóa input của Player khi bị Buffalo kéo
        PlayerMovement.isInputLocked = true;

        float elapsed = 0f;
        while (elapsed < pullDuration && controller != null)
        {
            elapsed += Time.deltaTime;

            // Kéo player theo hướng Buffalo
            Vector3 pullDir = moveDirection * pullStrength * Time.deltaTime;
            controller.Move(pullDir);

            // Buffalo bám theo Player
            transform.position = player.transform.position - moveDirection * 1f;

            yield return null;
        }

        // Sau khi kéo xong thì gây damage
        if (PlayerStats.instance != null)
        {
            PlayerStats.instance.TakeDamage(damage);
            Debug.Log($"Buffalo gây {damage} damage lên Player.");
        }

        // Mở lại input cho Player
        PlayerMovement.isInputLocked = false;

        Destroy(gameObject);
    }

}
