using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Unit
{
    [SerializeField]
    private float speed = 3.0F;
    [SerializeField]
    private int lives = 5;
    [SerializeField]
    private float jumpForce = 15.0F;
    //[SerializeField]
    //private float nextFire = 0.5F;
    [SerializeField]
    private float positionBullet = 0.4F;
    private float time;
    private bool isGrounded = false;

    private Bullet bullet;

    private CharState State
    {
        get { return (CharState)animator.GetInteger("State"); }
        set { animator.SetInteger("State", (int)value); }
    }

    new private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer sprite;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        bullet = Resources.Load<Bullet>("Bullet");
    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    //IEnumerator BulletAnim()
    //{
    //    State = CharState.shoot;
    //
    //    yield return new WaitForSeconds(1);
    //    Shoot();
    //}

    private void Shoot()
    {
        Vector3 position = transform.position;
        position.y += positionBullet;
        position.x += sprite.flipX ? -1.0F : 1.0F;
        Bullet newBullet = Instantiate(bullet, position, bullet.transform.rotation) as Bullet;

        newBullet.Direction = newBullet.transform.right * (sprite.flipX ? -1.0F : 1.0F) ;
        
    }

    private void Update()
    {
        if (isGrounded) State = CharState.idle;
        if (Input.GetButton("Fire1")) State = CharState.shoot;
        if (Input.GetButton("Horizontal")) Run();
        if (isGrounded &&  Input.GetButtonDown("Jump")) Jump();
    }

    private void Run()
    {
        Vector3 direction = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, speed * Time.deltaTime);

        sprite.flipX = direction.x < 0.0F;
        if (isGrounded) State = CharState.run;
    }

    private void Jump()
    {
        rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        State = CharState.jump;
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.3F);

        isGrounded = colliders.Length > 1;
        if (!isGrounded) State = CharState.jump;
    }
}

public enum CharState
{
    idle,
    run,
    jump,
    shoot
}
