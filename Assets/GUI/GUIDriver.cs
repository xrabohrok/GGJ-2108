using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIDriver : MonoBehaviour
{

    public int quitLevelIndex;

    public void quit()
    {
        SceneManager.LoadScene(quitLevelIndex);

    }
}
