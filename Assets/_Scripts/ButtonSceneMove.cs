using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneMove : MonoBehaviour
{
    public void xMoveSceneName(string sceneName) { SceneManager.LoadScene(sceneName); }
}
