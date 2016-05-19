using UnityEngine;
using System.Collections;

public class MechController : MonoBehaviour
{
    private int m_SpeedHash = Animator.StringToHash("Speed");
    private int m_TurnHash = Animator.StringToHash("Turn");
    private int m_JumpHash = Animator.StringToHash("Jumping");
    private int m_GroundedHash = Animator.StringToHash("Grounded");
    private Animator m_Animator;

    private float m_StickToGroundForce = 1f;

    public float m_JumpSpeed = 1f;
    public float m_JumpVertical = 1f;
    public float m_JumpTurning = 10f;

    public float m_CurrentJumpSpeed;
    private CharacterController m_CharacterController;
    private Vector3 m_MoveDir = Vector3.zero;

    public Transform m_FirePoint;
    public float m_FireDistance = 10f;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        bool fire = Input.GetButtonDown("Fire1");
        if (fire)
        {
            Debug.DrawRay(m_FirePoint.position, m_FirePoint.forward * m_FireDistance, Color.red, 1f);
            Ray ray = new Ray(m_FirePoint.position, m_FirePoint.forward);
            RaycastHit hit;
            int mask = LayerMask.GetMask(new string[] { "Default" });
            if (Physics.Raycast(ray, out hit, m_FireDistance, mask))
            {
                Debug.Log("Hit " + hit.collider.name);
            }
        }

        bool Jumping = Input.GetButton("Jump");
        m_Animator.SetBool(m_JumpHash, Jumping);
        if (!Jumping)
        {
            m_CurrentJumpSpeed = Mathf.Lerp(m_CurrentJumpSpeed, 0f, 0.35f);
            m_Animator.SetFloat(m_SpeedHash, Input.GetAxis("Vertical"));
            m_Animator.SetFloat(m_TurnHash, Input.GetAxis("Horizontal"));
            m_MoveDir = Vector3.zero;
        }
        else
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            m_CharacterController.transform.Rotate(0f, h * m_JumpTurning * Time.deltaTime, 0f);
            m_CurrentJumpSpeed = Mathf.Lerp(m_CurrentJumpSpeed, m_JumpVertical, 0.35f);
            m_MoveDir = m_CharacterController.transform.forward * m_JumpSpeed * v * Time.deltaTime;
        }
    }
        public void FixedUpdate()
    {
        if (m_CharacterController.isGrounded)
        {
            m_MoveDir.y = -m_StickToGroundForce;
        }
        else
        {
            m_MoveDir += Physics.gravity * Time.fixedDeltaTime;
        }

        m_MoveDir.y += m_CurrentJumpSpeed * Time.deltaTime;
        m_CharacterController.Move(m_MoveDir);

        m_Animator.SetBool(m_GroundedHash, m_CharacterController.isGrounded);
    }
}

