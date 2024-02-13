using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ShoppingHallGUIRenderer : MonoBehaviour
    {
        [Header("Public")]

        [Space(100)]

        [Header("Private")]
        // left, mid, right, solo
        public GameObject[] BarPieceSources;
        public GameObject energyBar;
        public Color32 energyBarColor;
        public GameObject potionBar;
        public Color32 potionBarColor;
        public TextMeshProUGUI potionCount;
        public GameObject spellBar;
        public Color32 spellBarColor;
        public TextMeshProUGUI spellCount;
        public GameObject enemyBar;
        public Color32 enemyBarColor;
        public TextMeshProUGUI enemyCount;
        public TextMeshProUGUI tokensCount;

        private Config config;

        void Start()
        {
            config = new Config();
            RenderGUI();
        }

        public void RenderGUI()
        {
            GameState gameState = Store._instance.gameState;
            RenderBar(energyBar, gameState.upgradeEnergyLevel + 1, energyBarColor);
            RenderBar(potionBar, gameState.upgradePotionLevel + 1, potionBarColor);
            RenderBar(spellBar, gameState.upgradeSpellLevel + 1, spellBarColor);
            RenderBar(enemyBar, gameState.upgradeEnemyLevel + 1, enemyBarColor);

            potionCount.SetText(gameState.upgradeInitPotions.ToString());
            spellCount.SetText(gameState.upgradeInitSpells.ToString());
            enemyCount.SetText((config.enemyAndTreasureProbability[gameState.upgradeEnemyAndTreasureProbability] * 100f) + "%");
            tokensCount.SetText(gameState.lastRunTokens.ToString());
        }

        private void RenderBar(GameObject container, int count, Color32 color)
        {
            var children = new List<GameObject>();
            foreach (Transform child in container.transform)
            {
                children.Add(child.gameObject);
            }
            children.ForEach(child => Destroy(child));

            if (count == 1)
            {
                GameObject copy = Instantiate(BarPieceSources[3]);
                copy.transform.SetParent(container.transform, false);
                copy.GetComponent<Image>().color = color;
            }
            if (count > 1)
            {
                GameObject copyBefore = Instantiate(BarPieceSources[0]);
                copyBefore.transform.SetParent(container.transform, false);
                copyBefore.GetComponent<Image>().color = color;

                for (int i = 2; i < count; i++)
                {
                    GameObject copyIn = Instantiate(BarPieceSources[1]);
                    copyIn.transform.SetParent(container.transform, false);
                    copyIn.GetComponent<Image>().color = color;
                };

                GameObject copyAfter = Instantiate(BarPieceSources[2]);
                copyAfter.transform.SetParent(container.transform, false);
                copyAfter.GetComponent<Image>().color = color;
            }
        }
    }
}