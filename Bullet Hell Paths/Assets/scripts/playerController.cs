using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    //object variables
    Animator animator;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    public Transform floorCheck;

    //animation variables
    bool isGrounded;
    bool facingLeft = false;

    //attack variables
    private bool canAttack = true;
    public float attackSpeed = 1;
    public float attackRange = 1;
    public int attackDamage = 50;
    public Transform attackPos;

    //enemy variables
    public LayerMask enemies;

    //movement variables
    public float runSpeed = 6;
    public float jumpSpeed = 8;
    public float fallMultiplier = 2;
    public float lowJumpMultiplier = 2.5f;

    //counting frames be like
    private int timer = 1;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        //check if lizy is on the ground
        if(Physics2D.Linecast(transform.position, floorCheck.position, 1 << LayerMask.NameToLayer("ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        //right left movement
        if (Input.GetKey("d"))
        {
            
            if (isGrounded && !isPlaying("lizy attack"))
            {
                rigid.velocity = new Vector2(runSpeed, rigid.velocity.y);
                spriteRenderer.flipX = false;
                animator.Play("lizy run");
                facingLeft = false;
            }
            if (!isGrounded)
            {
                rigid.velocity = new Vector2(runSpeed * .7f, rigid.velocity.y);
            }   
        }
        else if(Input.GetKey("a"))
        {
            rigid.velocity = new Vector2(-runSpeed, rigid.velocity.y);
            if (isGrounded && !isPlaying("lizy attack"))
            {
                spriteRenderer.flipX = false;
                animator.Play("lizy running left");
                facingLeft = true;
            }
            if (!isGrounded)
            {
                rigid.velocity = new Vector2(-runSpeed * .7f, rigid.velocity.y);
            }
        }
        //idle
        else
        {
            if (isGrounded && !isPlaying("lizy attack"))
            {
                animator.Play("lizy idle");
                if (facingLeft)
                {
                    spriteRenderer.flipX = true;
                }
            }
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        //jump
        if (Input.GetKeyDown("space") && isGrounded)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, jumpSpeed);
            if (!isPlaying("lizy attack jump") && rigid.velocity.y >= 0)
            {
                animator.Play("lizy jump");
                if (facingLeft)
                {
                    spriteRenderer.flipX = true;
                }
            }
        }
        //allow the player to do a low jump
        else if (rigid.velocity.y > 0 && !Input.GetKey("space"))
        {
            rigid.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        //land
        else if (!isGrounded && rigid.velocity.y < 0)
        {
            //make the fall a little faster
            rigid.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            if (!isPlaying("lizy attack jump"))
            {
                animator.Play("lizy land");
                if (facingLeft)
                {
                    spriteRenderer.flipX = true;
                }
            }
            
        }
        //fast fall
        if (Input.GetKey("s") && !isGrounded)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, -15);
            if (!isPlaying("lizy attack jump"))
            {
                animator.Play("lizy land");
                if (facingLeft)
                {
                    spriteRenderer.flipX = true;
                }
            }
            
        }
        //attacking
        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            //Debug.Log(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + ", " + rigid.position.x);
            attack();
            if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > floorCheck.position.x)
            {
                rigid.velocity = new Vector2(4, rigid.velocity.y);

                spriteRenderer.flipX = false;
                facingLeft = false;
                if (isGrounded)
                {
                    animator.Play("lizy attack");
                }
                else
                {
                    animator.Play("lizy attack jump");
                }
            }
            else
            {
                rigid.velocity = new Vector2(-4, rigid.velocity.y);
                
                spriteRenderer.flipX = true;
                facingLeft = true;
                if (isGrounded)
                {
                    animator.Play("lizy attack");
                }
                else
                {
                    animator.Play("lizy attack jump");
                }
            }
            canAttack = false;
        }

        //timer for attackspeed
        if (!canAttack)
        {
            timer++;
            if(timer == 50 * attackSpeed)
            {
                canAttack = true;
                timer = 1;
            }
        }
    }

    //is an animation playing
    private bool isPlaying(string animName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

    //create a hitbox and deal damage to enemies
    private void attack()
    {
        Collider2D[] enemyToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemies);
        for(int i = 0; i < enemyToDamage.Length; i++)
        {
            enemyToDamage[i].GetComponent<bossOne>().takeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }
}
