using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadSmash : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Breakable")
        {
            AudioControl.instance.BreakCrates(transform.position);
            EffectSFXHandler.instance.CrateBreaking(other.gameObject.transform.parent.position);
            gameObject.transform.parent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Destroy(other.gameObject.transform.parent.gameObject);
        }
    }
}
