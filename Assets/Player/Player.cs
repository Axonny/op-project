using UnityEngine;

public class Player : MonoBehaviour
{
    // internal int health;
    // internal int maxHealth;
    [SerializeField] private int speed;
    
    private SpriteRenderer sprite;
    private new Rigidbody2D rigidbody;
    
    private InputMaster input;
    private Vector2 movement;

    private void Awake()
    {
        input = new InputMaster();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        input.Player.Move.performed += context => Move(context.ReadValue<Vector2>());
        input.Player.Move.canceled += context => movement = Vector3.zero;
    }

    private void Update()
    {
        rigidbody.velocity = movement * speed;
    }
    
    private void Move(Vector2 inputMovement)
    {
        movement = inputMovement;
        if (inputMovement.x != 0)
            sprite.flipX = inputMovement.x < 0;
    }
    
    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
