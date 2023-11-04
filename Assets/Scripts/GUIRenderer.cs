using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class GUIRenderer : MonoBehaviour
    {
        public GameObject potionsText;
        public GameObject spellsText;
        public GameObject moneyText;
        public GameObject livesText;
        public GameObject tokensText;
        public GameObject weaponIcon;
        public GameObject healthBar;
        public GameObject energyBar;
        public Sprite[] barSprites;

        // Start is called before the first frame update
        void Start()
        {

        }

        public void UpdateGUI(TurnState turnState, GameState gameState, Config config)
        {
            potionsText.GetComponent<TextMeshProUGUI>().SetText(turnState.potions.ToString());
            spellsText.GetComponent<TextMeshProUGUI>().SetText(turnState.spells.ToString());
            moneyText.GetComponent<TextMeshProUGUI>().SetText(turnState.money.ToString());
            livesText.GetComponent<TextMeshProUGUI>().SetText(turnState.lives.ToString());
            tokensText.GetComponent<TextMeshProUGUI>().SetText(turnState.tokens.ToString());
            healthBar.GetComponent<Image>().sprite = barSprites[(int)Mathf.Floor(turnState.hp * barSprites.Length / ((float)config.playerDefaultHealthPoints + 1))];
            energyBar.GetComponent<Image>().sprite = barSprites[(int)Mathf.Floor(turnState.energy * barSprites.Length / ((float)config.energy[gameState.upgradeEnergyLevel] + 1))];
        }
    }
}