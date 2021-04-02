using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private int speed;
    
    [SerializeField] private Slider healthBar;
    
    private SpriteRenderer sprite;
    private new Rigidbody2D rigidbody;
    
    private InputMaster input;
    private Vector2 movement;

    internal int Health
    {
        get => health;
        set
        {
            if (value < 0) 
                Debug.Log("death");
            
            health = value;
            if (health > maxHealth)
                health = maxHealth;
            healthBar.value = value;
        }
    }

    private void Awake()
    {
        input = new InputMaster();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        rigidbody = gameObject.GetComponent<Rigidbody2D>();
        input.Player.Move.performed += context => Move(context.ReadValue<Vector2>());
        input.Player.Move.canceled += context => movement = Vector3.zero;
        Health = maxHealth;
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
