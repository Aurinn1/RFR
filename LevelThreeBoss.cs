using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Falls down from sky
/// Jumps after taking some damage to 
/// a random location
/// </summary>
public class LevelThreeBoss : MonoBehaviour {

    public Rigidbody2D _bossRB;
    private Animator _anim;
    public GameObject healthSliderObj;
    public Slider healthSlider;
    public SpriteRenderer bossSR;

    private Player _player;
    private GameObject _playerObj;




    //----------Reset---------
    private Vector3 resetPosition;
    //-------------------------------

    private float distanceBetween;
    public bool facingRight;





    //---------ThirdPhase---------
    public int startSpawningThreshold;

    public GameObject spinSaw;
    public GameObject dropLittleSaw;
    public bool canSpawn;
    public Transform dropPoint1, dropPoint2;
    private float dropFrequency;
    public float dropFrequencyThreshold;
    public float dropMoreFrequentlyThreshold;
    //----------------------------


    //-----For Jumping Pattern------
    public int changePatternThreshold;

    public bool canJump;
    private float jumpCooldown;
    public float jumpCountDownerThreshold;
    public float jumpPower;
    public float multiplier;
    //------------------------------


    //---------Movemenet---------
    public float speed;
    public bool canMove;
    //-------------------------


    //-------BossHealth-------
    public int bossHealth;
    public int bossHealthThreshold;
    //------------------------

	void Start () {
        _bossRB = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        bossSR = GetComponent<SpriteRenderer>();

        _player = FindObjectOfType<Player>();
        _playerObj = _player.gameObject;

        resetPosition = transform.position;

        canJump = false;
        jumpCooldown = jumpCountDownerThreshold;
        canMove = false;

        bossHealth = bossHealthThreshold;

        dropFrequency = dropFrequencyThreshold;

        healthSlider.maxValue = bossHealthThreshold;
 
    }
	
	
	void Update () {

        if(Time.timeScale == 0)
        {
            return;
        }

        if(healthSliderObj != null)
        {
            healthSlider.value = bossHealth;
        }



        FlipTheBoss();

        if(bossHealth <= 0)
        {
            Destroy(this.gameObject);
        }


//------------BOSS PHASES--------------
        if (canMove == true)
        {
            MovePattern();
        }

        if (bossHealth < changePatternThreshold)
        {
            canMove = false;
            canJump = true;
            jumpCooldown -= Time.deltaTime;

        }

        if (canJump == true)
        {
            Phase2JumpAction();
        }

        if(bossHealth <= startSpawningThreshold)
        {

            canSpawn = true;
            dropFrequency -= Time.deltaTime;
            Phase3Spawn();
        }


//----------------------------------


        if (_playerObj.activeInHierarchy == false)
        {
            Invoke("OnPlayerDeathReset", 3.2f);
        }

        if(Mathf.Abs(_bossRB.velocity.x) > 0 && _bossRB.velocity.y == 0 && canMove == true)
        {
            _anim.SetBool("isMoving", true);
        }
        else
        {
            _anim.SetBool("isMoving", false);
        }


        if(bossHealth <= 0)
        {
            EffectSFXHandler.instance.EnemyExplosion(transform.position);
            AudioControl.instance.EnemyExplodes(transform.position);
        }

    }


    /*-------initial contact/BossTrigger----
    private void OnBecameVisible()
    {
        healthSliderObj.SetActive(true);
        _bossRB.gravityScale = 3;
        canMove = true;
    } */


    private void Phase2JumpAction()
    {
        if(jumpCooldown <= 0)
        {
            jumpCooldown = jumpCountDownerThreshold;
            distanceBetween = _playerObj.transform.position.x - transform.position.x;

            if(Mathf.Abs(distanceBetween) < 20)
            {
                _bossRB.AddForce(new Vector2(distanceBetween * multiplier, jumpPower));
            }
            else
            {
                _bossRB.AddForce(new Vector2(distanceBetween * multiplier * 0.6f, jumpPower));
            }
        }

    }

    private void Phase3Spawn()
    {
        if(canSpawn == true)
        {
            spinSaw.SetActive(true);

            if (dropFrequency <= 0)
            {
                Instantiate(dropLittleSaw, new Vector3(Random.Range(dropPoint1.position.x, dropPoint2.position.x), dropPoint1.position.y, dropPoint1.position.z), Quaternion.identity);

                if (bossHealth <= startSpawningThreshold && bossHealth > startSpawningThreshold * 0.5f )
                {
                    dropFrequency = dropFrequencyThreshold;
                }

                else if (bossHealth <= startSpawningThreshold * 0.5f)
                {
                    dropFrequency = dropMoreFrequentlyThreshold; 
                }
            }
        }
    }



    private void FlipTheBoss()
    {
        distanceBetween = _playerObj.transform.position.x - transform.position.x;

        Vector3 theScale = transform.localScale;

        if(distanceBetween > 0 && facingRight == false || distanceBetween <= 0 && facingRight == true)
        {
            facingRight = !facingRight;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Bullet")
        {
            bossHealth--;
            healthSlider.value = bossHealth;
        }
    }

    private void MovePattern()
    {
        Vector3 direction = (_playerObj.transform.position - transform.position).normalized;

        _bossRB.velocity = new Vector2(direction.x * speed * Time.deltaTime, _bossRB.velocity.y);
        
    }

    private void OnPlayerDeathReset()
    {
        _bossRB.gravityScale = 0;
        canJump = false;
        canMove = false;
        transform.position = resetPosition;
        _bossRB.velocity = Vector2.zero;
        bossHealth = bossHealthThreshold;
        healthSliderObj.SetActive(false);



        jumpCooldown = jumpCountDownerThreshold;

        canSpawn = false;
        spinSaw.SetActive(false);
        dropFrequency = dropFrequencyThreshold;

        AudioControl.instance.BGMusic();


    }


}
