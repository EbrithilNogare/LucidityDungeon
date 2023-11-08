using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    public int audioValue;


    public Image audioBar;
    public Sprite[] barSprites;

    void Start()
    {
        audioBar.sprite = barSprites[audioValue];

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void audioPlus()
    {
        audioValue = Mathf.Clamp(audioValue + 1, 0, 5);
        audioBar.sprite = barSprites[audioValue];
    }

    public void audioMinus()
    {
        audioValue = Mathf.Clamp(audioValue - 1, 0, 5);
        audioBar.sprite = barSprites[audioValue];
    }
}
