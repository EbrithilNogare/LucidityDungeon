using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Assets.Scripts
{
    public class ChapterNavigationController : MonoBehaviour
    {

        public GameObject leftButton;
        public GameObject rightButton;
        public GameObject[] chapters;

        public string previousScene;
        public string nextScene;

        private int index;

        private void Start()
        {
            index = 0;

            foreach (GameObject chapter in chapters)
            {
                chapter.SetActive(false);
            }

            if (chapters.Length > 0)
            {
                chapters[0].SetActive(true);
            }
        }

        public void LeftButtonClicked()
        {
            index--;

            if (index < 0)
            {
                DOTween.KillAll(false);
                SceneManager.LoadScene(previousScene, LoadSceneMode.Single);
                return;
            }

            chapters[index + 1].SetActive(false);
            chapters[index].SetActive(true);
        }

        public void RightButtonClicked()
        {
            index++;

            if (index > chapters.Length - 1)
            {
                DOTween.KillAll(false);
                SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
                return;
            }

            chapters[index - 1].SetActive(false);
            chapters[index].SetActive(true);
        }
    }
}