using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameManagerHelper gameManagerHelper;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            gameManagerHelper.ChangeScene(4);
            Destroy(gameObject);
        }
    }
}
