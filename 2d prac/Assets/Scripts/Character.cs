using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    [SerializeField]
    private int lives = 5;
    [SerializeField]
    private float speed = 3.0F;
    [SerializeField]
    private float jumpForce = 15.0F;

    private bool isGrounded = false;

    private Bullet bullet;

    Vector2 colliderOffset;
    Vector2 colliderSize;


    private CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State",(int) value); }
    }

    new private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer sprite;

    private void Awake()
    {

        colliderOffset = gameObject.GetComponent<BoxCollider2D>().offset;
        colliderSize = gameObject.GetComponent<BoxCollider2D>().size;
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        bullet = Resources.Load<Bullet>("Bullet");
    }

    private void FixedUpdate()
    {
        CheckGround();
    }
    private void Update()
    {
        if(isGrounded) State = CharState.Idle;

        

        if (Input.GetButtonDown("Fire1")) Shoot();
        if (Input.GetButton("Horizontal")) Run();      
        if (isGrounded && Input.GetButton("Jump")) Jump();
        Sit();



    }
    private void Run()
    {
        Vector3 direction = transform.right * Input.GetAxis("Horizontal");

        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

        sprite.flipX = direction.x < 0.0F;

        

        if(isGrounded) State = CharState.Run;

        

    }
    
    private void Jump()
    {
        rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Sit()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0.1f);
            gameObject.GetComponent<BoxCollider2D>().size = new Vector2(1, 0.2f);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            gameObject.GetComponent<BoxCollider2D>().offset = colliderOffset;
            gameObject.GetComponent<BoxCollider2D>().size = colliderSize;
        }
    }

    private void Shoot()
    {
        Vector3 position = transform.position; position.y += 0.7F;
        Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation) as Bullet ;

        newBullet.Parent = gameObject;
        newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1.0F : 1.0F);

        State = CharState.attack;
    }

    public override void RecieveDamage()
    {
        lives--;

        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(transform.up * 9.0F, ForceMode2D.Impulse);

        Debug.Log(lives);
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3F);

        isGrounded = colliders.Length > 1;

        if(!isGrounded) State = CharState.Jump;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Unit unit = collision.gameObject.GetComponent<Unit>();
        if (unit) RecieveDamage();

    }

}

public enum CharState
{
    Idle,
    Run,
    Jump,
    attack
}