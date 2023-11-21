using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class GUIRenderer : MonoBehaviour
    {
        public TextMeshProUGUI potionsText;
        public TextMeshProUGUI spellsText;
        public TextMeshProUGUI moneyText;
        public TextMeshProUGUI livesText;
        public TextMeshProUGUI tokensText;
        public TextMeshProUGUI keysText;
        public TextMeshProUGUI exactHealtPointsValue;
        public TextMeshProUGUI exactEnergyValue;
        public GameObject weaponIcon;
        public GameObject healthBar;
        public GameObject energyBar;
        public Sprite[] barSprites;

        public void UpdateGUI(TurnState turnState, GameState gameState, Config config)
        {
            exactHealtPointsValue.SetText(turnState.hp.ToString() + " / " + config.playerDefaultHealthPoints);
            exactEnergyValue.SetText(turnState.energy.ToString() + " / " + config.energy[gameState.upgradeEnergyLevel]);
            potionsText.SetText(turnState.potions.ToString());
            spellsText.SetText(turnState.spells.ToString());
            moneyText.SetText(turnState.money.ToString());
            livesText.SetText(turnState.lives.ToString());
            tokensText.SetText(turnState.tokens.ToString());
            keysText.SetText(turnState.keys.ToString());
            healthBar.GetComponent<Image>().sprite = barSprites[(int)Mathf.Floor(turnState.hp * barSprites.Length / ((float)config.playerDefaultHealthPoints + 1))];
            energyBar.GetComponent<Image>().sprite = barSprites[(int)Mathf.Floor(turnState.energy * barSprites.Length / ((float)config.energy[gameState.upgradeEnergyLevel] + 1))];
        }
    }
}