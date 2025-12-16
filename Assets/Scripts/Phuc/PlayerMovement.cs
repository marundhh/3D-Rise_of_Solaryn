using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    public static bool isInputLocked = false;


    private float currentMoveSpeed;
    private float sprintMultiplier = 1.5f;

    private float dodgeDistance = 10f;
    private float dodgeSpeed = 10f;
    private float dodgeCooldown = 1f;

    public CharacterController characterController;
    private Vector3 moveDirection;

    private float verticalVelocity;
    private float gravity = -9.8f;

    private bool canDodge = true;

    private bool isWalkingSoundPlaying = false;
    private bool isRunningSoundPlaying = false;

    private void Update()
    {
        if (isInputLocked || PlayerState.instance.GetCurrentState() == PlayerState.state.Attack) return;

        currentMoveSpeed = PlayerStats.instance.currentMoveSpeed;

        if (AnimationManager.instance.CheckRollingComplete())
            Move();

        if (Input.GetKeyDown(KeyCode.Space) && canDodge)
            StartCoroutine(DodgeRoll());

        Gravity();
    }

    private IEnumerator DodgeRoll()
    {
        PlayerState.instance.SetCurrentState(PlayerState.state.Roll);
        canDodge = false;

        AudioManager.Instance.PlayRollSFX();

        Vector3 dodgeDirection = moveDirection.magnitude > 0.1f ? moveDirection.normalized : transform.forward;
        float dodgeTime = dodgeDistance / dodgeSpeed;

        float elapsedTime = 0f;
        while (elapsedTime < dodgeTime)
        {
            characterController.Move(dodgeDirection * dodgeSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        AnimationManager.instance.RollingComplete();

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 inputDir = new Vector3(moveX, 0f, moveZ).normalized;

        if (inputDir.magnitude > 0.1f)
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? currentMoveSpeed * sprintMultiplier : currentMoveSpeed;
            Vector3 continueMove = inputDir * speed * Time.deltaTime;
            characterController.Move(continueMove);

            Quaternion toRotation = Quaternion.LookRotation(inputDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 700 * Time.deltaTime);
        }

        PlayerState.instance.SetCurrentState(PlayerState.state.Idle);

        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }



    private void Move()
    {
        float moveX = Input.GetAxis("Horizontal"); // Lấy giá trị A/D
        float moveZ = Input.GetAxis("Vertical");   // Lấy giá trị W/S

        // Lấy hướng forward (trước) và right (phải) của camera
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // Bỏ thành phần Y để không làm nhân vật bị nghiêng khi camera nhìn từ trên xuống
        camForward.y = 0f;
        camRight.y = 0f;

        // Chuẩn hóa vector để đảm bảo độ dài = 1
        camForward.Normalize();
        camRight.Normalize();

        // Tính hướng di chuyển dựa trên input và hướng camera
        Vector3 inputDirection = camForward * moveZ + camRight * moveX;
        inputDirection.Normalize(); // Đảm bảo tổng vector có độ dài = 1

        

        // Nếu đang nhấn Shift thì chạy nhanh hơn
        float speed = Input.GetKey(KeyCode.LeftShift) ? currentMoveSpeed * sprintMultiplier : currentMoveSpeed;

        // Áp dụng tốc độ cho hướng di chuyển
        moveDirection = inputDirection * speed;

        if (inputDirection.magnitude > 0.1f)
        {
            // Di chuyển nhân vật
            characterController.Move(moveDirection * Time.deltaTime);

            // Quay mặt nhân vật về hướng đang di chuyển
            Quaternion toRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 700 * Time.deltaTime);

            // Cập nhật trạng thái nhân vật theo hành vi
            if (Input.GetKey(KeyCode.LeftShift))
            {
                PlayerState.instance.SetCurrentState(PlayerState.state.Run);

                if (!isRunningSoundPlaying)
                {
                    AudioManager.Instance.PlayRunSFX();
                    isRunningSoundPlaying = true;
                    isWalkingSoundPlaying = false;
                }
            }
            else
            {
                PlayerState.instance.SetCurrentState(PlayerState.state.Walk);

                if (!isWalkingSoundPlaying)
                {
                    AudioManager.Instance.PlayWalkSFX();
                    isWalkingSoundPlaying = true;
                    isRunningSoundPlaying = false;
                }
            }
        }
        else
        {
            PlayerState.instance.SetCurrentState(PlayerState.state.Idle); // Không di chuyển

            AudioManager.Instance.StopFootstepSFX();
            //AudioManager.Instance.StopFootstepSFX();
            isWalkingSoundPlaying = false;
            isRunningSoundPlaying = false;
        }
    }


    private void Gravity()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        characterController.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
}
