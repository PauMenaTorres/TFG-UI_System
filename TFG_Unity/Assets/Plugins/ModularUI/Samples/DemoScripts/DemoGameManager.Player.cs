using UnityEngine;
using UnityEngine.InputSystem;

namespace ModularUIRuntime.Demo
{
    public partial class DemoGameManager
    {
        public void ApplyDamage(float amount)
        {
            currentHealth = Mathf.Clamp01(currentHealth - amount);
            hudController?.SetHealth(currentHealth);
        }

        public void Heal(float amount)
        {
            currentHealth = Mathf.Clamp01(currentHealth + amount);
            hudController?.SetHealth(currentHealth);
        }

        public void UseMana(float amount)
        {
            currentMana = Mathf.Clamp01(currentMana - amount);
            hudController?.SetSecondaryResource(currentMana);
        }

        public void RestoreMana(float amount)
        {
            currentMana = Mathf.Clamp01(currentMana + amount);
            hudController?.SetSecondaryResource(currentMana);
        }

        public void TriggerInteract()
        {
            if (nearestPickup != null)
            {
                CollectItem(nearestPickup);
            }
        }

        void Jump()
        {
            if (isGrounded)
            {
                verticalVelocity = jumpForce;
                isGrounded = false;
            }
        }

        void HandleMovement()
        {
            if (playerTransform == null || inventoryOpen || gamePaused)
            {
                return;
            }

            Vector2 move = Vector2.zero;
            Vector2 look = Vector2.zero;

            bool isMobile = GetPlatform() == UIConfiguration.TargetPlatform.MobilePortrait ||
                            GetPlatform() == UIConfiguration.TargetPlatform.MobileLandscape;

            if (mobileInput != null && isMobile)
            {
                move = mobileInput.GetMovementInput();
#if UNITY_EDITOR
                if (move == Vector2.zero && moveAction != null)
                {
                    move = moveAction.ReadValue<Vector2>();
                }
                if (lookAction != null)
                {
                    look = lookAction.ReadValue<Vector2>();
                }
#endif
            }
            else
            {
                if (moveAction != null)
                {
                    move = moveAction.ReadValue<Vector2>();
                }
                if (lookAction != null)
                {
                    look = lookAction.ReadValue<Vector2>();
                }
            }

            yaw += look.x * lookSensitivity;
            pitch -= look.y * lookSensitivity;
            pitch = Mathf.Clamp(pitch, -90f, 90f);
            playerTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 fwd = playerTransform.forward;
            fwd.y = 0;
            fwd.Normalize();
            Vector3 right = playerTransform.right;
            right.y = 0;
            right.Normalize();

            // Simulated gravity & Jump physics
            if (playerTransform.position.y > groundY + 0.05f)
            {
                verticalVelocity += gravity * Time.deltaTime;
                isGrounded = false;
            }
            else
            {
                if (playerTransform.position.y < groundY)
                {
                    Vector3 pos = playerTransform.position;
                    pos.y = groundY;
                    playerTransform.position = pos;
                }
                verticalVelocity = 0f;
                isGrounded = true;
            }

            Vector3 deltaMove = (fwd * move.y + right * move.x) * moveSpeed * Time.deltaTime;
            deltaMove.y = verticalVelocity * Time.deltaTime;

            playerTransform.position += deltaMove;
        }

        void HandlePickupDetection()
        {
            if (playerTransform == null || pickupsParent == null)
            {
                return;
            }

            nearestPickup = null;
            float closest = pickupRange;

            foreach (Transform child in pickupsParent)
            {
                var p = child.GetComponent<DemoPickup>();
                if (p == null || p.isCollected)
                {
                    continue;
                }
                float d = Vector3.Distance(playerTransform.position, child.position);
                if (d < closest)
                {
                    closest = d;
                    nearestPickup = p;
                }
            }

            if (nearestPickup != lastDetectedPickup)
            {
                lastDetectedPickup = nearestPickup;
                if (nearestPickup != null)
                {
                    hudController?.ShowSimplePopUp("Press [E] to collect " + nearestPickup.itemData.itemName, 3f);
                }
            }

            hudController?.SetHotbarObjectName(nearestPickup != null ? "[E] " + nearestPickup.itemData.itemName : "");
        }

        void OnGUI()
        {
            if (!Application.isPlaying) return;

            if (GetPlatform() == UIConfiguration.TargetPlatform.Desktop)
            {
                GUI.depth = -10;
                GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
                Texture2D bgTexture = new Texture2D(1, 1);
                bgTexture.SetPixel(0, 0, new Color(0.05f, 0.05f, 0.05f, 0.75f));
                bgTexture.Apply();
                boxStyle.normal.background = bgTexture;
                boxStyle.border = new RectOffset(0, 0, 0, 0);

                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontSize = 13;
                labelStyle.richText = true;
                labelStyle.normal.textColor = Color.white;
                labelStyle.alignment = TextAnchor.MiddleLeft;

                string text = "<b><color=#00E5FF>CONTROLS</color></b>\n" +
                              "• <b>WASD:</b> Move Player\n" +
                              "• <b>Space:</b> Jump\n" +
                              "• <b>Mouse:</b> Look Around\n" +
                              "• <b>E:</b> Interact / Pick Up\n" +
                              "• <b>I:</b> Toggle Inventory\n" +
                              "• <b>ESC:</b> Toggle Pause Menu";

                float x = 20f;
                float y = Screen.height - 160f;
                float width = 220f;
                float height = 140f;

                GUI.Box(new Rect(x, y, width, height), "", boxStyle);
                GUILayout.BeginArea(new Rect(x + 10f, y + 10f, width - 20f, height - 20f));
                GUILayout.Label(text, labelStyle);
                GUILayout.EndArea();
            }
        }
    }
}
