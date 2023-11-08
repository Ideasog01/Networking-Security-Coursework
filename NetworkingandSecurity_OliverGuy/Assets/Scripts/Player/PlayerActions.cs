using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActions : MonoBehaviour
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

    [Header("Ability 3")]

    [SerializeField] private float ability3ProjectileSpeed;
    [SerializeField] private Transform ability3ProjectilePrefab;

    [Header("Ability Display")]
    [SerializeField] private Slider[] abilitySliderArray;
    [SerializeField] private TextMeshProUGUI[] abilityTextArray;

    private float[] _abilityCooldowns;

    private PlayerMovement _playerMovement;
    private HealthController _playerHealthController;
    private Animator _playerAnimator;

    private bool _isPlayerDisabled;

    private void Awake()
    {
        _playerMovement = this.GetComponent<PlayerMovement>();
        _playerHealthController = this.GetComponent<HealthController>();
        _playerAnimator = this.transform.GetChild(0).GetComponent<Animator>();
        _abilityCooldowns = new float[4];
    }

    private void Update()
    {
        if(_isPlayerDisabled)
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

        if (_abilityCooldowns[3] <= 0 && Input.GetKeyDown(KeyCode.R) && !_playerMovement.StopMovement)
        {
            PlayAnimation("Ability3");
            _abilityCooldowns[3] = abilityCooldownDurations[3];
        }

        _playerMovement.UpdateRotation();
    }

    private void FixedUpdate()
    {
        _playerMovement.UpdateMovement();
    }

    private void PlayAnimation(string trigger)
    {
        _playerAnimator.SetTrigger(trigger);
        _playerMovement.StopMovement = true;
    }

    public void FireBullet()
    {
        Rigidbody projectileRb = Instantiate(primaryProjectilePrefab.GetComponent<Rigidbody>(), projectileSpawn.transform.position, this.transform.rotation);

        projectileRb.transform.forward = this.transform.forward;
        projectileRb.velocity = projectileRb.transform.forward * primaryProjectileSpeed;

        projectileRb.GetComponent<CollisionController>().OwnerCollider = this.GetComponent<Collider>();

        VisualEffectManager.SpawnVisualEffect(primaryVisualEffectPrefab, projectileSpawn.position, this.transform.rotation, 5);

        StartCoroutine(AbilityCooldown(0));
    }

    public void LightBlast()
    {
        Rigidbody projectileRb = Instantiate(ability1ProjectilePrefab.GetComponent<Rigidbody>(), projectileSpawn.transform.position, this.transform.rotation);

        projectileRb.transform.forward = this.transform.forward;
        projectileRb.velocity = projectileRb.transform.forward * ability1ProjectileSpeed;

        projectileRb.GetComponent<CollisionController>().OwnerCollider = this.GetComponent<Collider>();

        StartCoroutine(AbilityCooldown(1));
    }

    public void Shield()
    {
        _playerHealthController.IsInvulnerable = true;
        shieldVisualEffect.Play();
        StartCoroutine(CancelShield());
    }

    public void TimeBlast()
    {
        Rigidbody projectileRb = Instantiate(ability3ProjectilePrefab.GetComponent<Rigidbody>(), projectileSpawn.transform.position, this.transform.rotation);

        projectileRb.transform.forward = this.transform.forward;
        projectileRb.velocity = projectileRb.transform.forward * ability3ProjectileSpeed;

        projectileRb.GetComponent<CollisionController>().OwnerCollider = this.GetComponent<Collider>();

        VisualEffectManager.SpawnVisualEffect(primaryVisualEffectPrefab, projectileSpawn.position, this.transform.rotation, 5);

        StartCoroutine(AbilityCooldown(3));
    }

    public void DisablePlayer(bool disable)
    {
        _playerMovement.StopMovement = disable;
        _isPlayerDisabled = disable;
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
}
