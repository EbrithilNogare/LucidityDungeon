using UnityEngine;
using UnityEngine.SceneManagement;
namespace Assets.Scripts
{
    public class EndScreenController : MonoBehaviour
    {
        public GameObject[] variants;

        private void Start()
        {
            foreach (GameObject variant in variants)
            {
                variant.SetActive(false);
            }

            switch (Store._instance.endScreenVariant)
            {
                case Store.EndScreenVariants.Victory: variants[0].SetActive(true); break;
                case Store.EndScreenVariants.Sleep: variants[1].SetActive(true); break;
                case Store.EndScreenVariants.Death: variants[2].SetActive(true); break;
                default: throw new System.Exception("End screen variant not set.");
            }
        }

        public void MainMenuButtonClicked()
        {
            SceneManager.LoadScene("Main menu", LoadSceneMode.Single);
        }

        public void ReturnTuDungeonButtonClicked()
        {
            SceneManager.LoadScene("Shopping hall", LoadSceneMode.Single);
        }
    }
}