using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ProjectPackager : MonoBehaviour
{
    public AssetReference persisitentScene;

    // Start is called before the first frame update
    private void Awake()
    {
        Addressables.LoadSceneAsync(persisitentScene);
    }
}
