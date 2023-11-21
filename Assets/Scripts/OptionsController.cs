using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class OptionsController : MonoBehaviour
    {
        public Image audioBar;
        public Image soundsBar;
        public Sprite[] barSprites;

        private int audioValue;
        private int soundsValue;

        void Start()
        {
            audioValue = Store._instance.volume;
            soundsValue = Store._instance.sounds;

            audioBar.sprite = barSprites[audioValue];
            soundsBar.sprite = barSprites[soundsValue];
        }

        public void audioPlus()
        {
            audioValue = Mathf.Clamp(audioValue + 1, 0, 5);
            audioBar.sprite = barSprites[audioValue];
            Store._instance.volume = audioValue;
            Store._instance.SavePrefs();
        }

        public void audioMinus()
        {
            audioValue = Mathf.Clamp(audioValue - 1, 0, 5);
            audioBar.sprite = barSprites[audioValue];
            Store._instance.volume = audioValue;
            Store._instance.SavePrefs();
        }

        public void soundsPlus()
        {
            soundsValue = Mathf.Clamp(soundsValue + 1, 0, 5);
            soundsBar.sprite = barSprites[soundsValue];
            Store._instance.sounds = soundsValue;
            Store._instance.SavePrefs();
        }

        public void soundsMinus()
        {
            soundsValue = Mathf.Clamp(soundsValue - 1, 0, 5);
            soundsBar.sprite = barSprites[soundsValue];
            Store._instance.sounds = soundsValue;
            Store._instance.SavePrefs();
        }
    }
}