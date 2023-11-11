using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class CreditsRunner : MonoBehaviour
    {
        public RectTransform text1;
        public RectTransform text2;
        public float moveDistance = 1500f;
        public float moveDuration = 5f;

        void Start()
        {
            Sequence sequence1 = DOTween.Sequence();
            sequence1.Append(text1.DOAnchorPosY(text1.anchoredPosition.y + moveDistance, moveDuration).SetEase(Ease.Linear))
                    .AppendCallback(() => ResetPosition(text1))
                    .SetLoops(-1);

            Sequence sequence2 = DOTween.Sequence();
            sequence2.Append(text2.DOAnchorPosY(text2.anchoredPosition.y + moveDistance, moveDuration).SetEase(Ease.Linear))
                    .AppendCallback(() => ResetPosition(text2))
                    .SetLoops(-1);

            sequence1.Play();
            sequence2.Play();
        }

        void ResetPosition(RectTransform obj)
        {
            obj.anchoredPosition = new Vector2(obj.anchoredPosition.x, obj.anchoredPosition.y - moveDistance);
        }

        public void BackToMainMenu()
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
}