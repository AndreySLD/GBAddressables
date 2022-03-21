using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LoadWindowView : AssetBundleViewBase
{
    [SerializeField]
    private Button _loadAsetButton;

    [SerializeField]
    private Button _spawnAssetButton;

    [SerializeField]
    private RectTransform _mountRootTransform;

    [SerializeField]
    private AssetReferenceGameObject _loadPrefab;

    private List<AsyncOperationHandle<GameObject>> _addressablePrefabs = new List<AsyncOperationHandle<GameObject>>();

    private void Start()
    {
        _loadAsetButton.onClick.AddListener(LoadAssets);
        _spawnAssetButton.onClick.AddListener(CreatePrefab);
    }

    private void OnDestroy()
    {
        _loadAsetButton.onClick.RemoveAllListeners();
        _spawnAssetButton.onClick.RemoveAllListeners();

        foreach(var addressablePrefab in _addressablePrefabs)
            Addressables.ReleaseInstance(addressablePrefab);

        _addressablePrefabs.Clear();
    }

    private void CreatePrefab()
    {
        var addressablePrefab = Addressables.InstantiateAsync(_loadPrefab, _mountRootTransform);
        _addressablePrefabs.Add(addressablePrefab);
        StartCoroutine(UnloadAssets());
    }

    private IEnumerator UnloadAssets()
    {
        yield return new WaitForSeconds(4f);
        foreach (var go in _addressablePrefabs)
        {
            Addressables.ReleaseInstance(go);
        }
    }

    private void LoadAssets()
    {
        _loadAsetButton.interactable = false;

        StartCoroutine(DownloadAndSetAssetBundle());
    }
}
