using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TimeLock))]
[RequireComponent(typeof(AssetLoader))]
public class TimelockedBox : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    private TimeLock timeLock;

    private AssetLoader assetLoader;

    private void Start()
    {
        timeLock = GetComponent<TimeLock>();
        assetLoader = GetComponent<AssetLoader>();

        timeLock.OnStateChanged.AddListener(OnStateChanged);
        timeLock.OnOpened.AddListener(OnOpened);
    }

    private void OnOpened()
    {
        StartCoroutine(assetLoader.Instantiate());
    }

    private void OnStateChanged(TimeLock.State newState)
    {
        animator.SetBool("Open", newState == TimeLock.State.Opened);
    }

    private IEnumerator CleanupAfterAssetLoaded()
    {
        foreach(var child in GetComponentsInChildren<Transform>())
        {
            if(child.transform != transform)
                Destroy(child.gameObject);
        }

        yield return new WaitUntil(() => assetLoader.Instantiated);
        Destroy(gameObject);
    }

    public void Cleanup()
    {
        StartCoroutine(CleanupAfterAssetLoaded());
    }
}
