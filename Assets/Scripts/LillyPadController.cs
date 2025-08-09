using UnityEngine;

public class LillyPadController : MonoBehaviour
{
    [SerializeField] protected GameObject lillyPad;
    [SerializeField] private GameObject lillyPadFlower;

    private bool isActive = false;

    private void Start()
    {
        lillyPad.SetActive(true);
        lillyPadFlower.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player.IsDead()) return; // Don't interact with dead player
            
            if (isActive)
            {
                player.PlayOnDeathParticles();
                player.StartCoroutine(player.ResetPositionWithDelay(gameObject, 1.0f));
                return;
            }
            player.PlayOnLilyPadParticles();
            player.ResetPositionWithoutLifeLoss();
            GameManager.Instance.AddLillyPad();
            lillyPadFlower.SetActive(true);
            lillyPad.SetActive(false);
            isActive = true;
        }
    }
}
