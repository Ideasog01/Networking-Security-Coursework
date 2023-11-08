using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Multiplayer : MonoBehaviour
{
    [Header("Ability Settings")]

    [SerializeField] private float[] abilityCooldownDurations;
    [SerializeField] private Transform projectileSpawn;

    [Header("Primary Ability")]

    [SerializeField] private float primaryProjectileSpeed;
    [SerializeField] private Transform primaryProjectilePrefab;
    [SerializeField] private GameObject primaryVisualEffectPrefab;

    [Header("Ability 1")]

    [SerializeField] private float ability1ProjectileSpeed;
    [SerializeField] private Transform ability1ProjectilePrefab;

    [Header("Ability 2")]

    [SerializeField] private ParticleSystem shieldVisualEffect;

    [Header("Ability Display")]
    [SerializeField] private Slider[] abilitySliderArray;
    [SerializeField] private TextMeshProUGUI[] abilityTextArray;

    private float[] _abilityCooldowns = new float[3];

    private PlayerMovement _playerMovement;
    private MultiplayerHealthController _playerHealthController;
    private Animator _playerAnimator;

    private bool _isPlayerDisabled;

    private PhotonView _photonView;

    private void Awake()
    {
        _photonView = this.GetComponent<PhotonView>();
        _playerMovement = this.GetComponent<PlayerMovement>();
        _playerAnimator = this.transform.GetChild(0).GetComponent<Animator>();
        _playerHealthController = this.GetComponent<MultiplayerHealthController>();

        abilitySliderArray = MultiplayerLevelManager.AbilitySliderArray;
        abilityTextArray = MultiplayerLevelManager.AbilityTextArray;

        if(!_photonView.IsMine)
        {
            this.transform.parent.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            this.transform.GetChild(3).gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(!_photonView.IsMine)
        {
            return;
        }

        if (_isPlayerDisabled)
        {
            return;
        }

        if (_abilityCooldowns[0] <= 0 && Input.GetMouseButtonDown(0) && !_playerMovement.StopMovement)
        {
            PlayAnimation("Primary");
            _abilityCooldowns[0] = abilityCooldownDurations[0];
        }

        if (_abilityCooldowns[1] <= 0 && Input.GetKeyDown(KeyCode.Q) && !_playerMovement.StopMovement)
        {
            PlayAnimation("Ability1");
            _abilityCooldowns[1] = abilityCooldownDurations[1];
        }

        if (_abilityCooldowns[2] <= 0 && Input.GetKeyDown(KeyCode.E) && !_playerMovement.StopMovement)
        {
            PlayAnimation("Ability2");
            _abilityCooldowns[2] = abilityCooldownDurations[2];
        }

        _playerMovement.UpdateRotation();
    }

    private void FixedUpdate()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        _playerMovement.UpdateMovement();
    }

    private void PlayAnimation(string trigger)
    {
        _playerAnimator.SetTrigger(trigger);
        _playerMovement.StopMovement = true;
    }

    public void ActivateAbility(string functionName)
    {
        //_photonView.RPC(functionName, RpcTarget.AllViaServer);
        Invoke(functionName, 0);
    }

    private void PrimaryAttack()
    {
        Rigidbody projectileRb = Instantiate(primaryProjectilePrefab.GetComponent<Rigidbody>(), projectileSpawn.transform.position, this.transform.rotation);

        projectileRb.transform.forward = this.transform.forward;
        projectileRb.velocity = projectileRb.transform.forward * primaryProjectileSpeed;

        projectileRb.GetComponent<MultiplayerCollisionController>().Owner = _photonView.Owner;
        projectileRb.GetComponent<MultiplayerCollisionController>().OwnerCollider = this.GetComponent<Collider>();

        _abilityCooldowns[0] = abilityCooldownDurations[0];
        StartCoroutine(AbilityCooldown(0));
    }

    private void LightBlast()
    {
        Rigidbody projectileRb = Instantiate(ability1ProjectilePrefab.GetComponent<Rigidbody>(), projectileSpawn.transform.position, this.transform.rotation);

        projectileRb.transform.forward = this.transform.forward;
        projectileRb.velocity = projectileRb.transform.forward * ability1ProjectileSpeed;

        projectileRb.GetComponent<MultiplayerCollisionController>().Owner = _photonView.Owner;
        projectileRb.GetComponent<MultiplayerCollisionController>().OwnerCollider = this.GetComponent<Collider>();

        StartCoroutine(AbilityCooldown(1));
    }

    private void Shield()
    {
        _playerHealthController.IsInvulnerable = true;
        shieldVisualEffect.Play();
        StartCoroutine(CancelShield());
    }

    private IEnumerator CancelShield()
    {
        yield return new WaitForSeconds(7);
        StartCoroutine(AbilityCooldown(2));
        _playerHealthController.IsInvulnerable = false;
    }

    private IEnumerator AbilityCooldown(int abilityIndex)
    {
        yield return new WaitForSeconds(0.1f);
        _abilityCooldowns[abilityIndex] -= 0.1f;

        abilitySliderArray[abilityIndex].maxValue = abilityCooldownDurations[abilityIndex];
        abilitySliderArray[abilityIndex].value = _abilityCooldowns[abilityIndex];

        if (_abilityCooldowns[abilityIndex] > 0)
        {
            abilityTextArray[abilityIndex].text = _abilityCooldowns[abilityIndex].ToString("F0");
            StartCoroutine(AbilityCooldown(abilityIndex));
        }
        else
        {
            abilityTextArray[abilityIndex].text = "";
        }
    }

    public void DisablePlayer(bool disable)
    {
        _playerMovement.StopMovement = disable;
        _isPlayerDisabled = disable;
    }
}
