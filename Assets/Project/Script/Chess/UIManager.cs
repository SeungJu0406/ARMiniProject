using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [System.Serializable]
    public struct GameOverUI
    {
        public GameObject box;
        public TextMeshProUGUI text;
    }
    public GameOverUI gameOverUI;

    public GameObject miniMapUI;

    public StringBuilder sb = new StringBuilder();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideGameOverUI();
    }

    public void ShowGameOverUI()
    {
        gameOverUI.box.SetActive(true);
    }
    public void HideGameOverUI()
    {
        gameOverUI.box.SetActive(false);
    }
    public void UpdateGameOverText(StringBuilder sb)
    {
        gameOverUI.text.SetText(sb);
    }

    public void PressGameOverButton(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void TriggerMiniMap()
    {
        if (miniMapUI.activeSelf)
        {
            miniMapUI.SetActive(false);
        }
        else
        {
            miniMapUI.SetActive(true);
        }
    }
}
