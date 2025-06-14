using UnityEngine;
using System.Collections;

public class IceStick : MonoBehaviour
{
    private bool hasPlayedSound = false;
    [SerializeField] private AudioClip attachSound;
    [SerializeField] private AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tongue"))
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (!hasPlayedSound && attachSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(attachSound);
                hasPlayedSound = true;
                StartCoroutine(StopSoundAfterDelay(3f));
            }
            if (rb != null && rb.bodyType == RigidbodyType2D.Static)
            {
                StartCoroutine(ChangeToDynamicAfterDelay(rb, 3f)); // 3초 후에 Dynamic으로 변경
            }
        }
    }

    private IEnumerator ChangeToDynamicAfterDelay(Rigidbody2D rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null && rb.bodyType == RigidbodyType2D.Static)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private IEnumerator StopSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}