using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
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

    private void Update()
    {
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
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(moveX, 0f, moveZ).normalized;

        float speed = Input.GetKey(KeyCode.LeftShift) ? currentMoveSpeed * sprintMultiplier : currentMoveSpeed;

        moveDirection = inputDirection * speed;

        if (inputDirection.magnitude > 0.1f)
        {
            characterController.Move(moveDirection * Time.deltaTime);

            Quaternion toRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 700 * Time.deltaTime);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                PlayerState.instance.SetCurrentState(PlayerState.state.Run);
            }
            else
            {
                PlayerState.instance.SetCurrentState(PlayerState.state.Walk);
            }
        }
        else
        {
            PlayerState.instance.SetCurrentState(PlayerState.state.Idle);
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
