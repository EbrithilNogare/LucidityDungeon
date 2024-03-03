using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class MenuController : MonoBehaviour
    {
        public RectTransform optionsMenu;

        private bool optionsMenuOpen = false;

        public void Play()
        {
            DOTween.KillAll(true);
            if (Store._instance.achievementProgress == 0)
            {
                SceneManager.LoadScene("Story", LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
        }
        public void Tutorial()
        {
            DOTween.KillAll(true);
            SceneManager.LoadScene("Story", LoadSceneMode.Single);
        }
        public void Options()
        {
            if (optionsMenuOpen)
            {
                optionsMenu.DOAnchorPos(new Vector2(900, 30), .5f);
            }
            else
            {
                optionsMenu.DOAnchorPos(new Vector2(-100, 30), .5f);
            }
            optionsMenuOpen = !optionsMenuOpen;
        }
        public void Credits()
        {
            DOTween.KillAll(true);
            SceneManager.LoadScene("Credits", LoadSceneMode.Single);
        }
        public void Exit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}