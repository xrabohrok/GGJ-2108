using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIDriver : MonoBehaviour
{

    public int quitLevelIndex;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void quit()
    {
        SceneManager.LoadScene(quitLevelIndex);

    }
}
