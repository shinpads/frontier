using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	private const float AIR_SPEED_FACTOR = 0.6f;
	private const float ADS_SPEED_FACTOR = 0.5f;
	public Camera playerCamera;
	[SerializeField] private AudioClip jumpSound;
	[SerializeField] private AudioSource audioSource;
	private CharacterController characterController;
	private Player[] scoreboardData;
	private Vector3 rotationY, verticalMovement, horizontalMovement;
	private float rotationX = 0;
	private Vector3 characterVelocity;
	private float characterSpeed;
	private const float GRAVITY = 19f;
	private const float JUMP_AMOUNT = 6f;
	private const float SENSITIVITY = 1.2f;
	private const float ADS_SENSITIVITY = 0.7f;
	private const float SCOPED_SENSITIVITY = 0.1f;
	private float currentSensitivity;
	private const int RUN_SPEED = 2;
	private float accelerationValue = 85f;
	private float decelerationValue = -70.3f;
	private Vector3 acceleration = new Vector3(0, 0 ,0);
	private Vector3 velocity = new Vector3(0, 0 ,0);
	private float recoilAmount = 0f;
	private float recoilDecreaseAcceleration = 0.003f;
	private IEnumerator recoilCoroutine;
	private int isSprinting;
	private bool isAds;
	private bool isAirborne;
	private bool atCart;
	private bool canMove;
	private Teams userTeam;
	private Character player;
	private PlayerGUI gui;
	private GameController gameController;
	Texture2D pixel;
	Color pixelColor;

	void Start() {
		characterController = GetComponent<CharacterController>();
		Screen.lockCursor = true;
		Cursor.visible = false;
		characterSpeed = gameObject.GetComponent<Character>().getSpeed();
		player = gameObject.GetComponent<Character> ();
		gui = gameObject.GetComponentInChildren<PlayerGUI> ();
		gameController = GameObject.FindWithTag("Control").GetComponent<GameController>();
		pixelColor = Color.black;
		pixelColor.a = 0.5f;
		pixel = new Texture2D (1, 1);
		pixel.SetPixel (0, 0, pixelColor);
		pixel.Apply ();
		currentSensitivity = SENSITIVITY;
		isAds = false;
		canMove = true;
	}

	void Update() {
		rotationY = new Vector3(0f, Input.GetAxisRaw("Mouse X"), 0f) * currentSensitivity;
		rotationX -= Input.GetAxis ("Mouse Y") * currentSensitivity;
		//verticalMovement = Input.GetAxisRaw("Vertical");// * transform.forward;
		//horizontalMovement = Input.GetAxisRaw("Horizontal");// * transform.right;
		if (Input.GetKey(KeyCode.LeftShift) && !isAds) {
			isSprinting = 1;
			if (velocity.magnitude > 4) {
				playerCamera.gameObject.GetComponent<Animator>().SetBool("isSprinting", true);
			} else {
				playerCamera.gameObject.GetComponent<Animator>().SetBool("isSprinting", false);
			}
		}	else {
			isSprinting = 0;
			playerCamera.gameObject.GetComponent<Animator>().SetBool("isSprinting", false);
			}

		gameObject.transform.Rotate(rotationY);

		// CHARACTER MOVEMENT ------------------------------------------------------------------------

		// non-directioned values
		acceleration.x = (Input.GetAxisRaw("Vertical")) * accelerationValue;
		acceleration.z = (Input.GetAxisRaw("Horizontal")) * accelerationValue;
		if (acceleration.x == 0 && Mathf.Abs(velocity.x) < 0.4f) {
			velocity.x = 0;
		}
		if (acceleration.z == 0 && Mathf.Abs(velocity.z) < 0.4f) {
			velocity.z = 0;
		}
		if (Mathf.Abs(velocity.x) > 0) {
			acceleration.x += (Mathf.Abs(velocity.x) / velocity.x) * decelerationValue;
		}
		if (Mathf.Abs(velocity.z) > 0) {
			acceleration.z += (Mathf.Abs(velocity.z) / velocity.z) * decelerationValue;
		}
		// Debug.Log(velocity.x.ToString() + " " + velocity.z.ToString());
		velocity.x += acceleration.x * Time.deltaTime;
		velocity.z += acceleration.z * Time.deltaTime;

		// max velocity
		float maxSpeed = characterSpeed * (isAds ? ADS_SPEED_FACTOR : 1f) + (RUN_SPEED * isSprinting);
		// maxSpeed *= (isAirborne ? 0.6f : 1f);
		float speedToMaxSpeed = velocity.magnitude / maxSpeed;
		if (speedToMaxSpeed > 1) {
			velocity /= speedToMaxSpeed;
		}
		// aligning velocity to direction
		characterVelocity.x = (velocity.x * transform.forward).x + (velocity.z * transform.right).x;
		characterVelocity.z = (velocity.x * transform.forward).z + (velocity.z * transform.right).z;

		// ---------------------------------------------------------------------------------------------
		// Clamp camera angle
 		rotationX = Mathf.Clamp (rotationX, -90.0f, 90.0f);
    	Camera.main.transform.localRotation = Quaternion.Euler (rotationX, 0, 0);

		if (characterController.isGrounded) {
			characterVelocity.y = 0;
			if (isAirborne) {
				//characterVelocity.x = 0;
				//characterVelocity.z = 0;
				// StartCoroutine(resetAcceleration());
			}
			isAirborne = false;
		} else {
			if (!isAirborne) {
				isAirborne = true;
				characterVelocity.y = 0;
			}
		}

		if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded) {
			characterVelocity.y = JUMP_AMOUNT;
			isAirborne = true;
			audioSource.PlayOneShot(jumpSound);
		}
		else {
			characterVelocity.y -= GRAVITY * Time.deltaTime;
		}
 		if (canMove) {
			characterController.Move(characterVelocity * Time.deltaTime);
		}

	}

	public void setSensitivity(int state) {
		switch (state) {
		case 0:
			currentSensitivity = SENSITIVITY;
			break;
		case 1:
			currentSensitivity = ADS_SENSITIVITY;
			break;
		case 2:
			currentSensitivity = SCOPED_SENSITIVITY;
			break;
		}
	}

	private void OnGUI() {
		if (Input.GetKey (KeyCode.Tab)){
			userTeam = gameController.getUserTeam (player.getUserId());
			scoreboardData = userTeam.getTeamStats ();
		  float screenWidth = Screen.width;
		  float screenHeight = Screen.height;
			int playerCount = 0;
			string playerCell;
			GUI.DrawTexture (new Rect (screenWidth/8 - screenWidth/100, screenHeight/4, 3*screenWidth/4 + screenWidth/100, screenHeight/2), pixel);
			foreach (Player statLine in scoreboardData) {
				if (statLine == null) {
					playerCell = "";
				} else {
					playerCell = Global.CHARACTER_NAMES [playerCount] + "\n" + statLine.getUsername () + "\nKills: "	+ statLine.getStatLine ().kills + "\nAssists: " + statLine.getStatLine ().assists + "\nDeaths: " + statLine.getStatLine ().deaths + "\nGold Stolen" + statLine.getStatLine ().goldStolen;
				}
				GUI.Label(new Rect ((screenWidth/8) + (3*screenWidth/20 * playerCount), screenHeight/4, 3*screenWidth/20, screenHeight/2), playerCell);
		        playerCount++;
	      	}
	    }
  	}

	public void setSpeed (float speed) {
		characterSpeed = speed;
	}

	public void changeAdsState(bool state) {
		isAds = state;
	}
	private IEnumerator resetAcceleration() {
		decelerationValue = -160f;
		yield return new WaitForSeconds(0.05f);
		decelerationValue = -49.3f;
	}

	public float getSpeed() {
		return characterSpeed;
	}

	public void setAbilityToMove(bool ableToMove) {
  	canMove = ableToMove;
  }
	public void setRecoil(float recoil) {
		recoilAmount += recoil;
		rotationX -= recoil;
		if (recoilCoroutine != null) {
			StopCoroutine(recoilCoroutine);
		}
		recoilCoroutine = resetRecoil();
		StartCoroutine(recoilCoroutine);
	}
	private IEnumerator resetRecoil() {
		float decreaseAmount = 0.025f;
		while (recoilAmount > 0) {
			if (recoilAmount < decreaseAmount) {
				rotationX += recoilAmount;
				recoilAmount = 0;
			} else if (recoilAmount > 0) {
				rotationX += decreaseAmount;
				recoilAmount -= decreaseAmount;
			}
			yield return new WaitForSeconds(0.01f);
			decreaseAmount += recoilDecreaseAcceleration;
		}
	}
}
