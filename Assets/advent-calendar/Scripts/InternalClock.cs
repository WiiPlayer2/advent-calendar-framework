using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InternalClock : MonoBehaviour
{
#if DEBUG
    public bool UseDevelopmentMode = true;

    public UDateTime OverrideDate = DateTime.Today;
#endif

    public int Year = DateTime.Today.Year;

    [Range(1f, 60f)]
    public float UpdateInterval = 1f;

    public UnityEvent OnDayChanged;

    private DateTime GetCurrentTime()
    {
#if DEBUG
        if (UseDevelopmentMode)
        {
            Debug.Log($"Override date: {OverrideDate.dateTime}");
            return OverrideDate;
        }
        else
        {
#endif
            return DateTime.Today;
#if DEBUG
        }
#endif
    }

    private void Start()
    {
        if (FindObjectsOfType<InternalClock>().Length > 1)
        {
            Debug.LogWarning("There is more than one InternalClock component in the scene");
        }
        StartCoroutine(UpdateTime());
    }

    public static InternalClock Instance
    {
        get
        {
            return FindObjectOfType<InternalClock>();
        }
    }

    public bool IsPast(int day)
    {
        return new DateTime(Year, 12, day) <= GetCurrentTime();
    }

    private IEnumerator UpdateTime()
    {
        var lastTime = DateTime.Today;
        while (true)
        {
            yield return new WaitForSecondsRealtime(UpdateInterval);

            var currentTime = GetCurrentTime();
            if (currentTime != lastTime)
            {
                OnDayChanged.Invoke();
            }
            lastTime = currentTime;
        }
    }
}
