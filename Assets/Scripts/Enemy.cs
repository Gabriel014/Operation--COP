using UnityEngine;
using System.Collections;

public class Enemy : MovingObject {

	public enum State{
		Agressive, Passive
	}

	public State current;
	public int playerDamage;
    public int visionRange;
    private int enemyDirection;

    private GameObject[] walls;
	private GameObject[] capturables;

	private Animator animator;
	private Transform target;
	private bool skipMove;
    private bool wallInPlace;
    public bool outOfSight = true;
	public AudioClip enemyAttack1;
	public AudioClip enemyAttack2;
	
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

		skipMove = false;
	}
    public void MoveEnemy()
	{
        wallInPlace = false;

        if (outOfSight)
			enemyDirection = Random.Range (1, 5);
			//Debug.Log (enemyDirection);
			switch (enemyDirection) {
			case 4: //down
				if (gameObject.transform.position.x == GameObject.Find ("Player").transform.position.x && gameObject.transform.position.y - GameObject.Find ("Player").transform.position.y <= visionRange) {
					outOfSight = false;
					if (current == State.Agressive && gameObject.tag != "Capturable") {
						MovingEnemyToKill ();
					} if(gameObject.tag == "Capturable") {
						MovingEnemyToFlee ();
					}
				}else {
                        walls = GameObject.FindGameObjectsWithTag("Coliders");
                        for (int i = 0; i < walls.Length; i++)
                        {
                            if (walls[i].transform.position == new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1))
                            {
                                wallInPlace = true;
                                break;
                            }
                        }
                        if (!wallInPlace) gameObject.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y - 1);
					outOfSight = true;
				}
				break;
			case 3: //left
                gameObject.transform.localScale = new Vector3 (1, 1); 
				if (gameObject.transform.position.y == GameObject.Find ("Player").transform.position.y && gameObject.transform.position.x - GameObject.Find ("Player").transform.position.x <= visionRange) {
					outOfSight = false;
					if (current == State.Agressive  && gameObject.tag != "Capturable") {
						MovingEnemyToKill ();
					} if(gameObject.tag == "Capturable") {
						MovingEnemyToFlee ();
					}
				} else {
                        walls = GameObject.FindGameObjectsWithTag("Coliders");
                        for (int i = 0; i < walls.Length; i++)
                        {
                            if (walls[i].transform.position == new Vector3(gameObject.transform.position.x - 1, gameObject.transform.position.y))
                            {
                                wallInPlace = true;
                                break;
                            }
                        }
                        if (!wallInPlace) gameObject.transform.position = new Vector3(gameObject.transform.position.x - 1, gameObject.transform.position.y);
                    outOfSight = true;
				}
				break;
			case 2: //up
                if (gameObject.transform.position.x == GameObject.Find("Player").transform.position.x && GameObject.Find("Player").transform.position.y - gameObject.transform.position.y <= visionRange)
                {
                    outOfSight = false;
                    if (current == State.Agressive && gameObject.tag != "Capturable")
                    {
                        MovingEnemyToKill();
                    }
                    if (gameObject.tag == "Capturable")
                    {
                        MovingEnemyToFlee();
                    }
                }
                else
                {
                    walls = GameObject.FindGameObjectsWithTag("Coliders");
                    for (int i = 0; i < walls.Length; i++)
                    {
                        if (walls[i].transform.position == new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1))
                        {
                            wallInPlace = true;
                            break;
                        }
                    }
                    if (!wallInPlace) gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1);
                    outOfSight = true;
                }
				break;
			case 1: //right
                gameObject.transform.localScale = new Vector3(-1, 1);
                if (gameObject.transform.position.y == GameObject.Find("Player").transform.position.y && GameObject.Find("Player").transform.position.x - gameObject.transform.position.x <= visionRange)
                {
                    outOfSight = false;
                    if (current == State.Agressive && gameObject.tag != "Capturable")
                    {
                        MovingEnemyToKill();
                    }
                    if (gameObject.tag == "Capturable")
                    {
                        MovingEnemyToFlee();
                    }
                }
                else
                {
                    walls = GameObject.FindGameObjectsWithTag("Coliders");
                    for (int i = 0; i < walls.Length; i++)
                    {
                        if (walls[i].transform.position == new Vector3(gameObject.transform.position.x + 1, gameObject.transform.position.y))
                        {
                            wallInPlace = true;
                            break;
                        }
                    }
                    if (!wallInPlace) gameObject.transform.position = new Vector3(gameObject.transform.position.x + 1, gameObject.transform.position.y);
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

		capturables = GameObject.FindGameObjectsWithTag ("Capturable");
		for (int i = 0; i < capturables.Length; i++) {
			if (capturables [i].transform.position.x <= 9) {
				GameManager.canExit = false;
			} else {
				GameManager.canExit = true;
			}
		}

		if (gameObject.transform.position.x == GameObject.Find ("Player").transform.position.x
			&& gameObject.transform.position.y == GameObject.Find ("Player").transform.position.y) {
			gameObject.transform.position = new Vector2 (gameObject.transform.position.x + 1, gameObject.transform.position.y + 1);
		}
	}
}
