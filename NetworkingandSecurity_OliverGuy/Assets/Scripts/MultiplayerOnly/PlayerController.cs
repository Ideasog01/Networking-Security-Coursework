using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
    public class PlayerController : MonoBehaviour
    {
        public bool pauseMenuActive;

        [Header("Ability Settings")]

        [SerializeField] private float[] abilityCooldownDurations; //The length of each ability cooldown in seconds.
        [SerializeField] private Transform projectileSpawn;
        [SerializeField] private ParticleSystem disableEffect; //The visual effect that will display when the player is disabled.

        [Header("Primary Ability")]

        [SerializeField] private float primaryProjectileSpeed;
        [SerializeField] private Transform primaryProjectilePrefab;
        [SerializeField] private GameObject primaryVisualEffectPrefab; //The visual effect that displays on primary ability activation.

        [Header("Ability 1")]

        [SerializeField] private float ability1ProjectileSpeed;
        [SerializeField] private Transform ability1ProjectilePrefab;

        [Header("Ability 2")]

        [SerializeField] private ParticleSystem shieldVisualEffect;  //The visual effect that displays on shield ability activation.
        [SerializeField] private float shieldDuration = 7;

        [Header("Ability 3")]

        [SerializeField] private int ability3ProjectileSpeed;
        [SerializeField] private Transform ability3ProjectilePrefab;
        [SerializeField] private float disableTimeDuration = 5; //The amount of time to disable the player hit by this ability in seconds.

        private float[] _abilityCooldowns = new float[4]; //The current cooldowns of all abilities, including primary fire.

        //Player Components
        private PlayerMovement _playerMovement;
        private HealthController _playerHealthController;
        private Animator _playerAnimator;
        private PhotonView _photonView;

        private bool _isPlayerDisabled;
        private float _disableTimer; //The current time remaining that the player is disabled. If this value is disabled, the player should not currently be disabled by another player. (Could be disabled via end of game or death)

        #region Properties

        public PlayerMovement PlayerMovement
        {
            get { return _playerMovement; }
        }

        public HealthController HealthController
        {
            get { return _playerHealthController; }
        }

        #endregion

        #region Core

        private void Awake()
        {
            //Initialise Player Components
            _playerMovement = this.GetComponent<PlayerMovement>();
            _playerHealthController = this.GetComponent<HealthController>();
            _playerAnimator = this.transform.GetChild(0).GetComponent<Animator>();
            _photonView = this.GetComponent<PhotonView>();

            this.transform.parent.GetChild(1).gameObject.SetActive(_photonView.IsMine); //Enable the following cursor object if this controller belongs to the local player.
            this.transform.GetChild(3).gameObject.SetActive(!_photonView.IsMine); //Only display the in-world health slider if this controller does NOT belong to the local player.
        }

        private void Update()
        {
            if (_photonView.IsMine)
            {
                if(!_isPlayerDisabled)
                {
                    PlayerGameplayInput(); //As long as the player is enabled, they can perform abilities (if not on cooldown).
                    _playerMovement.UpdateRotation(); //Updates the rotation to follow the mouse cursor.
                }

                if (!GameManager.GameInProgress)
                {
                    _playerMovement.StopMovement = true; //Do not allow the player to move if the game is no longer in progress
                }
                else if (Input.GetKeyDown(KeyCode.Escape)) //Pause menu does not affect other players or the gamemode state.
                {
                    FindFirstObjectByType<GameManager>().DisplayPauseMenu(true);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_photonView.IsMine)
            {
                _playerMovement.UpdateMovement(); //Update physics on fixed update only
            }
        }

        private void PlayerGameplayInput()
        {
            if (_disableTimer > 0) //Reduce the disable duration overtime.
            {
                _disableTimer -= Time.deltaTime * 1;

                if (_disableTimer <= 0)
                {
                    ActivateNetworkFunction("EnablePlayer");
                }
            }
            else //If the player is enabled, they can try to use any abilities.
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
        }

        public void ActivateNetworkFunction(string functionName)
        {
            _photonView.RPC(functionName, RpcTarget.AllViaServer); //Called on all clients
        }

        #endregion

        #region Ability Functions (Class)

        [PunRPC]
        private void PrimaryAttack()
        {
            //Spawn the projectile and apply a velocity
            Rigidbody projectileRb = Instantiate(primaryProjectilePrefab.GetComponent<Rigidbody>(), projectileSpawn.transform.position, this.transform.rotation); 

            projectileRb.transform.forward = this.transform.forward;
            projectileRb.velocity = projectileRb.transform.forward * primaryProjectileSpeed;

            CollisionController collisionController = projectileRb.GetComponent<CollisionController>();
            collisionController.Owner = _photonView.Owner;
            collisionController.OwnerCollider = this.GetComponent<Collider>();

            //Apply ability cooldown based on assigned duration
            _abilityCooldowns[0] = abilityCooldownDurations[0];
            StartCoroutine(AbilityCooldown(0));
        }

        [PunRPC]
        private void LightBlast()
        {
            //Spawn the projectile and apply a velocity
            Rigidbody projectileRb = Instantiate(ability1ProjectilePrefab.GetComponent<Rigidbody>(), projectileSpawn.transform.position, this.transform.rotation);

            projectileRb.transform.forward = this.transform.forward;
            projectileRb.velocity = projectileRb.transform.forward * ability1ProjectileSpeed;

            CollisionController collisionController = projectileRb.GetComponent<CollisionController>();
            collisionController.Owner = _photonView.Owner;
            collisionController.OwnerCollider = this.GetComponent<Collider>();

            //Apply ability cooldown based on assigned duration
            _abilityCooldowns[1] = abilityCooldownDurations[1];
            StartCoroutine(AbilityCooldown(1));
        }

        [PunRPC]
        private void Shield() //Makes the player immune to damage for a duration.
        {
            _playerHealthController.IsInvulnerable = true;
            shieldVisualEffect.Play();
            StartCoroutine(CancelShield()); //Note that the shield abilities' cooldown is only started after it ends.
        }

        private IEnumerator CancelShield() //Cancel the player's immunity to damage after a duration. Note that the visual effect duration is assigned inside the inspector on the corresponding particle system component.
        {
            yield return new WaitForSeconds(shieldDuration);
            _abilityCooldowns[1] = abilityCooldownDurations[2];
            StartCoroutine(AbilityCooldown(2)); //Now start the shield ability cooldown.
            _playerHealthController.IsInvulnerable = false;
        }

        [PunRPC]
        private void TimeBlast()
        {
            //Spawn the projectile and create a path that will be followed and randomised nearer to the end.
            FollowPath projectile = Instantiate(ability3ProjectilePrefab.GetComponent<FollowPath>(), projectileSpawn.transform.position, this.transform.rotation);

            projectile.transform.forward = this.transform.forward;

            CollisionController collisionController = projectile.GetComponent<CollisionController>();
            collisionController.Owner = _photonView.Owner;
            collisionController.OwnerCollider = this.GetComponent<Collider>();

            List<Vector3> pathList = new List<Vector3>();
            for (int i = 1; i < ability3ProjectileSpeed; i++)
            {
                Vector3 pathPoint = (projectileSpawn.position) + projectileSpawn.forward * i;

                if (i > 5) //Once we have passed five increments, randomise the path.
                {
                    pathPoint = new Vector3(pathPoint.x + Random.Range(-2, 2), pathPoint.y, pathPoint.z + Random.Range(-2, 2));
                }

                pathList.Add(pathPoint);
            }

            //The projectile will follow the path that has been created.
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

            GameManager.PlayerDisplay.UpdateCooldownDisplay(abilityIndex, abilityCooldownDurations[abilityIndex], _abilityCooldowns[abilityIndex]);

            if (_abilityCooldowns[abilityIndex] > 0)
            {
                StartCoroutine(AbilityCooldown(abilityIndex));
            }
        }

        private void PlayAbilityAnimation(string trigger)
        {
            _playerAnimator.SetTrigger(trigger);
            _playerMovement.StopMovement = true; //When an ability animation is currently active, disable the player's movement.
        }

        #endregion

        #region Player States

        public void DisablePlayerActions(bool disable) //For general game functions, such as showing the pause menu or ending the game. (DisablePlayer is for ability use only)
        {
            _playerMovement.StopMovement = disable;
            _isPlayerDisabled = disable;
        }

        public void ResetPlayer(Vector3 position) //Resets the player and teleports the object to the given position.
        {
            _playerHealthController.Heal(_playerHealthController.MaxHealth);
            _playerMovement.StopMovement = false;
            DisablePlayerActions(false);
            _playerAnimator.SetBool("isDead", false);
            this.transform.position = position;
        }

        #endregion

        #region Enable & Disable Player (Ability)

        //We need to enable/disable this player on all clients as we want to configure the display. (Referring to animation and effects)

        [PunRPC]
        private void EnablePlayer()
        {
            disableEffect.Stop();
            _playerMovement.StopMovement = false;
            _playerAnimator.SetBool("isDisabled", false);
        }

        [PunRPC]
        private void DisablePlayer() //Ability Only
        {
            if (!_playerHealthController.IsInvulnerable && _playerHealthController.CurrentHealth > 0)
            {
                _disableTimer = disableTimeDuration; //The timer will be reduced overtime, until it reaches zero and the player is enabled again.
                _playerMovement.StopMovement = true;
                _playerAnimator.SetBool("isDisabled", true);
                disableEffect.Play();
            }
        }

        #endregion
    }
}


