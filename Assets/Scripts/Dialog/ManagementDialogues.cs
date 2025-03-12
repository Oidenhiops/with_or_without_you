using System;
using System.Collections;
using System.Collections.Generic;
using Febucci.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagementDialogues : MonoBehaviour
{
    public GameManagerHelper gameManagerHelper;
    public DialogComponents dialogComponents;
    public TypewriterByCharacter typewriterByCharacter;
    public List<DialogInfo> dialogInfo;
    public int currentDialogIndex = -1;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }
    [NaughtyAttributes.Button]  public void NextLine()
    {
        currentDialogIndex++;
        if (currentDialogIndex > dialogInfo.Count - 1){
            gameManagerHelper.ChangeScene(4);
            return;
        }
        else
        {
            dialogComponents.characterDialog.text = dialogInfo[currentDialogIndex].characterDialog;
            dialogComponents.characterName.text = dialogInfo[currentDialogIndex].characterName;
            if (dialogInfo[currentDialogIndex].spriteCharacterLeft == null)
            {
                dialogComponents.characterLeft.enabled = false;
            }
            else {
                dialogComponents.characterLeft.sprite = dialogInfo[currentDialogIndex].spriteCharacterLeft;
                dialogComponents.characterLeft.enabled = true;
            }
            if (dialogInfo[currentDialogIndex].spriteCharacterRigth == null)
            {
                dialogComponents.characterRigth.enabled = false;
            }
            else
            {
                dialogComponents.characterRigth.sprite = dialogInfo[currentDialogIndex].spriteCharacterRigth;
                dialogComponents.characterRigth.enabled = true;
            }
            if (dialogInfo[currentDialogIndex].changeCharacterPositionLeft != null)
            {
                dialogInfo[currentDialogIndex].characterLeft.characterInfo.characterMove.SetPositionTarget(dialogInfo[currentDialogIndex].changeCharacterPositionLeft);
                dialogInfo[currentDialogIndex].characterLeft.characterInfo.characterMove.SetCanMoveState(true);
            }
            if (dialogInfo[currentDialogIndex].changeCharacterPositionRigth != null)
            {
                dialogInfo[currentDialogIndex].characterRigth.characterInfo.characterMove.SetPositionTarget(dialogInfo[currentDialogIndex].changeCharacterPositionRigth);
                dialogInfo[currentDialogIndex].characterRigth.characterInfo.characterMove.SetCanMoveState(true);
            }
            if (dialogInfo[currentDialogIndex].bg != null)
            {
                gameManagerHelper.ChangeBGMusic(dialogInfo[currentDialogIndex].bg);
            }
        }
    }
    [Serializable]   public class DialogInfo
    {
        public string characterName;
        public string characterDialog;
        public Sprite spriteCharacterLeft;
        public Character characterLeft;
        public Transform changeCharacterPositionLeft;
        public Sprite spriteCharacterRigth;
        public Character characterRigth;
        public Transform changeCharacterPositionRigth;
        public AudioClip bg;
    }
    [Serializable]  public class DialogComponents
    {
        public TMP_Text characterDialog;
        public TMP_Text characterName;
        public Image characterLeft;
        public Image characterRigth;
    }
}
