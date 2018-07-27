using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetLanding : MonoBehaviour {

    public Player _player;
    [SerializeField]
    private GameObject _playerPlatformHelper;

    public float stompJumpBoost;
    public float stompJumpBoostOnX;


    public bool onPlatform;

	void Start () {
       // _player = gameObject.transform.parent.GetComponent<Player>();

	}
	
	// Update is called once per frame
	void Update () {
        //stompJumpBoostOnX = Random.Range(-8f, 8f);
	}


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            _player.isJumping = false;
            EffectSFXHandler.instance.PlayerLandSFX(transform.position);
        }

        else if (other.tag == "Platform")
        {
            _player.playerRB.interpolation = RigidbodyInterpolation2D.Extrapolate;
            onPlatform = true;
            _player.isJumping = false;
            EffectSFXHandler.instance.PlayerLandSFX(transform.position);
            _playerPlatformHelper.transform.parent = other.gameObject.transform;
            _player.isStuck = false;
        } 

        if (other.tag == "MonsterHead")
        {
            GameControl.instance.RewardShiningCoin(other.gameObject.transform.position, 0.8f );
            EffectSFXHandler.instance.PlayerLandSFX(other.gameObject.transform.position);
            AudioControl.instance.EnemySmashed();
            _player.playerRB.velocity = new Vector2(0, stompJumpBoost);
        }

       else if (other.tag == "BossHead")
       {
            Vector3 distanceBetween = (transform.position - other.gameObject.transform.position).normalized;

            if (_player.isCollidingWithGround == false)
            {
                _player.knockbackCounter = _player.knockbackTime;
                _player.playerRB.velocity = new Vector2(distanceBetween.x * stompJumpBoostOnX, stompJumpBoost);
            }
            else
            {
                _player.knockbackCounter = _player.knockbackTime;
                _player.playerRB.velocity = new Vector2(-distanceBetween.x * stompJumpBoostOnX, stompJumpBoost);
            }

       } 

        //-------LevelThreeEnd------

        else if (other.tag == "EscapeRamp")
        {
            //_player.Level3EndAnimation();
            _player.playerRB.velocity = new Vector2(0, 23);

        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Platform")
        {
            if (gameObject.activeInHierarchy)
            {
                onPlatform = false;
                _playerPlatformHelper.transform.parent = null;
                _player.playerRB.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
        }

    }


}
