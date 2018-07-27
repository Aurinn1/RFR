using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour {

    public Rigidbody2D playerRB;
    private SpriteRenderer _playerSR;
    private Animator _anim;
    public LayerMask whatIsGround;      //for single jump
    public Transform feet;              //for single jump


    [SerializeField]
    private GameObject _leftSparkle;
    [SerializeField]
    private GameObject _rightSparkle;

    [SerializeField]
    private GameObject playerHead;
    [SerializeField]
    private GameObject playerFeet;
    private FeetLanding _feetLanding;


    public bool facingRight = true;
    public bool isJumping = false;
    public bool isGrounded = true;      //for single jump
    public bool canDoubleJump = true;   // for DOUBLE jump
    public bool SFXOn = true;
    public bool isStuck;
    [SerializeField]
    private bool _ableToFire = false;

    public bool isCollidingWithGround;


    private float canFire;
    private float fireRate = 0.45f;

    //------------Movement Related----------
    public float playerSpeed;
    private float playerActiveSpeed;
    private float jumpPower = 550f;
    public float speedMultiplier;
    public bool canInteract;
    //------------------------------------


    //----knockback effect related----------
    public float knockbackCounter;
    public float knockbackTime;
    public float kbForceX, kbForceY;
    public bool isInvincible;
    public float invincibilityCounter;
    public float invincibilityTime = 1f;
    //--------------------------------------

    public float boxWidth, boxHeight;   //for single jump
    public float delayForDoubleJump;    //for DOUBLE jump

    public Vector3 lifeSpentPosition;
    public Vector3 respawnPosition;


    // Use this for initialization
    void Start() {

        
        playerRB = GetComponent<Rigidbody2D>();
        _playerSR = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();

        if(playerFeet != null)
        {
            _feetLanding = FindObjectOfType<FeetLanding>();
        }


        respawnPosition = transform.position;
        lifeSpentPosition = transform.position;

        playerActiveSpeed = playerSpeed;
        canInteract = true;
    }

    // Update is called once per frame
    void Update() {


        if (Time.timeScale == 0)
        {
            return;
        }

        MovementAndKnockBackSystem();
        ShowFalling();

        //for single jump
        isGrounded = Physics2D.OverlapBox(feet.position, new Vector2(boxWidth, boxHeight), 360f, whatIsGround);

        //To make player(head) hit the crates and hidden stones from below
        if (playerRB.velocity.y > 0 )
        {
            playerHead.SetActive(true);
        }

        else
        {
            playerHead.SetActive(false);
        }
        //-------------------------------------------------------------



        //-------------------level3 end coroutine----------------------



    }

    private void FixedUpdate()
    {

    }

    public void Shoot()
    {

        if (Input.GetKeyDown(KeyCode.F)) // CrossPlatformInputManager.GetButtonDown("Fire"))
        {
            if (_ableToFire == true)
            {

                if (transform.localScale.x > 0 && Time.time > canFire)
                {
                    AudioControl.instance.FiringBullet(transform.position);
                    canFire = Time.time + fireRate;
                    Instantiate(_rightSparkle, transform.position + new Vector3(1, 0, 0), Quaternion.identity);
                }

                else if (transform.localScale.x < 0 && Time.time > canFire)
                {
                    AudioControl.instance.FiringBullet(transform.position);
                    canFire = Time.time + fireRate;
                    Instantiate(_leftSparkle, transform.position + new Vector3(-1, 0, 0), Quaternion.identity);
                }
            }
        }
    }

    public void Movement()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        //float horizontalMove = CrossPlatformInputManager.GetAxisRaw("Horizontal"); //for crossplatform

        if(_feetLanding.onPlatform == true)
        {
            playerActiveSpeed = playerSpeed * speedMultiplier;
        }

        else
        {
            playerActiveSpeed = playerSpeed;
        }
        playerRB.velocity = new Vector2(horizontalMove * playerActiveSpeed, playerRB.velocity.y);

        if (isJumping == false)
        {
            if (Mathf.Abs(playerRB.velocity.x) > 0)
            {
                _anim.SetInteger("State", 1);
            }
            else
            {
                StopMoving();
                _anim.SetInteger("State", 0);
            }
        }
        FlipPlayer(horizontalMove);//(horizontalMove);

    }

    public void StopMoving() {

        playerRB.velocity = new Vector2(0, playerRB.velocity.y);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(feet.position, new Vector3(boxWidth, boxHeight, 0)); // for single jump
    }

    public void Jump()
    {
        if (isGrounded == true)
        {
            isJumping = true;
            playerRB.AddForce(new Vector2(playerRB.velocity.x, jumpPower));
            _anim.SetInteger("State", 2);
            AudioControl.instance.PlayerJumping(transform.position);
            Invoke("EnableDoubleJump", delayForDoubleJump);  // for DOUBLE jump
        }
        if (canDoubleJump == true && isGrounded == false)  // for DOUBLE jump
        {
            playerRB.velocity = Vector2.zero;
            playerRB.AddForce(new Vector2(0, jumpPower));
            _anim.SetInteger("State", 2);
            AudioControl.instance.PlayerJumping(transform.position);
            canDoubleJump = false;
        }

    }

    public void EnableDoubleJump() //for DOUBLE jump
    {
        canDoubleJump = true;
    }

    public void ShowFalling()
    {
        if (playerRB.velocity.y < 0)
        {
            _anim.SetInteger("State", 3);
        }
    }

    private void FlipPlayer(float horizontal)
    {
        Vector2 theScale = transform.localScale;


        if (horizontal > 0 && facingRight == false || horizontal < 0 && facingRight == true)
        {
            facingRight = !facingRight;
            theScale.x *= -1;
            transform.localScale = theScale;     
        }
    }


    public void MovementAndKnockBackSystem()
    {
         if (knockbackCounter <= 0 && canInteract == true)
         {
             Movement();
             Shoot();

             if (Input.GetKeyDown(KeyCode.Space)) // ||CrossPlatformInputManager.GetButtonDown("Jump")) )
             {
                 Jump();
             }
         }

         if (knockbackCounter >= 0)
         {
             invincibilityCounter = invincibilityTime;
             knockbackCounter -= Time.deltaTime;
         } 


        if (isInvincible == true)
        {    
            invincibilityCounter -= Time.deltaTime;
            gameObject.layer = 13;

            if (invincibilityCounter <= 0)
            {
                isInvincible = false;
                _playerSR.color = Color.white;
                gameObject.layer = 11;
            }
        }
    }




    private void OnCollisionEnter2D(Collision2D other)
    {
        //Vector2 distanceBetween = (transform.position - other.gameObject.transform.position).normalized;
        float  distanceBetween = (transform.position - other.gameObject.transform.position).normalized.x;

        if (other.gameObject.tag == "Enemy")
        {

            if ((GameControl.instance.playerHealth > 1) && (isInvincible == false))
            {
                _anim.SetInteger("State", -1);
            }

            GameControl.instance.HealthSystem(1);
            GameControl.instance.StartCoroutine(GameControl.instance.OnHitRedAndWhiteCoroutine(gameObject));


            knockbackCounter = knockbackTime;
            isInvincible = true;

            playerRB.velocity = Vector2.zero;
            playerRB.AddForce(new Vector2(distanceBetween * kbForceX, kbForceY));

        }

        else if (other.gameObject.tag == "LethalEnemy")
        {

            if ((GameControl.instance.playerHealth > 2) && (isInvincible == false))
            {
                    _anim.SetInteger("State", -1);
            }

            GameControl.instance.HealthSystem(2);
            GameControl.instance.StartCoroutine(GameControl.instance.OnHitRedAndWhiteCoroutine(gameObject));


            knockbackCounter = knockbackTime;
            isInvincible = true;

            playerRB.velocity = Vector2.zero;
            playerRB.AddForce(new Vector2(distanceBetween * kbForceX, kbForceY));

        }

        else if (other.gameObject.tag == "Ground")
        {
            isCollidingWithGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == "Ground")
        {
            isCollidingWithGround = false;
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        //Vector2 distanceBetween = (transform.position - other.gameObject.transform.position).normalized;
        float distanceBetween = (transform.position - other.gameObject.transform.position).normalized.x;

        if (SFXOn == true)
        {
            if (other.tag == "Coin")
            {
                AudioControl.instance.CoinPickUp(transform.position);
                EffectSFXHandler.instance.CoinPickUpSFX(other.gameObject.transform.position);
                GameControl.instance.UpdateCoinCount(1);
            }

            else if (other.tag == "ShiningCoin")
            {
                AudioControl.instance.CoinPickUp(other.gameObject.transform.position);
                EffectSFXHandler.instance.CoinPickUpSFX(other.gameObject.transform.position);
                GameControl.instance.UpdateCoinCount(5);
                other.gameObject.SetActive(false);
            }

            else if (other.tag == "PowerUpM" || other.tag == "PowerUpM2")
            {
                _ableToFire = true;
                AudioControl.instance.PowerUpPicking(other.gameObject.transform.position);
                EffectSFXHandler.instance.PowerUpPickUpSFX(other.gameObject.transform.position);
                other.gameObject.SetActive(false);
            }

            else if (other.tag == "Water")
            {
                GameControl.instance.playerDrowning = true;   
                AudioControl.instance.FellIntoWater(transform.position);
                EffectSFXHandler.instance.WaterSplashing(transform.position);
                GameControl.instance.playerLives--;

                GameControl.instance.CheckForRespawn();
            }

            else if (other.tag == "Enemy")
            {

                if ((GameControl.instance.playerHealth > 1) && (isInvincible == false))
                {
                    _anim.SetInteger("State", -1);
                }
                GameControl.instance.HealthSystem(1);
                GameControl.instance.StartCoroutine(GameControl.instance.OnHitRedAndWhiteCoroutine(gameObject));


                knockbackCounter = knockbackTime;
                isInvincible = true;

                playerRB.velocity = Vector2.zero;
                playerRB.AddForce(new Vector2(distanceBetween * kbForceX, kbForceY));

            }

            else if (other.tag == "LethalEnemy")
            {

                if ((GameControl.instance.playerHealth > 2) && (isInvincible == false))
                {
                    _anim.SetInteger("State", -1);
                }
                GameControl.instance.HealthSystem(2);
                GameControl.instance.StartCoroutine(GameControl.instance.OnHitRedAndWhiteCoroutine(gameObject));

                knockbackCounter = knockbackTime;
                isInvincible = true;

                playerRB.velocity = Vector2.zero;
                playerRB.AddForce(new Vector2(distanceBetween * kbForceX, kbForceY));
            }
        }

        if (other.tag == "GoldenKey")
        {
            GameControl.instance.ShowLever();
        }

        else if (other.gameObject.tag == "BlueKeyHole")
        {
            GameControl.instance.UnlockBlueLock();
        }

        else if (other.gameObject.tag == "GreenKeyHole")
        {
            GameControl.instance.UnlockGreenLock();
        }

        else if (other.gameObject.tag == "Killplane")
        {
            AudioControl.instance.PlayerDied(transform.position);
            GameControl.instance.playerLives--;
            GameControl.instance.CheckForRespawn();
        }

        //----------CheckPoint-----------------

        else if (other.tag == "Checkpoint")
        {
            respawnPosition = other.gameObject.transform.position;
        }


        //----------Text PopUps----------------


        else if (other.tag == "WiseFrog")
        {
            TextControl.instance.HelpCat();
        }

        else if (other.tag == "SignHome")
        {
            TextControl.instance.Home();
        }

        else if (other.tag == "SignBeware")
        {
            TextControl.instance.Beware();
        }

        else if (other.tag == "SignDanger")
        {
            TextControl.instance.Danger();
        }

        else if (other.tag == "Cat")
        {
            TextControl.instance.CatCry();
        }

        else if (other.tag == "SignWatchStep")
        {
            TextControl.instance.WatchYourStep();
        }

        else if (other.tag == "SignGetYourWeapon")
        {
            TextControl.instance.GetYourWeapon();
        }

        else if (other.tag == "SnowmanHelp")
        {
            TextControl.instance.SnowmanHelp();
        }

        else if (other.tag == "PowerUpM")
        {
            TextControl.instance.HowToFire();
        }
    }


    public void Level3EndAnimation()
    {

            StartCoroutine(Level3EndCoroutine());


    }

    IEnumerator Level3EndCoroutine()
    {
        canInteract = false;
        yield return new WaitForSeconds(0.45f);

        playerRB.velocity = Vector2.zero;
        _anim.SetInteger("State", 0);
        yield return new WaitForSeconds(1.5f);

        _anim.SetInteger("State", 1);
        playerRB.velocity = new Vector2(7, 0);
        yield return new WaitForSeconds(2);
        GameControl.instance.LevelComplete();  

    }


}
