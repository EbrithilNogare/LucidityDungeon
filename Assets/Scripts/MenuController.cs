using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject optionsMenu;

    private bool optionsMenuOpen = false;

    public void Continue()
    {

    }
    public void NewGame()
    {
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    public void Options()
    {
        if (optionsMenuOpen)
        {
            optionsMenu.transform.DOLocalMove(new Vector3(1860, -440, 0), .5f);
        }
        else
        {
            optionsMenu.transform.DOLocalMove(new Vector3(860, -440, 0), .5f);
        }
        optionsMenuOpen = !optionsMenuOpen;
    }
    public void Credits()
    {

    }
    public void Exit()
    {
        Application.Quit();
    }
}
