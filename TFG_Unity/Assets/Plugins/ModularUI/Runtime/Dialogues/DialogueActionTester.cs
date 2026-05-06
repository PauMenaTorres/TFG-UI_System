using UnityEngine;

namespace ModularUIRuntime
{
    public class DialogueActionTester : MonoBehaviour
    {
        public void TeleportToPueblo()
        {
            Debug.Log("ACCIÓN DISPARADA: ¡Te has teletransportado al pueblo!");
        }

        public void GiveSword()
        {
            Debug.Log("ACCIÓN DISPARADA: ¡Has recibido una Espada Larga!");
        }

        public void TakeDamage()
        {
            Debug.Log("ACCIÓN DISPARADA: ¡Pierdes 10 puntos de vida!");
        }
    }
}