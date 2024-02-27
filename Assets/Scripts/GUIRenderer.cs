using System.Collections.Generic;
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
        public Sprite[] weaponSprites;
        public GameObject weaponIcon;
        public GameObject healthBar;
        public GameObject energyBar;
        public Sprite[] barSprites;
        public GameObject[] healthBarSprites; // 0:solo, 1:left, 2:mid, 3:right 
        public TextMeshProUGUI enemyName;
        public GameObject enemyHealthBarContainer;

        public void UpdateGUI(GameEngine gameEngine)
        {
            var turnState = gameEngine.turnState;
            var gameState = gameEngine.gameState;
            var config = gameEngine.config;

            var exitInfo = gameEngine.DistanceToExit(turnState);
            string exitDirection = "";
            switch (exitInfo.Item2)
            {
                case GameAction.GoUp:
                    exitDirection = "^"; break;
                case GameAction.GoLeft:
                    exitDirection = "<"; break;
                case GameAction.GoDown:
                    exitDirection = "v"; break;
                case GameAction.GoRight:
                    exitDirection = ">"; break;
                case GameAction.Exit:
                    exitDirection = "X"; break;
            }

            exactHealtPointsValue.SetText(turnState.hp.ToString() + " / " + config.playerDefaultHealthPoints);
            exactEnergyValue.SetText(turnState.energy.ToString() + " / " + config.energy[gameState.upgradeEnergyLevel] + "   |   " + exitInfo.Item1 + " " + exitDirection);
            potionsText.SetText(turnState.potions.ToString());
            spellsText.SetText(turnState.spells.ToString());
            moneyText.SetText(turnState.money.ToString());
            livesText.SetText(turnState.lives.ToString());
            tokensText.SetText(turnState.tokens.ToString());
            keysText.SetText(turnState.keys.ToString());
            healthBar.GetComponent<Image>().sprite = barSprites[(int)Mathf.Floor(turnState.hp * barSprites.Length / ((float)config.playerDefaultHealthPoints + 1))];
            energyBar.GetComponent<Image>().sprite = barSprites[(int)Mathf.Floor(turnState.energy * barSprites.Length / ((float)config.energy[gameState.upgradeEnergyLevel] + 1))];
            weaponIcon.GetComponent<Image>().sprite = weaponSprites[turnState.weaponLevel];

            var children = new List<GameObject>();
            foreach (Transform child in enemyHealthBarContainer.transform) children.Add(child.gameObject);
            children.ForEach(child => Destroy(child));

            if (gameEngine.isEnemyInMyRoom(turnState))
            {
                int bars = (turnState.enemyHp + 9) / 10;
                if (bars == 1)
                {
                    GameObject copy = Instantiate(healthBarSprites[0]);
                    copy.transform.SetParent(enemyHealthBarContainer.transform, false);
                }
                if (bars > 1)
                {
                    GameObject copyBefore = Instantiate(healthBarSprites[1]);
                    copyBefore.transform.SetParent(enemyHealthBarContainer.transform, false);

                    for (int i = 2; i < bars; i++)
                    {
                        GameObject copyIn = Instantiate(healthBarSprites[2]);
                        copyIn.transform.SetParent(enemyHealthBarContainer.transform, false);
                    };

                    GameObject copyAfter = Instantiate(healthBarSprites[3]);
                    copyAfter.transform.SetParent(enemyHealthBarContainer.transform, false);
                }

                enemyName.SetText("Enemy   [ " + turnState.enemyHp + " ]"); // todo
            }
            else
            {
                enemyName.SetText("");
            }
        }
    }
}