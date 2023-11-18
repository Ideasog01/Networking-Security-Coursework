using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Singleplayer
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Ability Settings")]

        [SerializeField] private float[] abilityCooldownDurations; //The length of each ability cooldown in seconds.
        [SerializeField] private Transform projectileSpawn;

        [Header("Primary Ability")]

        [SerializeField] private float primaryProjectileSpeed;
        [SerializeField] private Transform primaryProjectilePrefab;
        [SerializeField] private GameObject primaryVisualEffectPrefab; //The visual effect that displays on primary ability activation.

        [Header("Ability 1")]

        [SerializeField] private float ability1ProjectileSpeed;
        [SerializeField] private Transform ability1ProjectilePrefab;

        [Header("Ability 2")]

        [SerializeField] private ParticleSystem shieldVisualEffect; //The visual effect that displays on shield ability activation.
        [SerializeField] private float shieldDuration = 7;

        [Header("Ability 3")]

        [SerializeField] private int ability3ProjectileSpeed;
        [SerializeField] private Transform ability3ProjectilePrefab;

        [Header("Ability Display")]
        [SerializeField] private Slider[] abilitySliderArray;
        [SerializeField] private TextMeshProUGUI[] abilityTextArray;

        private float[] _abilityCooldowns; //The current cooldowns of all abilities, including primary fire.

        //Player Components
        private PlayerMovement _playerMovement;
        private HealthController _playerHealthController;
        private Animator _playerAnimator;

        private bool _isPlayerDisabled;

        #region Core

        private void Awake()
        {
            _playerMovement = this.GetComponent<PlayerMovement>();
            _playerHealthController = this.GetComponent<HealthController>();
            _playerAnimator = this.transform.GetChild(0).GetComponent<Animator>();
            _abilityCooldowns = new float[4];
        }

        private void Update()
        {
            if (!_isPlayerDisabled)
            {
                PlayerGameplayInput(); //As long as the player is enabled, they can perform abilities (if not on cooldown).
                _playerMovement.UpdateRotation(); //Updates the rotation to follow the mouse cursor.
            }
        }

        private void FixedUpdate()
        {
            _playerMovement.UpdateMovement(); //Update physics on fixed update only
        }

        private void PlayerGameplayInput()
        {
            if (_abilityCooldowns[0] <= 0 && Input.GetMouseButtonDown(0) && !_playerMovement.StopMovement)
            {
                PlayAbilityAnimation("Primary");
                _abilityCooldowns[0] = abilityCooldownDurations[0];
            }

            if (_abilityCooldowns[1] <= 0 && Input.GetKeyDown(KeyCode.Q) && !_playerMovement.StopMovement)
            {
                PlayAbilityAnimation("Ability1");
                _abilityCooldowns[1] = abilityCooldownDurations[1];
            }

            if (_abilityCooldowns[2] <= 0 && Input.GetKeyDown(KeyCode.E) && !_playerMovement.StopMovement)
            {
                PlayAbilityAnimation("Ability2");
                _abilityCooldowns[2] = abilityCooldownDurations[2];
            }

            if (_abilityCooldowns[3] <= 0 && Input.GetKeyDown(KeyCode.R) && !_playerMovement.StopMovement)
            {
                PlayAbilityAnimation("Ability3");
                _abilityCooldowns[3] = abilityCooldownDurations[3];
            }
        }

        #endregion

        #region Ability Functions (Class)

        public void PrimaryAttack()
        {
            //Spawn the projectile and apply a velocity
            Rigidbody projectileRb = Instantiate(primaryProjectilePrefab.GetComponent<Rigidbody>(), projectileSpawn.transform.position, this.transform.rotation);

            projectileRb.transform.forward = this.transform.forward;
            projectileRb.velocity = projectileRb.transform.forward * primaryProjectileSpeed;

            projectileRb.GetComponent<CollisionController>().OwnerCollider = this.GetComponent<Collider>();

            VisualEffectManager.SpawnVisualEffect(primaryVisualEffectPrefab, projectileSpawn.position, this.transform.rotation, 5);

            //Apply ability cooldown based on assigned duration
            _abilityCooldowns[0] = abilityCooldownDurations[0];
            StartCoroutine(AbilityCooldown(0));
        }

        public void LightBlast()
        {
            //Spawn the projectile and apply a velocity
            Rigidbody projectileRb = Instantiate(ability1ProjectilePrefab.GetComponent<Rigidbody>(), projectileSpawn.transform.position, this.transform.rotation);

            projectileRb.transform.forward = this.transform.forward;
            projectileRb.velocity = projectileRb.transform.forward * ability1ProjectileSpeed;

            projectileRb.GetComponent<CollisionController>().OwnerCollider = this.GetComponent<Collider>();

            //Apply ability cooldown based on assigned duration
            _abilityCooldowns[1] = abilityCooldownDurations[1];
            StartCoroutine(AbilityCooldown(1));
        }

        public void Shield() //Makes the player immune to damage for a duration.
        {
            _playerHealthController.IsInvulnerable = true;
            shieldVisualEffect.Play();
            StartCoroutine(CancelShield()); //Note that the shield abilities' cooldown is only started after it ends.
        }

        private IEnumerator CancelShield() //Cancel the player's immunity to damage after a duration. Note that the visual effect duration is assigned inside the inspector on the corresponding particle system component.
        {
            yield return new WaitForSeconds(shieldDuration);
            _abilityCooldowns[1] = abilityCooldownDurations[1];
            StartCoroutine(AbilityCooldown(2)); //Now start the shield ability cooldown.
            _playerHealthController.IsInvulnerable = false;
        }

        public void TimeBlast()
        {
            //Spawn the projectile and create a path that will be followed and randomised nearer to the end.
            FollowPath projectile = Instantiate(ability3ProjectilePrefab.GetComponent<FollowPath>(), projectileSpawn.transform.position, this.transform.rotation);

            projectile.transform.forward = this.transform.forward;
            projectile.GetComponent<CollisionController>().OwnerCollider = this.GetComponent<Collider>();

            List<Vector3> pathList = new List<Vector3>();
            for (int i = 1; i < ability3ProjectileSpeed; i++)
            {
                Vector3 pathPoint = (projectileSpawn.position) + projectileSpawn.forward * i;

                if (i > 5) //Once we have passed five increments, randomise the path.
                {
                    pathPoint = new Vector3(pathPoint.x + Random.Range(-2, 2), pathPoint.y, pathPoint.z + Random.Range(-2, 2));
                }

                //The projectile will follow the path that has been created.
                pathList.Add(pathPoint);
                GameObject debug = new GameObject();
                debug.transform.position = pathList[pathList.Count - 1];
                debug.name = "Debug";
            }

            projectile.PathList = pathList;

            VisualEffectManager.SpawnVisualEffect(primaryVisualEffectPrefab, projectileSpawn.position, this.transform.rotation, 5);

            //Apply ability cooldown based on assigned duration
            _abilityCooldowns[3] = abilityCooldownDurations[3];
            StartCoroutine(AbilityCooldown(3));
        }

        #endregion

        #region Ability Functions (General)

        private IEnumerator AbilityCooldown(int abilityIndex) //Overtime, decrease the ability cooldown at the given index. This is a recursive function and will only stop when the ability cooldown reaches zero.
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

        private void PlayAbilityAnimation(string trigger)
        {
            _playerAnimator.SetTrigger(trigger);
            _playerMovement.StopMovement = true; //When an ability animation is currently active, disable the player's movement.
        }

        #endregion

        public void DisablePlayer(bool disable)
        {
            _playerMovement.StopMovement = disable;
            _isPlayerDisabled = disable;
        }
    }
}