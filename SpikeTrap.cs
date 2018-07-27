using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour {

    [SerializeField]
    private GameObject _spikeTrap;

    /// <summary>
    /// Created an empty game object to set new 
    /// transform rotation
    /// </summary>
    public Transform targetTrap;
    /// <summary>
    /// created a new Quaternion.euler to set 
    /// </summary>
    private Quaternion returnTrap;

    public bool isTriggered = false;

    public float rotateTime;
    public float returnTrapRotation;

    public

	void Start () {
        returnTrap = Quaternion.Euler(0, 0, returnTrapRotation);
    }
	
	// Update is called once per frame
	void Update () {

        if (isTriggered == true)
        {
            ActivateTrap();
        }

        else
        {
           ResetTrap();
        }
    }


    /// <summary>
    /// 
    /// </summary>
    private void ActivateTrap()
    {
        _spikeTrap.transform.rotation = Quaternion.Lerp(_spikeTrap.transform.rotation, targetTrap.rotation, rotateTime * Time.deltaTime);
    }

    private void ResetTrap()
    {
        _spikeTrap.transform.rotation = Quaternion.Lerp(_spikeTrap.transform.rotation, returnTrap, rotateTime * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            isTriggered = true;

        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            isTriggered = false;
        }
    }

}
