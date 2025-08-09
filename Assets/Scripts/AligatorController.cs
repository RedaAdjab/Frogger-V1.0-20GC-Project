using UnityEngine;

public class AligatorController : LogController
{
    [SerializeField] private GameObject deathZone;
    private float timer = 0f;
    private bool isActive = false;
    private float activationDuration = 2f;
    private float activationFrequency = 5f;


    private new void Update()
    {
        if (!GameManager.Instance.IsGamePlaying())
        {
            return;
        }

        base.Update();

        timer += Time.deltaTime;

        if (isActive && timer >= activationDuration)
        {
            deathZone.SetActive(false);
            timer = 0f;
            isActive = false;
        }
        else if (!isActive && timer >= activationFrequency)
        {
            deathZone.SetActive(true);
            timer = 0f;
            isActive = true;
        }
    }
}
