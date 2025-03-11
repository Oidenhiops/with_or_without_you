using System;
using UnityEngine;

public class ManagementPlayerMovement : MonoBehaviour, Character.ICharacterMove
{
    Character character;
    Rigidbody rb;
    Vector3 camForward;
    Vector3 camRight;
    Vector3 movementDirection;
    bool isGrounded = false;
    float jumpForce = 3;
    Vector3 offsetCheckIsGrounded = new Vector3{y = 0.25f};
    Vector3 sizeCheckIsGrounded = new Vector3(0.25f, 0.1f, 0.25f);
    float rayDistance = 0.25f;
    void Start()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody>();
    }
    public void Move()
    {
        isGrounded = CheckIsGrounded();
        if (!character.characterInfo.isPushed)
        {
            Vector3 inputs = new Vector3(character.characterInputs.characterActionsInfo.movement.x, 0, character.characterInputs.characterActionsInfo.movement.y).normalized;
            CamDirection();
            Vector3 camDirection = (inputs.x * camRight + inputs.z * camForward).normalized;
            Jump();
            movementDirection = new Vector3(camDirection.x * character.characterInfo.GetStatisticByType(Character.TypeStatistics.Spd).currentValue, rb.linearVelocity.y, camDirection.z * character.characterInfo.GetStatisticByType(Character.TypeStatistics.Spd).currentValue);

            rb.linearVelocity = movementDirection;
        }
        else
        {
            if (character.characterInfo.characterAnimations.GetCurrentAnimation().typeAnimation != ManagementCharacterAnimations.TypeAnimation.TakeDamage)
            {
                character.characterInfo.isPushed = false;
            }
        }
    }
    void CamDirection()
    {
        Vector3 camForwardDirection = Camera.main.transform.forward;
        Vector3 camRightDirection = Camera.main.transform.right;

        camForwardDirection.y = 0;
        camRightDirection.y = 0;

        camForward = camForwardDirection.normalized;
        camRight = camRightDirection.normalized;
    }
    bool CheckIsGrounded()
    {
        return Physics.BoxCast(transform.position + offsetCheckIsGrounded, sizeCheckIsGrounded, Vector3.down, out _, Quaternion.identity, rayDistance, LayerMask.GetMask("Map"));
    }
    void Jump()
    {
        if (isGrounded && character.characterInputs.characterActionsInfo.jump.triggered)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    public Rigidbody GetRigidbody()
    {
        return rb;
    }
    void OnDrawGizmos()
    {
        Vector3 center = transform.position + offsetCheckIsGrounded; 
        RaycastHit hit;
        if (Physics.BoxCast(transform.position + offsetCheckIsGrounded, sizeCheckIsGrounded, Vector3.down, out hit, Quaternion.identity, rayDistance, LayerMask.GetMask("Map")))
        {   
            Vector3 hitPos = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(center, hitPos);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(hitPos, sizeCheckIsGrounded);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(center, center + Vector3.down * rayDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center + Vector3.down * rayDistance, sizeCheckIsGrounded);
        }
    }
}
