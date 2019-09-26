using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class playerController : MonoBehaviour
{
    //object variables
    Animator animator;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    public Transform floorCheck;

    //animation variables
    bool isGrounded;
    public bool facingLeft = false;

    //attack variables
    public bool canAttack = true;
    public float attackSpeed = 1;
    private float timer;
    public float attackRange = 1;
    public int attackDamage = 5;
    public Transform attackPos;
    public Text attackSpeedDisplay;

    //enemy variables
    public LayerMask enemies;

    //movement variables
    public float runSpeed = 6;
    public float jumpSpeed = 8;
    public float fallMultiplier = 2;
    public float lowJumpMultiplier = 2.5f;

    //player stats
    public int health = 0;

    //damaging the player
    public bool canTakeDamage = true;
    public float damageBoostTimer = 0;
    public bool full = true;
    public Transform playerHealth;

    //game over variables
    public Image gameOverScreen;
    public Text gameOverText;
    public Text restartText;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        timer = attackSpeed;
    }

    private void Update()
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
            if (isGrounded && !isPlaying("lizy attack"))
            {
                rigid.velocity = new Vector2(-runSpeed, rigid.velocity.y);
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
        if (Input.GetKeyDown("s") && !isGrounded)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, -30);
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
        if (canAttack)
        {
            if ((Input.GetMouseButtonDown(0) && Camera.main.ScreenToWorldPoint(Input.mousePosition).x >= floorCheck.position.x) || Input.GetKeyDown("right"))
            {
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
                attack(facingLeft);
                canAttack = false;
            }
            else if((Input.GetMouseButtonDown(0) && Camera.main.ScreenToWorldPoint(Input.mousePosition).x < floorCheck.position.x) || Input.GetKeyDown("left"))
            {
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
                attack(facingLeft);
                canAttack = false;
            }
        }

        //timer for attackspeed
        if (!canAttack)
        {
            attackSpeedDisplay.text = timer.ToString("F2");
            timer-= Time.deltaTime;
            if(timer <= 0)
            {
                canAttack = true;
                timer = attackSpeed;
                attackSpeedDisplay.text = "0.00";
            }
        }

        //timer for damage boost
        if (!canTakeDamage)
        {
            Color c = spriteRenderer.material.color;
            if (full)
            {
                c.a = .3f;
            }
            else
            {
                c.a = 1;
            }
            spriteRenderer.material.color = c;
            full = !full;

            damageBoostTimer += Time.deltaTime;
            if (damageBoostTimer >= 3)
            {
                canTakeDamage = true;
                damageBoostTimer = 0;
                c.a = 1;
                spriteRenderer.material.color = c;
                full = true;
            }
        }

        //reset game
        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    //is an animation playing
    private bool isPlaying(string animName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f;
    }

    //create a hitbox and deal damage to enemies
    private void attack(bool facingLeft)
    {
        if (!facingLeft)
        {
            Collider2D[] enemyToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemies);
            for (int i = 0; i < enemyToDamage.Length; i++)
            {
                enemyToDamage[i].GetComponent<bossOne>().takeDamage(attackDamage);
            }
        }
        else
        {
            Collider2D[] enemyToDamage = Physics2D.OverlapCircleAll(new Vector3(attackPos.position.x - .85f, attackPos.position.y), attackRange, enemies);
            for (int i = 0; i < enemyToDamage.Length; i++)
            {
                enemyToDamage[i].GetComponent<bossOne>().takeDamage(attackDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
        Gizmos.DrawWireSphere(new Vector3(attackPos.position.x - .85f, attackPos.position.y), attackRange);
    }

    //make the player take damage if they are hit by an attack
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.ToString() != "boss1" && collision.isTrigger)
        {
            if (canTakeDamage && health < 5)
            {
                playerHealth.GetChild(health).GetComponent<SpriteRenderer>().material.color = new Color(0,0,0,0);
                health++;
                canTakeDamage = false;
            }
            if (health > 4)
            {
                gameOver();
            }
        }
    }

    private void gameOver()
    {
        gameOverScreen.color = new Color(0, 0, 0, 1);
        gameOverText.text = "Game Over";
        gameOverText.color = new Color(1,1,1,1);
        restartText.text = "press r to restart";
        restartText.color = new Color(1,1,1,1);
        //stop music and sounds whenever i decide to add those to the game
    }
}
