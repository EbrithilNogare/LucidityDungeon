using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class OptionsController : MonoBehaviour
    {
        public Image audioBar;
        public Image soundsBar;
        public Sprite[] barSprites;
        public TextMeshProUGUI resertProgressLabel;

        private int audioValue;
        private int soundsValue;

        void Start()
        {
            audioValue = Store._instance.volume;
            soundsValue = Store._instance.sounds;

            audioBar.sprite = barSprites[audioValue];
            soundsBar.sprite = barSprites[soundsValue];
        }

        public void AudioPlus()
        {
            audioValue = Mathf.Clamp(audioValue + 1, 0, 5);
            audioBar.sprite = barSprites[audioValue];
            Store._instance.volume = audioValue;
            Store._instance.SavePrefs();
        }

        public void AudioMinus()
        {
            audioValue = Mathf.Clamp(audioValue - 1, 0, 5);
            audioBar.sprite = barSprites[audioValue];
            Store._instance.volume = audioValue;
            Store._instance.SavePrefs();
        }

        public void SoundsPlus()
        {
            soundsValue = Mathf.Clamp(soundsValue + 1, 0, 5);
            soundsBar.sprite = barSprites[soundsValue];
            Store._instance.sounds = soundsValue;
            Store._instance.SavePrefs();
        }

        public void SoundsMinus()
        {
            soundsValue = Mathf.Clamp(soundsValue - 1, 0, 5);
            soundsBar.sprite = barSprites[soundsValue];
            Store._instance.sounds = soundsValue;
            Store._instance.SavePrefs();
        }

        public void ResetProgress()
        {
            Store._instance.gameState = new GameState();
            Store._instance.SavePrefs();
            resertProgressLabel.SetText("Not much progress to remove, but DONE");
        }
    }
}