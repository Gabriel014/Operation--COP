using UnityEngine;
using System.Collections;

public class Enemy : MovingObject {

	public enum State{
		Agressive, Passive
	}

	public State current;
	public int playerDamage;
    public int visionRange;
	public bool agressive;
    private int enemyDirection;

	private Animator animator;
	private Transform target;
	private bool skipMove;
    public bool outOfSight = true;
	public AudioClip enemyAttack1;
	public AudioClip enemyAttack2;

    public Sprite spriteDown, spriteLeft, spriteUp, spriteRight;
	
	protected override void Start () {
		GameManager.instance.AddEnemyToList (this);
		animator = GetComponent<Animator> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		base.Start ();
		}

	protected override void AttemptMove <T> (int xDir, int yDir)
	{
		if (skipMove) {
			skipMove = false;
			return;
		}

		base.AttemptMove<T> (xDir, yDir);

		skipMove = true;
	}
    public void MoveEnemy()
	{
		if (outOfSight)
			enemyDirection = Random.Range (1, 5);
			Debug.Log (enemyDirection);
			switch (enemyDirection) {
			case 4: //down
				gameObject.GetComponent<SpriteRenderer> ().sprite = spriteDown;
				if (gameObject.transform.position.x == GameObject.Find ("Player").transform.position.x && gameObject.transform.position.y - GameObject.Find ("Player").transform.position.y <= visionRange) {
					outOfSight = false;
					if (current == State.Agressive && gameObject.tag != "Capturable") {
						MovingEnemyToKill ();
					} if(gameObject.tag == "Capturable") {
						MovingEnemyToFlee ();
					}
				}else {
					if (gameObject.transform.position.y > 0) {
						gameObject.transform.position = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y - 1);
					} 
					outOfSight = true;
				}
				break;
			case 3: //left
				gameObject.GetComponent<SpriteRenderer> ().sprite = spriteLeft;
				if (gameObject.transform.position.y == GameObject.Find ("Player").transform.position.y && gameObject.transform.position.x - GameObject.Find ("Player").transform.position.x <= visionRange) {
					outOfSight = false;
					if (current == State.Agressive  && gameObject.tag != "Capturable") {
						MovingEnemyToKill ();
					} if(gameObject.tag == "Capturable") {
						MovingEnemyToFlee ();
					}
				} else {
					if (gameObject.transform.position.x > 0) {
						gameObject.transform.position = new Vector2 (gameObject.transform.position.x - 1, gameObject.transform.position.y);
					} 
					outOfSight = true;
				}
				break;
			case 2: //up
				gameObject.GetComponent<SpriteRenderer> ().sprite = spriteUp;
				if (gameObject.transform.position.x == GameObject.Find ("Player").transform.position.x && GameObject.Find ("Player").transform.position.y - gameObject.transform.position.y <= visionRange) {
					outOfSight = false;
					if (current == State.Agressive  && gameObject.tag != "Capturable") {
						MovingEnemyToKill ();
					} if(gameObject.tag == "Capturable") {
						MovingEnemyToFlee ();
					}
				} else {
					if (gameObject.transform.position.y < 7) {
						gameObject.transform.position = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y + 1);
					}
					outOfSight = true;
				}
				break;
			case 1: //right
				gameObject.GetComponent<SpriteRenderer> ().sprite = spriteRight;
				if (gameObject.transform.position.y == GameObject.Find ("Player").transform.position.y && GameObject.Find ("Player").transform.position.x - gameObject.transform.position.x <= visionRange) {
					outOfSight = false;
					if (current == State.Agressive  && gameObject.tag != "Capturable") {
						MovingEnemyToKill ();
					} if(gameObject.tag == "Capturable") {
						MovingEnemyToFlee ();
					}
				} else {
					if (gameObject.transform.position.x < 7) {
						gameObject.transform.position = new Vector2 (gameObject.transform.position.x + 1, gameObject.transform.position.y);
					}
					outOfSight = true;
				}
				break;
			}
		}

	public void MovingEnemyToKill()
	{
        Debug.Log("Seeing Enemy AGRESSIVE");
		int xDir = 0;
		int yDir = 0;

		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
			yDir = target.position.y > transform.position.y ? 1 : -1;
		else
			xDir = target.position.x > transform.position.x ? 1 : -1;

		AttemptMove <Player> (xDir, yDir);
	}

	public void MovingEnemyToFlee()
	{
		Debug.Log("Seeing Enemy FLEE");
		int xDir = 0;
		int yDir = 0;

		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
			yDir = target.position.y < transform.position.y ? 1 : -1;
		else
			xDir = target.position.x < transform.position.x ? 1 : -1;

		AttemptMove <Player> (xDir, yDir);
	}

	protected override void OnCantMove <T> (T component)
	{
		Player hitPlayer = component as Player;

		hitPlayer.LoseFood (playerDamage);

		animator.SetTrigger ("enemyAttack");

		SoundManager.instance.RandomizeSfx (enemyAttack1, enemyAttack2);
	}

	void Update() {
		if (gameObject.transform.position.x == GameObject.Find ("Player").transform.position.x
			&& gameObject.transform.position.y == GameObject.Find ("Player").transform.position.y) {
			gameObject.transform.position = new Vector2 (gameObject.transform.position.x + 1, gameObject.transform.position.y + 1);
		}
	}
}
