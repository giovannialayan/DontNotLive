using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    bool isGrounded;
    bool facingLeft = false;

    public Transform floorCheck;

    private int attackSpeed = 1;
    private bool canAttack = true;

    public float runSpeed = 6;
    public float jumpSpeed = 8;

    private int timer = 1;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        Debug.Log(Time.fixedDeltaTime);

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
            rigid.velocity = new Vector2(runSpeed, rigid.velocity.y);
            if (isGrounded && !isPlaying("lizy attack"))
            {
                spriteRenderer.flipX = false;
                animator.Play("lizy run");
                facingLeft = false;
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
        //land
        else if (!isGrounded && rigid.velocity.y < 0)
        {
            if (!isPlaying("lizy attack jump"))
            {
                animator.Play("lizy land");
                if (facingLeft)
                {
                    spriteRenderer.flipX = true;
                }
            }
            
        }
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

        //timer for
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
}
