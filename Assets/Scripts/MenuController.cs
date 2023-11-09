using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public RectTransform optionsMenu;

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
            optionsMenu.DOAnchorPos(new Vector2(900, 100), .5f);
        }
        else
        {
            optionsMenu.DOAnchorPos(new Vector2(-100, 100), .5f);
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
