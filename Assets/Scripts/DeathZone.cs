using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player.IsDead()) return; // Don't interact with dead player
            
            player.PlayOnDeathParticles();
            player.StartCoroutine(player.ResetPositionWithDelay(gameObject, 1.0f));
        }
    }
}
