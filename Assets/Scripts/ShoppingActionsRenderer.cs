using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    class ShoppingActionsRenderer : MonoBehaviour
    {
        [Header("Editable")]

        [Space(100)]

        [Header("From prefab")]
        public GameObject initSpells;
        public GameObject spellStrength;
        public GameObject initPotions;
        public GameObject potionsStrength;
        public GameObject energyUpgrade;
        public GameObject enemyLevel;
        public GameObject enemyCount;
        public GameObject exit;

        public void InitSpells() { }
        public void SpellStrength() { }
        public void InitPotions() { }
        public void PotionsStrength() { }
        public void EnergyUpgrade() { }
        public void EnemyLevel() { }
        public void EnemyCount() { }
        public void Exit() { }

        public void RenderShoppingActions(List<ShoppingHallAction> actions, GameState gameState, Config config)
        {
            initSpells.SetActive(actions.Contains(ShoppingHallAction.upgradeInitSpells));
            spellStrength.SetActive(actions.Contains(ShoppingHallAction.upgradeSpellLevel));
            initPotions.SetActive(actions.Contains(ShoppingHallAction.upgradeInitPotions));
            potionsStrength.SetActive(actions.Contains(ShoppingHallAction.upgradePotionLevel));
            energyUpgrade.SetActive(actions.Contains(ShoppingHallAction.upgradeEnergyLevel));
            enemyLevel.SetActive(actions.Contains(ShoppingHallAction.upgradeEnemyLevel));
            enemyCount.SetActive(actions.Contains(ShoppingHallAction.upgradeEnemyAndTreasureProbability));
        }
    }
}
