using UnityEngine;

public class GameQuitter : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
    }
}
