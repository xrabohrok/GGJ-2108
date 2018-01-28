using UnityEngine;

public class MusicContinuer : MonoBehaviour
{

    private static MusicContinuer instance = null;

    public static MusicContinuer Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}