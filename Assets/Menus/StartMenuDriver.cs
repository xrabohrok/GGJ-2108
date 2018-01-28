using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuDriver : MonoBehaviour
{

    public enum nextScene
    {
        NONE,
        CREDITS,
        GAME,
        SCENEJUMPA,
        END
    };

    private nextScene chosenScene;

    public float startAnimationDelay = .5f;
    private float startDelay = 0;
    public float nextSceneDelay = .5f;
    private float nextDelay = 0;

    public int gameStartSceneIndex = 1;
    public int creditsSceneIndex = 1;

    public int sceneJumpAIndex = 2;

//    private Animator _animations;

	// Use this for initialization
	void Start ()
	{
//	    _animations = this.GetComponent<Animator>();
	    chosenScene = nextScene.NONE;
	}
	
	// Update is called once per frame
	void Update () {
	    if (startDelay > startAnimationDelay)
	    {
//	        _animations.SetTrigger("Start");
	        startDelay = 0;
	    }
	    else
	    {
	        startDelay += Time.deltaTime;
	    }

	    if (chosenScene != nextScene.NONE)
	    {
	        if (nextDelay > nextSceneDelay)
	        {
	            if (chosenScene == nextScene.GAME)
	            {
	                SceneManager.LoadScene(gameStartSceneIndex);
	            }
                else if (chosenScene == nextScene.CREDITS)
	            {
	                SceneManager.LoadScene(creditsSceneIndex);
	            }
                else if (chosenScene == nextScene.END)
	            {
	                Application.Quit();
	            }
                else if (chosenScene == nextScene.SCENEJUMPA)
	            {
	                SceneManager.LoadScene(sceneJumpAIndex);
	            }
	            nextSceneDelay = 0;
	        }
	        else
	        {
	            nextDelay += Time.deltaTime;
	        }
	    }
	}

    public void startGame()
    {
        chosenScene = nextScene.GAME;
//        _animations.SetTrigger("Used");
        Debug.Log("loading new game...");

    }

    public void quitGame()
    {
        chosenScene = nextScene.END;
//        _animations.SetTrigger("Used");
        Debug.Log("quitting...");

    }

    public void credits()
    {
        chosenScene = nextScene.CREDITS;
//        _animations.SetTrigger("Used");
        Debug.Log("loading credits...");
    }

    public void sceneJumpA()
    {
        chosenScene = nextScene.SCENEJUMPA;
//        _animations.SetTrigger("Used");
        Debug.Log("loading credits...");
    }
}
