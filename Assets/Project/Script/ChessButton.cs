using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChessButton : MonoBehaviour
{
    public void ChangeChessScene()
    {
        Debug.Log("�� ��ü");
        SceneManager.LoadSceneAsync("ChessScene");
    }
}
