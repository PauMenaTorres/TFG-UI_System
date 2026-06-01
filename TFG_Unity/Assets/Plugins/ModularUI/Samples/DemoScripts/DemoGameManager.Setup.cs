using UnityEngine;

namespace ModularUIRuntime.Demo
{
    public partial class DemoGameManager
    {
        void SetupPlayerPhysics()
        {
            if (playerTransform != null)
            {
                Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = playerTransform.gameObject.AddComponent<Rigidbody>();
                }
                rb.isKinematic = true;
                rb.useGravity = false;

                Collider col = playerTransform.GetComponent<Collider>();
                if (col == null)
                {
                    CapsuleCollider cap = playerTransform.gameObject.AddComponent<CapsuleCollider>();
                    cap.radius = 0.5f;
                    cap.height = 2f;
                    cap.center = new Vector3(0, 1, 0);
                }
            }
        }

        void SetupPlayerCamera()
        {
            Camera firstPersonCam = null;
            Camera[] cams = FindObjectsByType<Camera>(FindObjectsSortMode.None);
            foreach (var c in cams)
            {
                if (c.gameObject.name != "MinimapCamera" && c.gameObject.name != "Minimap Camera")
                {
                    firstPersonCam = c;
                    break;
                }
            }

            if (firstPersonCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                camObj.tag = "MainCamera";
                firstPersonCam = camObj.AddComponent<Camera>();
                camObj.AddComponent<AudioListener>();
            }

            if (playerTransform != null)
            {
                firstPersonCam.transform.SetParent(playerTransform);
                firstPersonCam.transform.localPosition = new Vector3(0f, 1.8f, 0f);
                firstPersonCam.transform.localRotation = Quaternion.identity;
            }
            firstPersonCam.orthographic = false;
        }

        void SetupMapColorChanger()
        {
            GameObject ground = GameObject.Find("Ground");
            if (ground != null)
            {
                if (ground.GetComponent<DemoMapColorChanger>() == null)
                {
                    ground.AddComponent<DemoMapColorChanger>();
                }
            }
        }

        void SetupSimplifiedElements()
        {
            if (!Application.isPlaying)
            {
                GameObject existingDamage = GameObject.Find("Damage Trigger");
                GameObject existingHeal = GameObject.Find("Heal Trigger");
                GameObject existingDrain = GameObject.Find("Stamina Drain Trigger");
                GameObject existingAdd = GameObject.Find("Stamina Add Trigger");

                bool pickupsExist = false;
                if (pickupsParent != null && pickupsParent.childCount > 0)
                {
                    pickupsExist = true;
                }

                if (existingDamage != null && existingHeal != null && existingDrain != null && existingAdd != null && pickupsExist)
                {
                    return;
                }
            }

            CleanupExistingDemoObjects();

            ItemData goldCoinData = Resources.Load<ItemData>("ItemGoldCoin");
            ItemData redPotionData = Resources.Load<ItemData>("ItemRedPotion");

            if (goldCoinData != null)
            {
                CreateDemoPickup(goldCoinData, new Vector3(2.5f, 0.5f, 2f));
            }
            if (redPotionData != null)
            {
                CreateDemoPickup(redPotionData, new Vector3(-2.5f, 0.5f, 2f));
            }

            CreateTriggerZone("Damage Trigger", new Vector3(-5f, 1f, 6f), DemoTrigger.TriggerType.Damage, Color.red);
            CreateTriggerZone("Heal Trigger", new Vector3(-2f, 1f, 6f), DemoTrigger.TriggerType.Heal, Color.green);
            CreateTriggerZone("Stamina Drain Trigger", new Vector3(2f, 1f, 6f), DemoTrigger.TriggerType.DrainStamina, new Color(1f, 0.5f, 0f));
            CreateTriggerZone("Stamina Add Trigger", new Vector3(5f, 1f, 6f), DemoTrigger.TriggerType.AddStamina, Color.blue);
        }

        void CleanupExistingDemoObjects()
        {
            if (pickupsParent != null)
            {
                for (int i = pickupsParent.childCount - 1; i >= 0; i--)
                {
                    if (Application.isPlaying)
                        Destroy(pickupsParent.GetChild(i).gameObject);
                    else
                        DestroyImmediate(pickupsParent.GetChild(i).gameObject);
                }
            }

            string[] triggerNames = { "Damage Trigger", "Heal Trigger", "Stamina Drain Trigger", "Stamina Add Trigger" };
            foreach (string tName in triggerNames)
            {
                GameObject existing = GameObject.Find(tName);
                if (existing != null)
                {
                    if (Application.isPlaying)
                        Destroy(existing);
                    else
                        DestroyImmediate(existing);
                }
            }
        }

        void CreateDemoPickup(ItemData data, Vector3 position)
        {
            GameObject pickupGo = new GameObject("Pickup_" + data.itemName);
            if (pickupsParent != null)
            {
                pickupGo.transform.SetParent(pickupsParent);
            }
            pickupGo.transform.position = position;

            DemoPickup pickup = pickupGo.AddComponent<DemoPickup>();
            pickup.itemData = data;
            pickup.quantity = 1;
            pickup.bobAmplitude = 0.15f;
            pickup.bobSpeed = 3f;
            pickup.rotationSpeed = 120f;

            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.transform.SetParent(pickupGo.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localScale = Vector3.one * 0.5f;

            Collider coll = visual.GetComponent<Collider>();
            if (coll != null)
            {
                if (Application.isPlaying)
                    Destroy(coll);
                else
                    DestroyImmediate(coll);
            }

            Renderer rend = visual.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.color = data.type == ItemType.Consumable ? Color.red : Color.yellow;
            }
        }

        void CreateTriggerZone(string name, Vector3 position, DemoTrigger.TriggerType type, Color color)
        {
            GameObject triggerGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            triggerGo.name = name;
            triggerGo.transform.position = position;
            triggerGo.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            Collider col = triggerGo.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }

            Rigidbody rb = triggerGo.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            Renderer rend = triggerGo.GetComponent<Renderer>();
            if (rend != null)
            {
                Shader shader = Shader.Find("Universal Render Pipeline/Simple Lit");
                if (shader == null)
                {
                    shader = Shader.Find("Universal Render Pipeline/Lit");
                }
                if (shader == null)
                {
                    shader = Shader.Find("Standard");
                }
                Material mat = new Material(shader);
                if (shader != null && shader.name.Contains("Universal Render Pipeline"))
                {
                    mat.SetFloat("_Surface", 1);
                    mat.SetFloat("_Blend", 0);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                }
                else
                {
                    mat.SetFloat("_Mode", 3);
                    mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    mat.SetInt("_ZWrite", 0);
                    mat.DisableKeyword("_ALPHATEST_ON");
                    mat.EnableKeyword("_ALPHABLEND_ON");
                    mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    mat.renderQueue = 3000;
                }
                color.a = 0.4f;
                mat.color = color;
                rend.material = mat;
            }

            DemoTrigger dt = triggerGo.AddComponent<DemoTrigger>();
            dt.type = type;
            dt.amount = 0.2f;
        }

        void EditorSetup()
        {
            if (Application.isPlaying) return;

            SetupPlayerPhysics();
            SetupPlayerCamera();
            SetupMapColorChanger();
            SetupSimplifiedElements();
            SetupMobileControlsInEditor();
            FixRenderPipelineShaders();
        }

        public void FixRenderPipelineShaders()
        {
            bool isURP = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline != null;
            string targetShaderName = isURP ? "Universal Render Pipeline/Lit" : "Standard";
            Shader targetShader = Shader.Find(targetShaderName);
            if (targetShader == null && isURP)
            {
                targetShader = Shader.Find("Universal Render Pipeline/Simple Lit");
            }
            if (targetShader == null)
            {
                targetShader = Shader.Find("Standard");
            }

            if (targetShader != null)
            {
                var renderers = FindObjectsByType<Renderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var r in renderers)
                {
                    
                    if (r != null && r.gameObject.layer == 5)
                    {
                        continue;
                    }

                    if (r != null && r.sharedMaterial != null)
                    {
                        var shader = r.sharedMaterial.shader;
                        bool needsFix = shader == null || 
                                        shader.name == "Hidden/InternalErrorShader" || 
                                        shader.name.Contains("ErrorShader") || 
                                        (isURP && shader.name == "Standard");

                        if (needsFix)
                        {
                            Color oldColor = Color.white;
                            Texture oldMainTex = null;

                            try
                            {
                                if (r.sharedMaterial.HasProperty("_Color"))
                                    oldColor = r.sharedMaterial.color;
                                else if (r.sharedMaterial.HasProperty("_BaseColor"))
                                    oldColor = r.sharedMaterial.GetColor("_BaseColor");

                                if (r.sharedMaterial.HasProperty("_MainTex"))
                                    oldMainTex = r.sharedMaterial.mainTexture;
                                else if (r.sharedMaterial.HasProperty("_BaseMap"))
                                    oldMainTex = r.sharedMaterial.GetTexture("_BaseMap");
                            }
                            catch {}

                            Material newMat = new Material(targetShader);
                            newMat.name = r.sharedMaterial.name + "_Fixed";
                            
                            if (newMat.HasProperty("_Color"))
                                newMat.color = oldColor;
                            else if (newMat.HasProperty("_BaseColor"))
                                newMat.SetColor("_BaseColor", oldColor);

                            if (oldMainTex != null)
                            {
                                if (newMat.HasProperty("_MainTex"))
                                    newMat.mainTexture = oldMainTex;
                                else if (newMat.HasProperty("_BaseMap"))
                                    newMat.SetTexture("_BaseMap", oldMainTex);
                            }

                            if (Application.isPlaying)
                            {
                                r.material = newMat;
                            }
                            else
                            {
                                r.sharedMaterial = newMat;
#if UNITY_EDITOR
                                UnityEditor.EditorUtility.SetDirty(r);
#endif
                            }
                        }
                    }
                }
            }
        }

        private void SubscribeToConfigEvents()
        {
            UnsubscribeFromConfigEvents();
            if (ModularThemeManager.HasInstance && ModularThemeManager.Instance.Config != null)
            {
                ModularThemeManager.Instance.Config.OnConfigurationChanged += HandleConfigChangedInEditor;
            }
        }

        private void UnsubscribeFromConfigEvents()
        {
            if (ModularThemeManager.HasInstance && ModularThemeManager.Instance.Config != null)
            {
                ModularThemeManager.Instance.Config.OnConfigurationChanged -= HandleConfigChangedInEditor;
            }
        }

        private void HandleConfigChangedInEditor()
        {
            if (this == null) return;
            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.delayCall -= EditorSetup;
                UnityEditor.EditorApplication.delayCall += EditorSetup;
#endif
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            
            SubscribeToConfigEvents();
            
            UnityEditor.EditorApplication.delayCall -= EditorSetup;
            UnityEditor.EditorApplication.delayCall += EditorSetup;
        }
#endif
    }
}
