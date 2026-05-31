using UnityEngine;
using System.Collections;

namespace ModularUIRuntime.Demo
{
    public class DemoPickup : MonoBehaviour
    {
        public enum PickupEffect { None, HealPlayer, RestoreMana, DamagePlayer }

        public ItemData itemData;
        public int quantity = 1;
        public PickupEffect pickupEffect = PickupEffect.None;
        [Range(0f, 1f)] public float effectValue = 0.25f;
        public float bobAmplitude = 0.25f;
        public float bobSpeed = 2f;
        public float rotationSpeed = 90f;
        public ParticleSystem collectParticles;
        public AudioClip collectSound;

        [HideInInspector] public bool isCollected = false;
        Vector3 startPos;
        float bobTimer;

        void Start()
        {
            startPos = transform.position;
            bobTimer = Random.Range(0f, Mathf.PI * 2f);
        }

        void Update()
        {
            if (isCollected) return;
            bobTimer += Time.deltaTime;
            Vector3 pos = startPos;
            pos.y += Mathf.Sin(bobTimer * bobSpeed) * bobAmplitude;
            transform.position = pos;
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }

        public void Collect()
        {
            isCollected = true;
            if (collectParticles != null)
            {
                collectParticles.transform.SetParent(null);
                collectParticles.Play();
                Destroy(collectParticles.gameObject, collectParticles.main.duration + 1f);
            }
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            StartCoroutine(CollectAnim());
        }

        IEnumerator CollectAnim()
        {
            float t = 0f;
            Vector3 s = transform.localScale;
            while (t < 0.3f)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(s, Vector3.zero, t / 0.3f);
                transform.position += Vector3.up * Time.deltaTime * 3f;
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}
