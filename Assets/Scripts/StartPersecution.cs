using UnityEngine;

public class StartPersecution : MonoBehaviour
{
    public Transform player;
    public Character rozaline;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rozaline.characterInfo.characterMove.SetTarget(player);
            rozaline.characterInfo.characterMove.SetCanMoveState(true);
            rozaline.characterInfo.managementCharacterModelDirection.SetTarget(player.gameObject);
            Destroy(gameObject);
        }
    }
}
