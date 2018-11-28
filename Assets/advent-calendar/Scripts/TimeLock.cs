using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimeLock : MonoBehaviour
{
    public enum State
    {
        Locked,
        Unlocked,
        Opened,
    }

    [Serializable]
    public class StateChanged : UnityEvent<State> { }

    public const string TAG = "TimeLocked";

    [Range(1, 24)]
    public int Day = 1;

    public StateChanged OnStateChanged;

    public UnityEvent OnUnlocked;

    public UnityEvent OnOpened;

    private InternalClock clock;

    private State _currentState = State.Locked;
    public State CurrentState
    {
        get { return _currentState; }
        private set
        {
            if(value != _currentState)
            {
                _currentState = value;

                OnStateChanged.Invoke(value);
                switch(_currentState)
                {
                    case State.Unlocked:
                        OnUnlocked.Invoke();
                        break;
                    case State.Opened:
                        OnOpened.Invoke();
                        break;
                }
            }
        }
    }

    private void Start()
    {
        clock = InternalClock.Instance;
        clock.OnDayChanged.AddListener(OnDayChanged);

        gameObject.tag = TAG;
        OnDayChanged();
    }

    private void OnDayChanged()
    {
        if(CurrentState == State.Locked && clock.IsPast(Day))
        {
            CurrentState = State.Unlocked;
        }
    }

    public bool TryOpen()
    {
        if(CurrentState == State.Unlocked)
        {
            CurrentState = State.Opened;
            return true;
        }
        return false;
    }
}
