using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TimeLock))]
public class TimelockedBox : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private TimeLock timeLock;

    private void Start()
    {
        timeLock = GetComponent<TimeLock>();

        timeLock.OnStateChanged.AddListener(OnStateChanged);
    }

    private void OnStateChanged(TimeLock.State newState)
    {
        animator.SetBool("Open", newState == TimeLock.State.Opened);
    }
}
