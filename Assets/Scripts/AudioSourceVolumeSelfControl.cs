using UnityEngine;
namespace Assets.Scripts
{
    public class AudioSourceVolumeSelfControl : MonoBehaviour
    {
        void Start()
        {
            float soundVolume = Store._instance.sounds == 0 ? 0 : Mathf.Pow(10f, Mathf.Lerp(-40f, 0f, Store._instance.sounds / 5f) / 20f);
            GetComponent<AudioSource>().volume = soundVolume;
        }
    }
}