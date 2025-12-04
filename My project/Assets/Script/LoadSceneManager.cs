using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneManager : MonoBehaviour
{
    public LoadSceneManager instance;

    public string CurrenctSceneName;
    public string nextSceneName;
    public AsyncOperation LoadingOperation;
    public Canvas LoadingUi;
    public float progress;
}
