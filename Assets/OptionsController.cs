using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    public int audioValue;
    public int soundsValue;


    public Image audioBar;
    public Image soundsBar;
    public Sprite[] barSprites;

    void Start()
    {
        audioBar.sprite = barSprites[audioValue];
        soundsBar.sprite = barSprites[soundsValue];

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

    public void soundsPlus()
    {
        soundsValue = Mathf.Clamp(soundsValue + 1, 0, 5);
        soundsBar.sprite = barSprites[soundsValue];
    }

    public void soundsMinus()
    {
        soundsValue = Mathf.Clamp(soundsValue - 1, 0, 5);
        soundsBar.sprite = barSprites[soundsValue];
    }
}
