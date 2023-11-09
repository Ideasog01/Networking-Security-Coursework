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

    public bool IsInvulnerable
    {
        get { return _isInvulnerable; }
        set { _isInvulnerable = value; }
    }

    private void Start()
    {
        _photonView = this.GetComponent<PhotonView>();

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
        if(!_isInvulnerable)
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
                
                ControllerDeath();
            }
        }
    }

    public void ControllerDeath()
    {
        _currentHealth = maxHealth;

        if (_photonView.IsMine)
        {
            healthSlider.value = _currentHealth;
        }
    }
}
