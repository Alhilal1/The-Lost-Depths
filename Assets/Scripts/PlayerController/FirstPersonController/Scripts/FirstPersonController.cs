using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.InputSystem.InputSettings;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Current player\'s velocity")]
		public float targetSpeed;
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;
		[Tooltip("Current player\'s Speed")]
        public float _speed;

        [Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 1.2f;
		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.1f;
		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		[Header("Match & Flashlight Prefabs")]
		[Tooltip("Drop the match prefabs here")]
		public GameObject MatchPrefabs;
		[Tooltip("Drop the match light here")]
		public GameObject MatchLight;
        [Tooltip("Drop the flashlight prefabs here")]
        public GameObject FlashlightPrefabs;
        [Tooltip("Drop the flashlight light here")]
        public GameObject FlashlightLight;

		[Header("Global Volume Edit")]
		[Tooltip("Global Volume")]
		public Volume GlobalVolume;
        public float MatchView;
        public float FlashlightView;
        public float NormalView;
		public GameObject CutsceneManager;
		public GameObject UIManager;

        // cinemachine
        private float _cinemachineTargetPitch;
		
		// player
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;
		
		// crouch & prone
        private float _originalHeight;
        private float _crouchHeight = 0.6f;        
		private float _crouchScale = 0.5f;
		private float _proneScale = 0.1f;
        private float _originalSpeed;

		// animation
		private Animator _animator;

		// match & flashlihgt
		private bool _isMatchOn = false;
		private bool _matchPrefabsActive;
		[SerializeField] private bool _isFlashlightOn = false;
		[SerializeField] private bool _flashlightPrefabsActive;
        private int _flashlightHash = Animator.StringToHash("Flashlight");
		[SerializeField] private float _batteryLevel = 0f;
		private AudioSource _audioSource;

		[Header("Audio Reference")]
		public AudioClip flashlightDead;
		public AudioClip firstBatteryFound;
		public AudioClip flashlightSound;

        // volume
        private Fog _fogVolume;

		// cutscene
		public bool canMove;
		private CutsceneManager _scene;
		private UIManager _ui;


#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;

		private const float _threshold = 0.01f;

		private bool IsCurrentDeviceMouse
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
				#else
				return false;
				#endif
			}
		}

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
			_animator = GetComponent<Animator>();
            _scene = CutsceneManager.GetComponent<CutsceneManager>();
			_ui = UIManager.GetComponent<UIManager>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();			
#else
			Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
			_originalHeight = _controller.center.y;
			_originalSpeed = MoveSpeed;
			_audioSource = transform.GetChild(4).gameObject.GetComponent<AudioSource>();
		}

		private void Update()
		{
			if (canMove)
			{
                JumpAndGravity();
                GroundedCheck();
                Move();
                CrouchProneHandling();
                MatchHandling();
                FlashlightHandling();
            }
		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}

		private void CameraRotation()
		{
			// if there is an input
			if (_input.look.sqrMagnitude >= _threshold)
			{
				//Don't multiply mouse input by Time.deltaTime
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				_cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
				_rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

				// clamp our pitch rotation
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Update Cinemachine camera target pitch
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

				// rotate the player left and right
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed 
			targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
			targetSpeed = _input.crouch && !_input.prone? targetSpeed / 2 : targetSpeed;
			targetSpeed = !_input.crouch && _input.prone? targetSpeed / 4 : targetSpeed;

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) targetSpeed = 0.0f;

			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

				// round speed to 3 decimal places
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				// move
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}

			// move the player
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				// reset the fall timeout timer
				_fallTimeoutDelta = FallTimeout;

				// stop our velocity dropping infinitely when grounded
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				_jumpTimeoutDelta = JumpTimeout;

				// fall timeout
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}

				// if we are not grounded, do not jump
				_input.jump = false;
			}

			// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		}

        private void CrouchProneHandling()
        {
            if (_input.prone)
            {
                transform.localScale = new Vector3(1f, _proneScale, 1f);
				this.gameObject.transform.GetChild(0).transform.localScale = new Vector3(1f, 1/_proneScale, 1f);
                _controller.center = new Vector3(0f, 2.7f, 0f);                
            }
            else if (_input.crouch)
            {
                if (CanCrouch()) 
                {
                    transform.localScale = new Vector3(1f, _crouchScale, 1f);
                    this.gameObject.transform.GetChild(0).transform.localScale = new Vector3(1f, 1/_crouchScale, 1f);
                    _controller.center = new Vector3(0f, 1f, 0f);                                           
                }
                else
                {
					_input.crouch = false;
                    _input.prone = true;
                    Debug.Log("Gabisa Apa Apa");
                }
            }
            else
            {
                if (CanStandUp())
                {
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    this.gameObject.transform.GetChild(0).transform.localScale = new Vector3(1f, 1f, 1f);
                    _controller.center = new Vector3(0f, 1f, 0f);                  
                }
                else if (CanCrouch())  // If standing isn't possible, try crouching
                {
					_input.crouch = true;
                    transform.localScale = new Vector3(1f, _crouchScale, 1f);
                    this.gameObject.transform.GetChild(0).transform.localScale = new Vector3(1f, 1/_crouchScale, 1f);
                    _controller.center = new Vector3(0f, 1f, 0f);                   
					Debug.Log("Gabisa Berdiri");
                }
                else
                {
					Debug.Log("Gabisa Apa Apa");
                    _input.prone = true; // If neither is possible, stay prone
                }
            }
        }

        private bool CanStandUp()
        {
            float checkHeight = 2f;
            Vector3 checkPosition = transform.position;

            return !Physics.Raycast(checkPosition, Vector3.up, checkHeight, LayerMask.GetMask("Environment"));
        }

        private bool CanCrouch()
        {
            float checkHeight = 0.5f;
            Vector3 checkPosition = transform.position + Vector3.up * _crouchHeight / 2;

            return !Physics.Raycast(checkPosition, Vector3.up, checkHeight, LayerMask.GetMask("Environment"));
        }

		public void MatchHandling()
		{
			if (_input.match && !_isMatchOn)
			{
				_isMatchOn = true;
				_matchPrefabsActive = true;
				StartCoroutine(MatchCaroutine());
			}
			else if (!_input.match && _matchPrefabsActive)
			{
				_matchPrefabsActive = false;
				StartCoroutine(MatchCaroutine());
			}
		}

		private IEnumerator MatchCaroutine()
		{

            yield return new WaitForSeconds(1f);

			MatchPrefabs.SetActive(_matchPrefabsActive);
			MatchLight.SetActive(_matchPrefabsActive);
			FogChange(MatchView, _matchPrefabsActive);

			_isMatchOn = false;
		}

		public void FogChange(float view, bool check)
		{
            if (GlobalVolume.profile != null)
            {
                if (GlobalVolume.profile.TryGet(out _fogVolume))
                {
					// Modify the attenuation distance
					_fogVolume.meanFreePath.value = check ? view : NormalView;
                }
                else
                {
                    Debug.LogWarning("Fog settings not found in the volume profile.");
                }
            }
            else
            {
                Debug.LogWarning("No Volume component found on this GameObject.");
            }

        }

        public void FlashlightHandling()
        {
				if (_input.flashlight && !_isFlashlightOn)
				{
					if (_batteryLevel > 0f)
					{
						_isFlashlightOn = true;
						_flashlightPrefabsActive = !_flashlightPrefabsActive;
						_input.flashlight = _flashlightPrefabsActive;

						_animator.SetTrigger("FlashlightTrigger");
						StartCoroutine(FlashlightCoroutine());
					}
                    else
                    {
						_audioSource.Stop();
                        _audioSource.PlayOneShot(flashlightDead);
                        _isFlashlightOn = false;
                        _input.flashlight = false;
                    }
                }
				else if (!_input.flashlight && _isFlashlightOn && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.5f)
				{
					_isFlashlightOn = false;
					_flashlightPrefabsActive = !_flashlightPrefabsActive;
					_input.flashlight = _flashlightPrefabsActive;

					_animator.SetTrigger("FlashlightTrigger");
					StartCoroutine(FlashlightCoroutine());
				}
				_input.flashlight = _flashlightPrefabsActive;
            
        }

        private IEnumerator FlashlightCoroutine()
        {
            yield return new WaitForSeconds(2.0f);

			_audioSource.PlayOneShot(flashlightSound);
            FlashlightLight.SetActive(_flashlightPrefabsActive);
            FogChange(FlashlightView, _flashlightPrefabsActive);

        }

        public void LocalAdjustmentUpdate(float newNormalView, float newMatchView, float newFlashlightView)
		{
			NormalView = newNormalView;
			MatchView = newMatchView;
			FlashlightView = newFlashlightView;
		}

		public bool thereIsBattery()
		{
			return _batteryLevel > 0f;
		}

		public void addBattery(float add)
		{
			_batteryLevel += add;
			_batteryLevel = Mathf.Clamp(_batteryLevel, 0f, 1f);
		}

		public void FoundFirstBattery()
		{
			_audioSource.Stop();
			_audioSource.PlayOneShot(firstBatteryFound);
		}
    }

}