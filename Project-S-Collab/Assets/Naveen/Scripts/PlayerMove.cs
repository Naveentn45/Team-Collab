using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;



public class PlayerMove : MonoBehaviour
{
    public CinemachineSplineDolly dolly;
    public Transform player;
    public float moveSpeed = 5f;

    private PlayerControl controls;
    private Animator animator;
    private float move;

    private void FixedUpdate()
    {
        dolly.CameraPosition = player.position.x;
    }
    void Awake()
    {
        controls = new PlayerControl();
        animator = GetComponent<Animator>();

        controls.Player.Move.performed += ctx => move = ctx.ReadValue<float>();
        controls.Player.Move.canceled += ctx => move = 0f;
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        Vector3 moveDirection = new Vector3(move, 0f, 0f);

        CharacterController controller = GetComponent<CharacterController>();
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        
        if (move > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (move < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        animator.SetFloat("Speed", Mathf.Abs(move));
    }
}
