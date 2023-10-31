using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Continue()
    {

    }
    public void NewGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    public void Options()
    {

    }
    public void Credits()
    {

    }
    public void Exit()
    {
        Application.Quit();
    }
}
