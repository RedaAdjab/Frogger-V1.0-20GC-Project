using UnityEngine;

public interface IHasPlatform
{
    float GetWidth();
    Transform GetTransform();
    void SetPlayerController(PlayerController playerController);
    PlayerController GetPlayerController();
    void ClearPlayerController();
}
