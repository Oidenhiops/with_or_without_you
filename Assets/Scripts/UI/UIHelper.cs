using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHelper : MonoBehaviour
{
    public GameObject[] objectsForActive;
    public TypeUiHelper typeUiHelper;
    public void PlayASound(AudioClip audioClip)
    {
        GameObject blockSound = Instantiate(Resources.Load<GameObject>("Prefabs/Effects/BlockSound/BlockSound"));
        blockSound.GetComponent<ManagementBlockSound>().PlaySound(audioClip, false);
        Destroy(blockSound, audioClip.length);
    }
    public void FadeOutVolume()
    {
        StartCoroutine(FadeOut());
    }
    public void FadeInVolume()
    {
        StartCoroutine(FadeIn());
    }
    public IEnumerator FadeOut()
    {
        float decibelsMaster = 0;
        float currentValue = ManagementData.saveData.configurationsInfo.soundConfiguration.MASTERValue;
        while (currentValue > 0)
        {
            currentValue--;
            decibelsMaster = 20 * Mathf.Log10(currentValue / 100);
            ManagementData.audioMixer.SetFloat(ManagementOptions.TypeSound.Master.ToString(), decibelsMaster);
            yield return new WaitForSeconds(0.01f);
        }
    }
    public IEnumerator FadeIn()
    {
        float decibelsMaster = 0;
        float masterValue = ManagementData.saveData.configurationsInfo.soundConfiguration.MASTERValue;
        float currentValue = 0;
        while (currentValue < masterValue)
        {
            currentValue++;
            decibelsMaster = 20 * Mathf.Log10(currentValue / 100);
            ManagementData.audioMixer.SetFloat(ManagementOptions.TypeSound.Master.ToString(), decibelsMaster);
            yield return new WaitForSeconds(0.01f);
        }
    }
    public void DischargeSceneOptions()
    {
        var uiHelpers = FindObjectsByType<UIHelper>(FindObjectsSortMode.None);
        foreach (var uiHelper in uiHelpers)
        {
            if (uiHelper.typeUiHelper == TypeUiHelper.Options)
            {
                foreach (var objects in uiHelper.objectsForActive)
                {
                    objects.SetActive(true);
                }
                break;
            }
        }
        SceneManager.UnloadSceneAsync("OptionsScene");
    }
    public void ChargeSceneOptions()
    {
        foreach (var objects in objectsForActive)
        {
            objects.SetActive(false);
        }
        SceneManager.LoadScene("OptionsScene", LoadSceneMode.Additive);
    }
    public enum TypeUiHelper
    {
        None = 0,
        Options = 1
    }
}
