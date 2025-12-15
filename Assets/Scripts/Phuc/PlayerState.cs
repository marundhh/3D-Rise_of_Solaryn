using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance;

    public enum state { Idle, Walk, Run, Roll, Attack }
    private state currentState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetCurrentState(state.Idle);
    }

    private void StateManager()
    {
        switch (currentState)
        {
            case state.Idle:
                AnimationManager.instance.SetAnimation(0);
                break;

            case state.Walk:
                AnimationManager.instance.SetAnimation(1);
                break;

            case state.Run:
                AnimationManager.instance.SetAnimation(2);
                break;

            case state.Roll:
                AnimationManager.instance.SetAnimation(3);
                break;
        }
    }

    public void SetCurrentState(state newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        StateManager();
        //Debug.Log("State set to: " + currentState);
    }

    public state GetCurrentState()
    {
        return currentState;
    }
}
