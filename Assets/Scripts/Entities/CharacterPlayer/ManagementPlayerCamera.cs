using Cinemachine;
using UnityEngine;

public class ManagementPlayerCamera : MonoBehaviour, Character.ICharacterCamera
{
    [SerializeField] Character character;
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] float speed = 0.01f;
    public void MoveCamera()
    {
        if (character.characterInputs.characterActionsInfo.unlockCamera)
        {
            var orbital = vcam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
            orbital.m_XAxis.Value += character.characterInputs.characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>().x * speed;
        }
    }
}
