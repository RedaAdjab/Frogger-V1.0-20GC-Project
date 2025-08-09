using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] private bool immortal = false;

    // Particles
    [SerializeField] private ParticleSystem onDeathParticles;
    [SerializeField] private ParticleSystem onDrawnParticles;
    [SerializeField] private ParticleSystem onLilyPadParticles;

    // Sound Effects
    [SerializeField] private AudioClip onDeathSound;
    [SerializeField] private AudioClip onDrawnSound;
    [SerializeField] private AudioClip onLilyPadSound;
    [SerializeField] private AudioSource audioSource;

    // Player state
    private bool isDead = false;

    private float tile = 1f;
    private float moveTimer = 0.075f;
    private float timer = 0f;
    private Vector3 direction = Vector3.zero;
    private Vector3 newPosition = Vector3.zero;
    private float xBound = 8.5f;
    private float yBound = 6.5f;
    private Vector2 yRiver = new Vector2(0.5f, 4.5f);
    private int highestYSteps = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetPositionWithoutLifeLoss();
        
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource not found, adding one to PlayerController.");
        }
        
        audioSource.spatialBlend = 0f; // 2D audio
        audioSource.volume = 1f;
        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGamePlaying() || isDead)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer > moveTimer)
        {
            direction = Vector3.zero;

            if (Input.GetKeyDown(KeyCode.W)) direction = Vector3.up;
            else if (Input.GetKeyDown(KeyCode.S)) direction = Vector3.down;
            else if (Input.GetKeyDown(KeyCode.A)) direction = Vector3.left;
            else if (Input.GetKeyDown(KeyCode.D)) direction = Vector3.right;

            newPosition = transform.position + direction * tile;

            if (direction != Vector3.zero && !IsOutOfBounds(newPosition))
            {
                transform.SetPositionAndRotation(newPosition, Quaternion.LookRotation(Vector3.forward, direction));
                float yOffset = -6.5f;
                if (newPosition.y - yOffset > highestYSteps)
                {
                    highestYSteps = (int)(newPosition.y - yOffset);
                    GameManager.Instance.AddScore(1);
                }
                StartCoroutine(RiverHandler(newPosition));
                timer = 0f;
            }
        }
    }

    private bool IsOutOfBounds(Vector3 position)
    {
        return position.x < -xBound || position.x > xBound || position.y < -yBound || position.y > yBound;
    }
    private bool IsInRiver(Vector3 position)
    {
        return position.y >= yRiver.x && position.y <= yRiver.y;
    }

    private bool IsOutOfPlatform(Vector3 position)
    {
        if (!HasParent()) return true;

        if (!GetParent().TryGetComponent<IHasPlatform>(out IHasPlatform platform))
        {
            Debug.LogError("Parent object does not implement IHasPlatform interface.");
            return true;
        }

        float halfWidth = platform.GetWidth() / 2f;
        float errorMargin = 0f;
        float xErrorMargin = 0.2f;

        Transform platformTransform = platform.GetTransform();
        float relativeX = position.x - platformTransform.position.x;
        float relativeY = Mathf.Abs(position.y - platformTransform.position.y);

        return relativeX < -halfWidth - xErrorMargin || relativeX > halfWidth + xErrorMargin || relativeY > errorMargin;
    }

    IEnumerator RiverHandler(Vector3 newPosition)
    {
        yield return new WaitForFixedUpdate();

        if (HasParent())
        {
            if (IsOutOfPlatform(newPosition))
            {
                RemoveParent();
                if (IsInRiver(newPosition))
                {
                    PlayOnDrawnParticles();
                    StartCoroutine(ResetPositionWithDelay(gameObject, 1.0f));
                }
            }
        }
        else
        {
            if (IsInRiver(newPosition))
            {
                PlayOnDrawnParticles();
                StartCoroutine(ResetPositionWithDelay(gameObject, 1.0f));
            }
        }
    }

    public IEnumerator ResetPositionWithDelay(GameObject causeObject, float delay)
    {
        // Set player as dead to prevent all interactions
        isDead = true;
        RemoveParent();
        
        yield return new WaitForSeconds(delay);
        
        if (!immortal)
        {
            timer = 0f;
            highestYSteps = 0;
            transform.position = new Vector3(0.5f, -6.5f, -2f);
            transform.localScale = Vector3.one;
            Debug.Log("Player reset position due to " + causeObject.name);
            GameManager.Instance.RemoveLife();
        }
        
        // Revive player and allow interactions again
        isDead = false;
    }

    public void ResetPosition(GameObject gameObject)
    {
        if (!immortal)
        {
            RemoveParent();
            timer = 0f;
            highestYSteps = 0;
            transform.position = new Vector3(0.5f, -6.5f, -2f);
            Debug.Log("Player reset position due to " + gameObject.name);
            GameManager.Instance.RemoveLife();
        }
    }

    public void ResetPositionWithoutLifeLoss()
    {
        transform.position = new Vector3(0.5f, -6.5f, -2f);
    }

    public void SetParent(Transform parent)
    {
        if (isDead) return; // Prevent parent changes when dead
        
        if (HasParent() && GetParent().TryGetComponent<IHasPlatform>(out IHasPlatform platform))
        {
            platform.ClearPlayerController();
        }
        transform.SetParent(parent, true);
        if (parent.TryGetComponent<IHasPlatform>(out platform))
        {
            platform.SetPlayerController(this);
        }
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool HasParent()
    {
        return transform.parent != null;
    }

    public GameObject GetParent()
    {
        if (transform.parent != null)
        {
            return transform.parent.gameObject;
        }
        return null;
    }

    public void RemoveParent()
    {
        if (HasParent() && GetParent().TryGetComponent<IHasPlatform>(out IHasPlatform platform))
        {
            platform.ClearPlayerController();
        }
        transform.SetParent(null);
    }

    public void TeleportToParent(Vector3 targetPosition)
    {
        if (HasParent())
        {
            if (!GetParent().TryGetComponent<IHasPlatform>(out IHasPlatform platform))
            {
                Debug.LogError("Parent object does not implement IHasPlatform interface.");
                return;
            }
            Vector3 position = transform.position;
            float relativeX = position.x - targetPosition.x;
            transform.position = targetPosition + new Vector3(relativeX, 0f, -2f);
        }
    }

    public void PlayOnDeathParticles()
    {
        if (onDeathParticles != null)
        {
            ParticleSystem particles = Instantiate(onDeathParticles, transform.position, Quaternion.identity);
            particles.Play();
        }
        
        // Play death sound immediately and at full volume
        if (onDeathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(onDeathSound);
        }
    }

    public void PlayOnDrawnParticles()
    {
        if (onDrawnParticles != null)
        {
            ParticleSystem particles = Instantiate(onDrawnParticles, transform.position, Quaternion.identity);
            particles.Play();
        }
        
        // Play drowned sound immediately and at full volume
        if (onDrawnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(onDrawnSound);
        }
    }

    public void PlayOnLilyPadParticles()
    {
        if (onLilyPadParticles != null)
        {
            ParticleSystem particles = Instantiate(onLilyPadParticles, transform.position, Quaternion.identity);
            particles.Play();
        }
        
        // Play lily pad sound immediately and at full volume
        if (onLilyPadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(onLilyPadSound);
        }
    }

}

