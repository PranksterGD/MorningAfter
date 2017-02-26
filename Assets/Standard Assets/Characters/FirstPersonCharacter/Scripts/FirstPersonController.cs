using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(GameManager))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(MouseLook))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField]
        private bool m_IsWalking;

        [SerializeField]
        private float m_WalkSpeed;

        [SerializeField]
        [Range(0f, 1f)]
        private float m_RunstepLenghten;

        [SerializeField]
        private float m_GravityMultiplier;

        [SerializeField]
        private float m_StepInterval;

        [SerializeField]
        private float m_StickToGroundForce;

        private GameManager gameManager;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private float m_StepCycle;
        private float m_NextStep;
        private MouseLook mouseLook;

        public Canvas canvas;
        private Text interactText;

        public float cameraZoomInSpeed = 3;

        private bool isInteracting;
        public static bool hasControl;
        private Vector3 currentPosition;
        private Quaternion currentrotation;
        private Vector3 targetPosition;
        private Quaternion targetrotation;

        [SerializeField]
        private float minimumResetDistance = 0.1f;

        [SerializeField]
        private float minimumResetAngle = 10.0f;

        [SerializeField]
        private float distanceFromObject = 0.5f;

        [SerializeField]
        private float lerpTime = 1f;

        private float currentLerpTime = 0;

        [SerializeField]
        private float ZoomInMinX;
        [SerializeField]
        private float ZoomInMaxX;
        [SerializeField]
        private float ZoomInMinY;
        [SerializeField]
        private float ZoomInMaxY;

        private Coroutine ZoomCoroutine;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            canvas = canvas.GetComponent<Canvas>();
            interactText = canvas.transform.FindChild("InteractText").GetComponent<Text>();
            interactText.enabled = false;
            mouseLook = GetComponent<MouseLook>();
            gameManager = GetComponent<GameManager>();

            isInteracting = false;
            hasControl = false;
            targetPosition = transform.position;
            targetrotation = transform.rotation;
        }

        // Update is called once per frame
        private void Update()
        {
            //print(hasControl);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                m_MoveDir.y = 0f;
            }
            if (!m_CharacterController.isGrounded && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            //ZoomInOnInteractable();
            HandleInteractable();
        }

        IEnumerator ZoomIn(Transform target)
        {
            currentPosition = transform.position;
            currentrotation = transform.rotation;
            transform.LookAt(target);
            targetPosition = target.position + (transform.position - target.position).normalized * distanceFromObject;
            targetrotation = transform.rotation;
            transform.position = currentPosition;
            transform.rotation = currentrotation;
            hasControl = false;
            mouseLook.setMovementLocked(true);
            currentLerpTime = 0;
            while (true)
            {
                float lerpFraction = ZoomMotion();
                if (lerpFraction >= 0.9f)
                {
                    break;
                }
                yield return null;
            }
            mouseLook.setMovementLocked(true);
            // mouseLook.setMouseLookLimits(ZoomInMinX, ZoomInMaxX, ZoomInMinY, ZoomInMaxY);
        }

        float ZoomMotion()
        {
            currentLerpTime += Time.deltaTime;
            float lerpFraction = currentLerpTime / lerpTime;
            if (lerpFraction >= 0.9f)
            {
                transform.position = targetPosition;
                transform.rotation = targetrotation;
            }
            else
            {
                lerpFraction = lerpFraction * lerpFraction * lerpFraction * (lerpFraction * (6 * lerpFraction - 15) + 10);
                transform.position = Vector3.Lerp(transform.position, targetPosition, lerpFraction);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetrotation, lerpFraction);
            }
            return lerpFraction;
        }

        IEnumerator ZoomOut()
        {
            targetPosition = currentPosition;
            targetrotation = currentrotation;
            currentLerpTime = 0;
            mouseLook.setMovementLocked(true);
            while (true)
            {
                float lerpFraction = ZoomMotion();
                if (lerpFraction >= 0.9f)
                {
                    break;
                }
                yield return null;
            }

            transform.position = currentPosition;
            transform.rotation = currentrotation;

            mouseLook.setMovementLocked(false);
            // mouseLook.resetMouseLookLimits();
            hasControl = true;
        }

        private void HandleInteractable()
        {
            if (CrosshairRaycast.canInteract || isInteracting)
            {
                Transform target = CrosshairRaycast.interactingObject.transform;
                Emissive emissive = target.GetComponent<Emissive>();
                if (emissive != null)
                {
                    emissive.lookAt();
                }

                //interactText.enabled = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    //interactText.enabled = false;
                    InteractableBehaviour interactable = target.GetComponent<InteractableBehaviour>();
                    isInteracting = !isInteracting;
                    if (isInteracting)
                    {
                        playAudioForInteractable(target);
                        if (interactable == null || interactable.canZoomIn())
                        {
                            if (ZoomCoroutine != null)
                            {
                                StopCoroutine(ZoomCoroutine);
                            }
                            ZoomCoroutine = StartCoroutine(ZoomIn(target));
                        }

                        if (interactable != null)
                        {
                            interactable.startInteraction();
                            if (!interactable.canZoomIn())
                            {
                                isInteracting = false;
                            }
                        }
                        gameManager.interacted(target.gameObject);
                    }
                    else
                    {
                        if (interactable != null)
                        {
                            interactable.endInteraction();
                        }
                        if (ZoomCoroutine != null)
                        {
                            StopCoroutine(ZoomCoroutine);
                        }
                        ZoomCoroutine = StartCoroutine(ZoomOut());                        
                    }
                }
            }
            else
            {
                //interactText.enabled = false;
            }
        }

        public void ToggleHasControl(bool movementEnabled)
        {
            hasControl = movementEnabled;
        }

        private void playAudioForInteractable(Transform interactable)
        {
            AudioSource audioSource = interactable.GetComponent<AudioSource>();
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        private void stopAudioForInteractable(Transform interactable)
        {
            AudioSource audioSource = interactable.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        private void FixedUpdate()
        {
            if (!hasControl)
            {
                return;
            }

            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f, ~0, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;

            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;
            }
            else
            {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }

            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
            ProgressStepCycle(speed);
        }

        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;
        }

        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            m_Input = new Vector2(horizontal, vertical);
            speed = m_WalkSpeed;

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //don't move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
    }
}