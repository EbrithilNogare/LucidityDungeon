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
            SceneManager.LoadScene("Main menu", LoadSceneMode.Single);
        }
    }
}