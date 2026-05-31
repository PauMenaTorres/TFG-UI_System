using UnityEngine;

namespace ModularUIRuntime.Demo
{
    public class DemoHotbarActions : MonoBehaviour
    {
        public DemoGameManager gameManager;

        public void UseHealthPotion()
        {
            if (gameManager != null)
            {
                gameManager.Heal(0.2f);
                gameManager.hudController?.ShowSimplePopUp("Used Health Potion!", 2f);
            }
        }

        public void UseManaPotion()
        {
            if (gameManager != null)
            {
                gameManager.RestoreMana(0.3f);
                gameManager.hudController?.ShowSimplePopUp("Used Mana Potion!", 2f);
            }
        }

        public void TriggerSprint()
        {
            if (gameManager != null)
            {
                gameManager.moveSpeed = 8f;
                Invoke(nameof(ResetSpeed), 3f);
                gameManager.hudController?.ShowSimplePopUp("Sprinting!", 1.5f);
            }
        }

        void ResetSpeed()
        {
            if (gameManager != null)
                gameManager.moveSpeed = 5f;
        }
    }
}
