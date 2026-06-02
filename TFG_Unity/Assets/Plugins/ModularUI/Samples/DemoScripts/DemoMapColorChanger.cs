using UnityEngine;

namespace ModularUIRuntime.Demo
{
    public class DemoMapColorChanger : MonoBehaviour
    {
        public Renderer planeRenderer;
        public float changeInterval = 15f;

        private float timer;
        private Color[] colors = new Color[] 
        { 
            Color.red, 
            Color.green, 
            Color.blue, 
            Color.yellow, 
            Color.magenta, 
            Color.cyan 
        };
        private int currentIndex = 0;

        private void Start()
        {
            if (planeRenderer == null)
            {
                planeRenderer = GetComponent<Renderer>();
            }
        }

        private void Update()
        {
            if (planeRenderer == null) return;

            timer += Time.deltaTime;
            if (timer >= changeInterval)
            {
                timer = 0f;
                currentIndex = (currentIndex + 1) % colors.Length;
                if (Application.isPlaying)
                {
                    planeRenderer.material.color = colors[currentIndex];
                }
                else
                {
                    planeRenderer.sharedMaterial.color = colors[currentIndex];
                }
            }
        }
    }
}
