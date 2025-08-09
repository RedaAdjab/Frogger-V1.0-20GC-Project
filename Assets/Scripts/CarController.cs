using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private bool isRightToLeft = true;
    [SerializeField] private float speed = 5f;
    private float xBound = 12f;

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
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player.IsDead()) return; // Don't interact with dead player
            
            player.PlayOnDeathParticles();
            player.StartCoroutine(player.ResetPositionWithDelay(gameObject, 1.0f));
            Debug.Log("Player hit by car, resetting position.");
        }
    }
}
