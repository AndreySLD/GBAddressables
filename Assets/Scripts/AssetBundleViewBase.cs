using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

public class AssetBundleViewBase : MonoBehaviour
{
    private const string UrlAssetBundleSprites = "https://drive.google.com/uc?export=download&id=1xbM9c8OLEVYuk7Xsq_5F4dFYNB6HarGx";
    private const string UrlAssetBundleAudio = "https://drive.google.com/uc?export=download&id=1c2ivFzUSnPtabzUV58q75AQgiCYJyuWv";

    [SerializeField]
    private DataSpriteBundle[] _dataSpriteBundles;

    [SerializeField]
    private DataAudioBundle[] _dataAudioBundles;

    [SerializeField] 
    private SpriteReferenceData[] _referenceData;

    private AssetBundle _spritesAssetBundle;
    private AssetBundle _audioAssetBundle;

    protected IEnumerator DownloadAndSetAssetBundle()
    {
        yield return GetSpritesAssetBundle();
        yield return GetAudioAssetBundle();

        if (_spritesAssetBundle == null || _audioAssetBundle == null)
        {
            Debug.LogError("Error in asset bundle");
            yield break;
        }

        SetDownloadAsset();
        yield return null;

        foreach (var spriteRef in _referenceData)
        {
            var operation = Addressables.LoadAssetAsync<Sprite>(spriteRef.TargetSprite);
            yield return operation;
            spriteRef.TargetImage.sprite = operation.Result;
        }
    }

    private void SetDownloadAsset()
    {
        foreach(var data in _dataSpriteBundles)
            data.Image.sprite = _spritesAssetBundle.LoadAsset<Sprite>(data.NameAssetBundle);

        foreach (var data in _dataAudioBundles)
        {
            data.AudioSource.clip = _audioAssetBundle.LoadAsset<AudioClip>(data.NameAssetBundle);
            data.AudioSource.Play();
        }
    }

    private IEnumerator GetAudioAssetBundle()
    {
        var request = UnityWebRequestAssetBundle.GetAssetBundle(UrlAssetBundleAudio);

        yield return request.SendWebRequest();

        while (!request.isDone)
            yield return null;

        StateRequest(request, ref _audioAssetBundle);
    }

    private IEnumerator GetSpritesAssetBundle()
    {
        var request = UnityWebRequestAssetBundle.GetAssetBundle(UrlAssetBundleSprites);

        yield return request.SendWebRequest();

        while (!request.isDone)
            yield return null;

        StateRequest(request, ref _spritesAssetBundle);
    }

    private void StateRequest(UnityWebRequest request, ref AssetBundle assetBundle)
    {
        if (request.error == null)
        {
            assetBundle = DownloadHandlerAssetBundle.GetContent(request);
            Debug.LogError("COMPLETE!!!");
        }
        else
        {
            Debug.LogError(request.error);
        }
    }
}
