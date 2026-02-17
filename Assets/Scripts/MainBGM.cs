using UnityEngine;

public class PlayBGMPlayer : MonoBehaviour
{
    public AudioClip playClip; 

    void Start()
    {
        AudioManager.Instance.PlayBGM(playClip);
    }
}
