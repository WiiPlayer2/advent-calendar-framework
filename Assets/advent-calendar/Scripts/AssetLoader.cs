using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AssetLoader : MonoBehaviour
{
#if UNITY_EDITOR
    public const string ASSETBUNDLE_DIRECTORY = "Assets/AssetBundles";
#else
    public const string ASSETBUNDLE_DIRECTORY = "AssetBundles";
#endif

    [SerializeField]
    private GameObject loadingObject;

    [SerializeField]
    public string AssetBundle;

    [SerializeField]
    public string AssetName;

    [SerializeField]
    public string AssetUrl;

    [SerializeField]
    private Vector3 targetPosition;

    [SerializeField]
    private Vector3 targetRotation;

    [SerializeField]
    private Vector3 targetScale = Vector3.one;

    private bool isLoading = false;

    private AssetBundle assets;

    private GameObject loadedAsset;

    public bool Instantiated { get; private set; }

    public IEnumerator Load()
    {
        if (assets != null)
            yield break;

        if (isLoading)
        {
            yield return new WaitWhile(() => isLoading);
            yield break;
        }

        isLoading = true;
        var file = System.IO.Path.Combine(ASSETBUNDLE_DIRECTORY, AssetBundle);
        if(!File.Exists(file))
        {
            using(var www = UnityWebRequest.Get(AssetUrl))
            {
                var handler = new DownloadHandlerFile(file);
                www.downloadHandler = handler;
                yield return www.SendWebRequest();
                yield return new WaitUntil(() => handler.isDone);
            }
        }

        if (File.Exists(file))
        {
            var request = UnityEngine.AssetBundle.LoadFromFileAsync(file);
            yield return request;
            assets = request.assetBundle;
        }

        isLoading = false;
    }

    public IEnumerator Instantiate()
    {
        var loadingGameObj = Instantiate(loadingObject, transform.position + targetPosition, loadingObject.transform.rotation);

        if (assets == null)
            yield return Load();

        if (loadedAsset == null)
        {
            var request = assets.LoadAssetAsync<GameObject>(AssetName);
            yield return request;
            loadedAsset = request.asset as GameObject;
        }

        Destroy(loadingGameObj);

        var gameObj = Instantiate(loadedAsset, transform.position + targetPosition, Quaternion.Euler(targetRotation));
        gameObj.transform.localScale = targetScale;

        Instantiated = true;
    }
}
