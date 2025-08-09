using System.Collections;
using UnityEngine;

public class TurtleController : MonoBehaviour, IHasPlatform
{
    [SerializeField] private bool isRightToLeft = true;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float width = 3f;
    [SerializeField] private bool canSubmerge = false;
    private PlayerController playerController = null;
    private float xBound = 15f;
    private bool isSubmerged = false;
    private float timer = 0f;
    private float submergeFreq = 10f;
    private float submergeDuration = 2f;

    private void Update()
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }

        if (isRightToLeft)
        {
            transform.position += speed * Time.deltaTime * Vector3.left;
            if (transform.position.x < -xBound)
            {
                ResetPosition();
            }
        }
            else
        {
            transform.position += speed * Time.deltaTime * Vector3.right;
            if (transform.position.x > xBound)
            {
                ResetPosition();
            }
        }

        if (!canSubmerge || isSubmerged)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer > submergeFreq)
        {
            SubmergeForSeconds(submergeDuration);
            timer = 0f;
        }
    }

    private void ResetPosition()
    {
        if (isRightToLeft)
        {
            transform.position = new Vector3(xBound, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(-xBound, transform.position.y, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isSubmerged)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player.IsDead()) return; // Don't interact with dead player
            
            player.SetParent(transform);
            playerController = player;
        }
    }

    public float GetWidth()
    {
        return width;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    private void SubmergeForSeconds(float seconds)
    {
        StartCoroutine(SubmergeCoroutine(seconds));
    }

    private IEnumerator SubmergeCoroutine(float seconds)
    {
        // Make the turtle semi-transparent
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color color = spriteRenderer.color;
        color.a = 0.5f;
        spriteRenderer.color = color;
        isSubmerged = true;
        if (playerController != null)
        {
            playerController.PlayOnDrawnParticles();
            playerController.StartCoroutine(playerController.ResetPositionWithDelay(gameObject, 1.0f));
            playerController.RemoveParent();
            playerController = null;
        }

        yield return new WaitForSeconds(seconds);

        // Restore the turtle's opacity
        color.a = 1f;
        spriteRenderer.color = color;
        isSubmerged = false;
    }
    
    public void SetPlayerController(PlayerController playerController)
    {
        this.playerController = playerController;
    }
    public PlayerController GetPlayerController()
    {
        return playerController;
    }

    public void ClearPlayerController()
    {
        playerController = null; // Clear the reference when the player leaves the turtle
    }
}
