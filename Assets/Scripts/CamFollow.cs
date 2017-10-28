using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * //BEN SPURR 
 * CLEARLY UNUSED 
 * 
*/

public class CamFollow : MonoBehaviour 
{

	//private Vector2 velocity;

	//public float offsetY;
	//public float offsetX;
	//public float fallOffsetSpeed=0.001f;
	//public float smoothTimeX;
	//public float smoothTimeY;

 //   public bool isRight;
 //   public float CamSwitchDamp = 0.7f;

 //   public bool isGrounded;

 //   private float storeOffsetY;
 //   private float storeOffsetX;



 //   public GameObject player;

	//Rigidbody playerRigidbody;

 //   PlayerController playerController;


	//void Start () 
	//{
	//	player = GameObject.FindGameObjectWithTag("Player");
	//	playerRigidbody = player.GetComponent<Rigidbody>();

 //       storeOffsetY = offsetY;
 //       storeOffsetX = offsetX;

 //       playerController = player.GetComponent<PlayerController>();
 //   }


	//void FixedUpdate () 
	//{
	//	float posX = Mathf.SmoothDamp (transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);

	//	float posY = Mathf.SmoothDamp (transform.position.y, player.transform.position.y, ref velocity.y, smoothTimeY);

	//	transform.position = new Vector3(posX + offsetX, posY + offsetY, transform.position.z);

 //       this.isRight = playerController.isRight;
 //       this.isGrounded = playerController.onTheGround;

	//	if (playerRigidbody.velocity.y < -1f && !isGrounded)
	//		offsetY = offsetY + (playerRigidbody.velocity.y * fallOffsetSpeed);
	//	else
	//		offsetY = storeOffsetY;


 //       if (isRight)
 //       {
 //           offsetX = Mathf.Lerp(offsetX, storeOffsetX, Time.deltaTime * CamSwitchDamp);
 //       }
 //       else
 //           offsetX = Mathf.Lerp(offsetX, -storeOffsetX, Time.deltaTime * CamSwitchDamp);
 //   }

}