using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Main");
    }
}
