using UnityEngine;
using UnityEngine.UI;

namespace ModularUIRuntime
{
    [RequireComponent(typeof(Image))]
    public class ModularSpriteAnimator : MonoBehaviour
    {
        [SerializeField] private Sprite[] animationFrames;
        [SerializeField] private float framesPerSecond = 12f;
        [SerializeField] private bool playOnAwake = true;
        [SerializeField] private bool loop = true;

        private Image targetImage;
        private float timer;
        private int currentFrameIndex;
        private bool isPlaying;

        private void Awake()
        {
            targetImage = GetComponent<Image>();

            if (playOnAwake)
            {
                Play();
            }
        }

        private void Update()
        {
            if (!isPlaying || animationFrames == null || animationFrames.Length == 0)
            {
                return;
            }

            timer += Time.deltaTime;

            if (timer >= 1f / framesPerSecond)
            {
                timer = 0f;
                currentFrameIndex++;

                if (currentFrameIndex >= animationFrames.Length)
                {
                    if (loop)
                    {
                        currentFrameIndex = 0;
                    }
                    else
                    {
                        Stop();
                        return;
                    }
                }

                targetImage.sprite = animationFrames[currentFrameIndex];
            }
        }

        public void Play()
        {
            isPlaying = true;
        }

        public void Stop()
        {
            isPlaying = false;
        }

        public void ResetAnimation()
        {
            currentFrameIndex = 0;
            timer = 0f;

            if (animationFrames != null && animationFrames.Length > 0)
            {
                targetImage.sprite = animationFrames[0];
            }
        }
    }
}