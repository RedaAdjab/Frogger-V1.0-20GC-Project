using UnityEngine;

public class LogController : MonoBehaviour, IHasPlatform
{
    [SerializeField] private bool isRightToLeft = true;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float width = 4f;
    private float xBound = 16f;
    private PlayerController playerController = null;

    protected void Update()
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
        if (playerController != null)
        {
            playerController.TeleportToParent(transform.position); // Teleport player to the new position
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
        playerController = null; // Clear the reference when the player leaves the log
    }
}
