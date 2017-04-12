﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    // SceneManagers
    private PlayManager playManager;
    private MenuManager menuManager;

    private string currentSceneName;
    private string lastSceneName = "QUIT";

    private Slider loadingBarSlider;
    private Canvas loadingBarCanvas;

    private void Awake()
    {
        loadingBarSlider = GetComponentInChildren<Slider>();
        loadingBarCanvas = GetComponentInChildren<Canvas>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        StartLoading("Menu");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        currentSceneName = scene.name;

        switch (currentSceneName)
        {
            case "Menu":
                SetMenuScene();
                lastSceneName = "QUIT";
                break;
            case "Play":
                SetPlayScene();
                lastSceneName = "Menu";
                break;
            default:
                break;
        }
    }

    private void SetPlayScene()
    {
        playManager = FindObjectOfType<PlayManager>();
        playManager.backButton.onClick.AddListener(() => StartLoading("Menu"));
    }

    private void SetMenuScene()
    {
        menuManager = FindObjectOfType<MenuManager>();
        menuManager.playButton.onClick.AddListener(() => StartLoading("Play"));
        menuManager.quitButton.onClick.AddListener(Quit);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (lastSceneName == "QUIT")
                Quit();
            else
                StartLoading(lastSceneName);
        }
    }

    public void StartLoading(string sceneToLoad)
    {
        loadingBarCanvas.gameObject.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneToLoad));
        loadingBarCanvas.gameObject.SetActive(false);
    }

    public IEnumerator LoadSceneAsync(string sceneToLoad)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        while (!asyncOperation.isDone)
        {
            loadingBarSlider.value = asyncOperation.progress;
            yield return null;
        }
    }

    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}