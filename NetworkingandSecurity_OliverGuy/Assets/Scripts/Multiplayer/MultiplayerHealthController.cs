using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerHealthController : MonoBehaviour, IPunObservable
{
    [Header("Statistics")]

    [SerializeField] private int maxHealth;
    [SerializeField] private bool isEnemy;
    [SerializeField] private Slider healthSlider;

    private int _currentHealth;

    private bool _isInvulnerable;

    private PhotonView _photonView;
    private Animator _characterAnimator;

    public bool IsInvulnerable
    {
        get { return _isInvulnerable; }
        set { _isInvulnerable = value; }
    }

    public int CurrentHealth
    {
        get { return _currentHealth; }
    }

    private void Start()
    {
        _photonView = this.GetComponent<PhotonView>();
        _characterAnimator = this.transform.GetChild(0).GetComponent<Animator>();

        if (_photonView.IsMine)
        {
            healthSlider = MultiplayerLevelManager.PlayerHealthSlider; //Assign to HUD Slider
        }

        _currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = _currentHealth;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentHealth);
        }
        else if(stream.IsReading)
        {
            _currentHealth = (int)stream.ReceiveNext();
            healthSlider.value = _currentHealth;
        }
    }

    public void TakeDamage(MultiplayerCollisionController collision)
    {
        if(!_isInvulnerable && _currentHealth > 0)
        {
            _currentHealth -= collision.CollisionDamage;

            Debug.Log("Entity took damage: " + collision.Owner.NickName + "\nHealth: " + _currentHealth.ToString());

            if(_photonView.IsMine)
            {
                healthSlider.value = _currentHealth;
            }

            if (_currentHealth <= 0)
            {
                if(!_photonView.IsMine)
                {
                    collision.Owner.AddScore(1);
                }
                
                ControllerDeath(collision);
            }
        }
    }

    public void ControllerDeath(MultiplayerCollisionController collision)
    {
        if (_photonView.IsMine)
        {
            _characterAnimator.SetBool("isDead", true);
            FindFirstObjectByType<MultiplayerLevelManager>().PlayerDeath(PhotonNetwork.LocalPlayer, collision.Owner, collision.OwnerCollider.gameObject);
        }
    }

    public void ResetPlayer()
    {
        _characterAnimator.SetBool("isDead", false);
        _currentHealth = maxHealth;
    }
}
