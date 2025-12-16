using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject model;
    [SerializeField] private AudioSource audioSource;

    private enum AttackState { Idle, Attack1, WaitingForCombo, Attack2 }
    private AttackState attackState = AttackState.Idle;

    private float attackRange;

    private float comboTimer = 0f;
    private float comboWindow = 0.5f;

    private bool hasDealtDamage = false;


    private void Start()
    {
        attackRange = PlayerStats.instance.currentAttackRange;

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        //Debug.Log(attackState);
        switch (attackState)
        {
            case AttackState.Idle:
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    RotateToMouse();

                    StartAttack1();
                }
                break;

            case AttackState.WaitingForCombo:
                comboTimer -= Time.deltaTime;
                if (comboTimer <= 0f)
                {
                    ResetAttack();
                }
                else if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    RotateToMouse();

                    StartAttack2();
                }
                break;
        }
    }

    private void StartAttack1()
    {
        PlayAttackSound();

        SetModelRotationY(50f);
        attackState = AttackState.Attack1;
        hasDealtDamage = false;

        PlayerState.instance.SetCurrentState(PlayerState.state.Attack);
        AnimationManager.instance.PlayAttackCombo(1);

        if (attackTimeoutCoroutine != null) StopCoroutine(attackTimeoutCoroutine);
        attackTimeoutCoroutine = StartCoroutine(AttackTimeout(1f)); // 1 giây
    }

    private void StartAttack2()
    {
        PlayAttackSound();

        SetModelRotationY(50f);
        attackState = AttackState.Attack2;
        hasDealtDamage = false;

        AnimationManager.instance.PlayAttackCombo(2);

        if (attackTimeoutCoroutine != null) StopCoroutine(attackTimeoutCoroutine);
        attackTimeoutCoroutine = StartCoroutine(AttackTimeout(1f)); // 1 giây
    }

    private void PlayAttackSound()
    {
        var clip = ClassDataPlayerChoose.instance.characterClassData.attackSound;

        if (clip != null)
        {
            AudioManager.Instance.PlayAttackSFX(clip);
        }
    }

    private void RotateToMouse()
    {
        Debug.Log("Click");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            Vector3 direction = (hitPoint - transform.position).normalized;
            direction.y = 0f;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }

            /*Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow, 2f);
            Debug.DrawLine(transform.position, hitPoint, Color.red, 2f);*/

            Debug.DrawLine(firePoint.position, firePoint.position + direction * attackRange, Color.black, 1f);
        }
    }

    private void AttackRay()
    {
        float currentRange = PlayerStats.instance.currentAttackRange; // luôn lấy mới
        Vector3 direction = transform.forward;

        if (Physics.Raycast(firePoint.position, direction, out RaycastHit enemyHit, currentRange, enemyLayer))
        {
            EnemyStats targetStats = enemyHit.collider.GetComponent<EnemyStats>();
            if (targetStats != null)
            {
                float damage = PlayerStats.instance.currentPhysicalDamage + PlayerStats.instance.currentMagicDamage
                    + WeaponManager.instance.selectedWeaponData.damage;
                Debug.Log("Damage player: " + damage);
                targetStats.TakeDamage(damage);
            }
        }

        Debug.DrawLine(firePoint.position, firePoint.position + direction * currentRange, Color.white, 1f);
    }
    private Coroutine attackTimeoutCoroutine;

    private IEnumerator AttackTimeout(float time)
    {
        yield return new WaitForSeconds(time);
        if (attackState == AttackState.Attack1 || attackState == AttackState.Attack2)
        {
            Debug.LogWarning("Attack timeout - resetting state");
            ResetAttack();
        }
    }

    public void OnAttack1Complete()
    {
        if (attackTimeoutCoroutine != null) StopCoroutine(attackTimeoutCoroutine);
        SetModelRotationY(0f);
        attackState = AttackState.WaitingForCombo;
        comboTimer = comboWindow;
    }

    public void OnAttack2Complete()
    {
        if (attackTimeoutCoroutine != null) StopCoroutine(attackTimeoutCoroutine);
        SetModelRotationY(0f);
        ResetAttack();
    }

    private IEnumerator AutoResetAfter(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (attackState == AttackState.WaitingForCombo)
        {
            ResetAttack();
        }
    }

    public void DealDamageAttack()
    {
        if (!hasDealtDamage)
        {
            RotateToMouse();
            AttackRay();
            hasDealtDamage = true;
            Debug.Log("Deal damage");
        }
    }

    private void ResetAttack()
    {
        attackState = AttackState.Idle;
        PlayerState.instance.SetCurrentState(PlayerState.state.Idle);
        hasDealtDamage = false;
    }

    private Coroutine rotateCoroutine;

    private void SetModelRotationY(float yAngle)
    {
        if (model != null)
        {
            if (rotateCoroutine != null) StopCoroutine(rotateCoroutine);
            rotateCoroutine = StartCoroutine(RotateModelToY(yAngle));
        }
    }

    private IEnumerator RotateModelToY(float targetY)
    {
        float duration = 0.1f;
        float time = 0f;

        Vector3 startEuler = model.transform.localEulerAngles;
        Vector3 targetEuler = new Vector3(startEuler.x, targetY, startEuler.z);

        while (time < duration)
        {
            model.transform.localEulerAngles = Vector3.Lerp(startEuler, targetEuler, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        model.transform.localEulerAngles = targetEuler;
    }

}
