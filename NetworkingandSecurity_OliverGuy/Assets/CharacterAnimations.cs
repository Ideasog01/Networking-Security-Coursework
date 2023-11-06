using UnityEngine;

public class CharacterAnimations : MonoBehaviour
{
    [SerializeField] private Animator characterAnimator;

    private Rigidbody _characterRb;

    private void Awake()
    {
        _characterRb = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Debug.Log(_characterRb.velocity);
        characterAnimator.SetFloat("Horizontal", _characterRb.velocity.x);
        characterAnimator.SetFloat("Vertical", _characterRb.velocity.z);
    }

    public void RotateCharacter(float axis)
    {
        bool isTurning = axis != 0 && _characterRb.velocity.x == 0 && _characterRb.velocity.z == 0;

        characterAnimator.SetBool("IsTurning", isTurning);
        characterAnimator.SetFloat("TurnSpeed", axis);
    }
}
