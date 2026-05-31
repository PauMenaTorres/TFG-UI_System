using UnityEngine;

namespace ModularUIRuntime.Demo
{
    public class DemoTrigger : MonoBehaviour
    {
        public enum TriggerType
        {
            Damage,
            DrainStamina,
            Heal,
            AddStamina
        }

        public TriggerType type;
        public float amount = 0.2f;
        public float cooldown = 1.5f;

        private float lastTriggerTime = -999f;

        private void OnTriggerEnter(Collider other)
        {
            TriggerAction(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TriggerAction(other);
        }

        private void TriggerAction(Collider other)
        {
            if (Time.time - lastTriggerTime < cooldown)
            {
                return;
            }

            DemoGameManager gameManager = FindFirstObjectByType<DemoGameManager>();
            if (gameManager != null && gameManager.playerTransform != null)
            {
                if (other.transform == gameManager.playerTransform || other.transform.IsChildOf(gameManager.playerTransform))
                {
                    lastTriggerTime = Time.time;
                    switch (type)
                    {
                        case TriggerType.Damage:
                            gameManager.ApplyDamage(amount);
                            gameManager.hudController?.ShowSimplePopUp("Took " + (amount * 100f) + "% damage!", 2f);
                            break;
                        case TriggerType.DrainStamina:
                            gameManager.UseMana(amount);
                            gameManager.hudController?.ShowSimplePopUp("Lost " + (amount * 100f) + "% stamina!", 2f);
                            break;
                        case TriggerType.Heal:
                            gameManager.Heal(amount);
                            gameManager.hudController?.ShowSimplePopUp("Healed " + (amount * 100f) + "% health!", 2f);
                            break;
                        case TriggerType.AddStamina:
                            gameManager.RestoreMana(amount);
                            gameManager.hudController?.ShowSimplePopUp("Restored " + (amount * 100f) + "% stamina!", 2f);
                            break;
                    }
                }
            }
        }
    }
}
