using DG.Tweening;
using UnityEngine;

public class TutorialDirector : MonoBehaviour
{
    public RectTransform player;
    public GameObject hiddenTile;
    public GameObject button;

    public void OnMoveClicked()
    {
        button.SetActive(false);
        player.DOAnchorPos(new Vector2(0, 0), .5f).OnComplete(() =>
        {
            hiddenTile.SetActive(true);
            player.DOAnchorPos(new Vector2(256, 0), .5f);
        });
    }
}
