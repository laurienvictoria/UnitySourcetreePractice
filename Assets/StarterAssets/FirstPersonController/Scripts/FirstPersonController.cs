using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using DG.Tweening;


namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class FirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 4.0f;
		[Tooltip("Min Move speed of the character in m/s when on sloped terrain")]
		public float MinMoveSpeed = 1f;
		public float MaxMoveSpeed = 1.7f;
		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 6.0f;
		public float MinSprintSpeed = 2.2f;
		public float MaxSprintSpeed = 3.2f;
		[Tooltip("Crouch speed of the character in m/s")]
		public float CrouchSpeed = 0.5f;
		public float MinCrouchSpeed = 0.2f;
		public float MaxCrouchSpeed = 0.55f;

		[Tooltip("Rotation speed of the character")]
		public float RotationSpeed = 1.0f;
		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

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

		[Space(20)]
		// player
		[Tooltip("Position in front of player raycast is being cast from, to work out slope height")]
		public float positionForward = 2.0f;
		[Tooltip("Magic number used to affect how much bob is modified depending on modified speed (worked out based on slope height")]
		public float bobModifier = 0.5f;


		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;
		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;
		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.5f;
		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Crouch Parameters")]
		public float crouchHeight;
		public float standingHeight;
		public float timeToCrouch;

		[Header("Headbob Parameters")]
		[SerializeField] private bool useHeadbob = true;
		public float walkBobSpeed = 14f;
		public float walkBobAmount = 0.05f;
		public float sprintBobSpeed = 18f;
		public float sprintBobAmount = 0.1f;
		public float crouchBobSpeed = 8f;
		public float crouchBobAmount = 0.025f;
		private float defaultYPos = 0;
		private float timer;

		[Header("Footstep Parameters")]
		[SerializeField] private bool useFootsteps = true;
		private AudioSource footstepAudioSource;
		[SerializeField] private AudioClip[] grassFootsteps;
		[SerializeField] private AudioClip[] woodFootsteps;


		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;
		public GameObject PlayerHeadHeight;
		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 90.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -90.0f;

		// cinemachine
		private float _cinemachineTargetPitch;


		[Space(40)]
		public float _speed;
		public float _modifiedSpeed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		//crouch
		private Tweener crouchTween;




#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
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
				#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
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

			footstepAudioSource = GetComponent<AudioSource>();
			PositionCheck = new GameObject();
			PositionCheck2 = new GameObject();
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
			_playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

			// reset our timeouts on start
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		private void Update()
		{
			JumpAndGravity();
			GroundedCheck();
			Move();

			if (useHeadbob)
				HandleHeadbob();

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

		private GameObject PositionCheck;
		private GameObject PositionCheck2;
		private float targetSpeed;

		private void Move()
		{
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// set target speed based on move speed, sprint speed and if sprint is pressed
			targetSpeed = _input.crouch ? CrouchSpeed : _input.sprint ? SprintSpeed : MoveSpeed;

			targetSpeed = ModifySpeed(targetSpeed, HeightDifference(inputDirection));
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
			//Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

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


		private float HeightDifference(Vector3 inputDirection)
        {
			float heightDifference;
			//SLOPE ANGLE CALCULATIONS
			Vector3 inputDirectionPlus = transform.position + transform.TransformDirection(inputDirection) * positionForward;
			Vector3 rayOrigin = inputDirectionPlus;
			rayOrigin.y = 100f;
			RaycastHit hitInfo;
			Physics.Raycast(rayOrigin, Vector3.down, out hitInfo, Mathf.Infinity, GroundLayers);

			Vector3 predictedPos = hitInfo.point;
			PositionCheck.transform.position = hitInfo.point;
			rayOrigin = transform.position;
			rayOrigin.y = 100f;

			Physics.Raycast(rayOrigin, Vector3.down, out hitInfo, Mathf.Infinity, GroundLayers);
			Vector3 myPos = hitInfo.point;
			PositionCheck2.transform.position = hitInfo.point;

			return heightDifference = Mathf.Round((predictedPos.y - myPos.y) * 1000) / 1000;
		}


		private float MaxSlopeDiffModifier = 1.2f;
		public float slopeHeight;
		private float ModifySpeed(float speed, float heightDiff)
        {
            if (heightDiff == 0)
				return speed;
			slopeHeight = heightDiff;
			float modSpeed = speed;
			heightDiff = Mathf.Clamp(heightDiff, -MaxSlopeDiffModifier, MaxSlopeDiffModifier);

				//uphill
			if (heightDiff > 0)
            {
				float minSpeed = _input.crouch ? MinCrouchSpeed : _input.sprint ? MinSprintSpeed : MinMoveSpeed;
				modSpeed = ExtensionMethods.Remap(heightDiff, 0, MaxSlopeDiffModifier, speed, minSpeed);
			}
			else if (heightDiff < 0)
			{
				float maxSpeed = _input.crouch ? MaxCrouchSpeed : _input.sprint ? MaxSprintSpeed : MaxMoveSpeed;
				modSpeed = ExtensionMethods.Remap(heightDiff, 0, -MaxSlopeDiffModifier, speed, maxSpeed);
			}

			return modSpeed;
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
				/*if (_input.jump && _jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
				}

				// jump timeout
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}*/
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

		public void StartCrouch()
        {
			CrouchTween(standingHeight, crouchHeight);
		}
		public void StopCrouch()
		{
			CrouchTween(crouchHeight, standingHeight);
		}



		private void CrouchTween(float startHeight, float endHeight)
        {
			if (crouchTween != null) crouchTween.Kill();

			crouchTween = DOVirtual.Float(startHeight, endHeight, timeToCrouch, (float val) => {
				SetCameraHeight(val);
			}).OnComplete(() => {
				crouchTween = null;
			});
		}


		private Vector3 camPosition = new Vector3(0,0,0);
		private void SetCameraHeight(float val)
        {
            camPosition = PlayerHeadHeight.transform.position;
			camPosition.y = val;
			PlayerHeadHeight.transform.position = camPosition;
		}


		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}



		public float modifiedBobSpeed;
		private float nextFootstepTime = 4; //at 1.5 * PI, the bob will be at its lowest point, so this value is just shy of that so that the step audio initiates just before.
		
		//LAURIEN NOTE: sin has a max number in unity, so still need to reset the timer when player not moving
		private void HandleHeadbob()
		{
			if (!Grounded) return;
			float defaultMoveSpeed = MoveSpeed;
			float defaultBobSpeed = walkBobSpeed;

			if (_input.crouch)
			{
				defaultMoveSpeed = CrouchSpeed;
				defaultBobSpeed = crouchBobSpeed;
			}
			else if (_input.sprint)
			{
				defaultMoveSpeed = SprintSpeed;
				defaultBobSpeed = sprintBobSpeed;
			}

			float bob_speed = defaultBobSpeed;

			if (_speed != defaultMoveSpeed)
            {
				bob_speed = defaultBobSpeed * (1 + ((_speed / defaultMoveSpeed - 1) * bobModifier));
			}

			modifiedBobSpeed = bob_speed;

			if (_speed > 0)
			{
				timer += Time.deltaTime * (bob_speed);


				CinemachineCameraTarget.transform.localPosition = new Vector3(
					CinemachineCameraTarget.transform.localPosition.x,
					defaultYPos + Mathf.Sin(timer) * (_input.crouch ? crouchBobAmount : _input.sprint ? sprintBobAmount : walkBobAmount),
					CinemachineCameraTarget.transform.localPosition.z);

                if (useFootsteps)
                {
					if (timer >= nextFootstepTime)
					{
						PlayFootstep();
						nextFootstepTime += 2 * Mathf.PI;
					}
				}
			}
            else
				modifiedBobSpeed = 0;
		}

		private void PlayFootstep()
		{
			if (Physics.Raycast(CinemachineCameraTarget.transform.position, Vector3.down, out RaycastHit hit, 3))
			{
				switch (hit.collider.tag)
				{
					case "Footsteps/Grass":
						footstepAudioSource.PlayOneShot(grassFootsteps[Random.Range(0, grassFootsteps.Length - 1)]);
						break;
					case "Footsteps/Wood":
						footstepAudioSource.PlayOneShot(woodFootsteps[Random.Range(0, woodFootsteps.Length - 1)]);
						break;
					default:
						break;
				}
			}
	
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
	}
}