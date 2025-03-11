using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagementOpenCloseScene : MonoBehaviour
{
    public GameManager gameManager;
    public Animator openCloseSceneAnimator;
    [SerializeField] Image openCloseSceneLoader;
    bool finishLoad = false;
    float currentLoad = 0;
    public bool auto = false;
    bool CantCharge = false;
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        if (auto) StartCoroutine(AutoCharge());
    }
    public void Update()
    {
        if (!finishLoad && !CantCharge)
        {
            float value = currentLoad / 100 > 0 ? currentLoad / 100 : 1;
            openCloseSceneLoader.fillAmount = Mathf.Lerp(openCloseSceneLoader.fillAmount, currentLoad / 100, value * Time.deltaTime);
            if (openCloseSceneLoader.fillAmount >= 0.99)
            {
                finishLoad = true;
                CantCharge = true;
                gameManager.EnterScene();
                FinishLoad();
            }
        }
    }
    public IEnumerator AutoCharge()
    {
        while (true)
        {
            if (currentLoad >= 100)
            {
                break;
            }
            currentLoad += 50;
            yield return new WaitForSeconds(2);
        }
    }
    public void AdjustLoading(float amount)
    {
        currentLoad += amount;
    }
    public void ResetParams()
    {
        finishLoad = false;
        currentLoad = 0;
        openCloseSceneLoader.fillAmount = 0;
    }
    public void StartCharge()
    {
        CantCharge = false;
        StartCoroutine(AutoCharge());
    }
    public void FinishLoad()
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Character character = FindAnyObjectByType<Character>();
            character.characterInfo.isActive = true;
            character.GetComponent<Rigidbody>().isKinematic = false;
            break;
        }
    }
    public enum TypeScene
    {
        MenuScene = 0,
        GameScene = 1
    }
}
