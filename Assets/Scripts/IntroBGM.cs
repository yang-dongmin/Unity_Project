using UnityEngine;

public class IntroScene : MonoBehaviour
{
    public AudioClip introClip;

    void Start()
    {
        AudioManager.Instance.PlayBGM(introClip);
    }
}
