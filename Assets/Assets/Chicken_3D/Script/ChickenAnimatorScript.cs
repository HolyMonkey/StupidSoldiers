using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SmapleChicken {
public class ChickenAnimatorScript : MonoBehaviour
{
	Animator animator;
	private float chicken_speed = 1f;
	private float speed;
	private bool isRunning = false;
	private int hp;
	private GameObject hpSystem;
	private HPSystem hp_num;
	private GameObject textObj;
	private Text text;
	private bool to_stop = false;

	private CharacterController ctrl;
	private float gravity = 5.0f;
	private Vector3 moveDirection = Vector3.zero;

	private GameObject view_camera;

	void Start()
	{
		animator = this.GetComponent<Animator>();
		ctrl = this.GetComponent<CharacterController>();
		hp_num = FindObjectOfType<HPSystem>();
		
		textObj = GameObject.Find("AnimationState");

		view_camera = GameObject.Find("Camera");
	}
	void Update()
	{
		STATE_TEXT();
		CAMERA();

		hp = hp_num.HP_num;
		
		if(hp<=0)
		{
			if(!to_stop)
			{
				animator.CrossFade("down", 0.1f, 0, 0);
				animator.CrossFade("wing_down", 0.1f, 1, 0);
				to_stop = true;
			}
			else if(to_stop)
			{
				isRunning = false;
				animator.SetBool("to_move", false);
				// recovery
				if (Input.GetKeyDown(KeyCode.E))
				{
					animator.SetTrigger("jump");
					hp_num.HP_num = 1000;
					to_stop = false;
					moveDirection.y = 3.0f;
				}
			}
		}

		GRAVITY();
		if(hp>0 && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Stop"))
		{
			if(!animator.GetCurrentAnimatorStateInfo(0).IsTag("Action") 
					&& !animator.GetCurrentAnimatorStateInfo(0).IsTag("Damage"))
			{
				MOVE();
				JUMP();
				KEY_DOWN2();
			}
			KEY_DOWN();
		}
		KEY_UP();

		if(animator.GetCurrentAnimatorStateInfo(0).IsTag("Damage"))
		{
			animator.SetBool("during_damage", true);
		}
		else if(!animator.GetCurrentAnimatorStateInfo(0).IsTag("Damage"))
		{
			animator.SetBool("during_damage", false);
		}
	}
	//--------------------------------------------------------------------- CAMERA
	private void CAMERA ()
	{
		view_camera.transform.position = this.transform.position + new Vector3(0, 1, -3);
	}

	//--------------------------------------------------------------------- GRAVITY
	private void GRAVITY ()
	{
		if (ctrl.isGrounded)
		{
			animator.SetBool("to_landing", true);
			if(moveDirection.y < -0.5f) moveDirection.y = -0.5f;
		}
		else if (!ctrl.isGrounded)
		{
			animator.SetBool("to_landing", false);
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			animator.SetTrigger("to_flapping");
		}
		else if(animator.GetBool("to_flapping") && animator.GetCurrentAnimatorStateInfo(0).IsTag("Basis"))
		{
			animator.ResetTrigger("to_flapping");
		}
		if (Input.GetKey(KeyCode.W) && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Stop"))
		{
			if(this.transform.position.y < 0.5f)
			{
				moveDirection.y = 0.5f;
			}
			else{
				moveDirection.y = 0;
			}
		}
		else{
			moveDirection.y -= gravity * Time.deltaTime;
		}
		ctrl.Move(moveDirection * Time.deltaTime);
	}

	//--------------------------------------------------------------------- MOVE
	private void MOVE (){
		//------------------------------------------------------------ Speed
        if(isRunning)
        {
        	speed = chicken_speed * 2;
        }
		else {
			speed = chicken_speed;
		}
		animator.SetFloat("speed", speed);

        //------------------------------------------------------------ Foreward
        Vector3 velocity;
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
        	animator.SetBool("to_move", true);
            // velocity
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("move") || !ctrl.isGrounded)
            {
                velocity = this.transform.rotation * new Vector3(0, 0, speed);
                this.transform.position += velocity * Time.deltaTime;
            }
        }
        
        //------------------------------------------------------------ character rotation
        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.Rotate(Vector3.up, 2f);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.Rotate(Vector3.up, -2f);
        }
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
            {
            	animator.SetBool("to_move", true);
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
            {
            	animator.SetBool("to_move", true);
            }
            // rotate stop
            else if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow))
            {
            	animator.SetBool("to_move", false);
            }
        }
	}
	//--------------------------------------------------------------------- JUMP
	private void JUMP ()
	{
		if(!animator.IsInTransition(0))
		{
			if(!animator.GetCurrentAnimatorStateInfo(1).IsName("wing_flapping"))
			{
				if (Input.GetKeyDown(KeyCode.S))
				{
					animator.SetTrigger("jump");
					moveDirection.y = 3.0f;
				}
			}
		}
	}
	//--------------------------------------------------------------------- KEY_DOWN
	private void KEY_DOWN ()
	{
		// run
		if (Input.GetKeyDown(KeyCode.Z))
		{
			isRunning = true;
		}
		// crouch
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName("move"))
		{
			if (Input.GetKey(KeyCode.C))
			{
				animator.SetBool("to_crouch", true);
			}
		}
		// damage
		if (Input.GetKeyDown(KeyCode.Q))
		{
			animator.SetTrigger("damage");
		}
	}
	//--------------------------------------------------------------------- KEY_DOWN2
	private void KEY_DOWN2 ()
	{
		if (Input.GetKeyDown(KeyCode.X))
		{
			animator.SetTrigger("honk");
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			animator.SetTrigger("eat");
		}
		if (Input.GetKeyDown(KeyCode.A) && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Jump"))
		{
			animator.SetTrigger("peck");
		}
	}
	//--------------------------------------------------------------------- KEY_UP
	private void KEY_UP ()
	{
		// run
		if (Input.GetKeyUp(KeyCode.Z))
		{
			isRunning = false;
		}
		// crouch
		else if (Input.GetKeyUp(KeyCode.C))
		{
			animator.SetBool("to_crouch", false);
		}
		// move stop
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
        	animator.SetBool("to_move", false);
        }
        // rotate stop
        else if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
        	if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        	{
        		animator.SetBool("to_move", false);
            }
        }
	}
	//--------------------------------------------------------------------- STATE_TEXT
	private void STATE_TEXT ()
	{
		if(animator.GetCurrentAnimatorStateInfo(0).IsName("idle")){
			textObj.GetComponent<Text>().text = "idle";
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("move")){
			if(animator.GetFloat("speed") == 1){
				textObj.GetComponent<Text>().text = "walk";
			}
			else if(animator.GetFloat("speed") > 1){
				textObj.GetComponent<Text>().text = "run";
			}
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("flapping")){
			textObj.GetComponent<Text>().text = "flapping";
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("peck_flapping")){
			textObj.GetComponent<Text>().text = "peck_flapping";
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("peck")){
			textObj.GetComponent<Text>().text = "peck";
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("jump")){
			textObj.GetComponent<Text>().text = "jump";
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("crouch")){
			textObj.GetComponent<Text>().text = "crouch";
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("eat")){
			textObj.GetComponent<Text>().text = "eat";
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("honk")){
			textObj.GetComponent<Text>().text = "honk";
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("damage")){
			textObj.GetComponent<Text>().text = "damage";
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("damage_flapping")){
			textObj.GetComponent<Text>().text = "damage_flapping";
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("down")){
			textObj.GetComponent<Text>().text = "down";
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("recovery")){
			textObj.GetComponent<Text>().text = "recovery";
		}
	}
}
}


