using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Assets.Scripts
{
    public class TutorialNavigationController : MonoBehaviour
    {

        public GameObject leftButton;
        public GameObject rightButton;

        public void LeftButtonClicked()
        {

        }

        public void RightButtonClicked()
        {
            DOTween.KillAll(false);
            SceneManager.LoadScene("Main menu", LoadSceneMode.Single);
        }
    }
}