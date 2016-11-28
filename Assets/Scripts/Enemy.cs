using UnityEngine;
using System.Collections;

public class Enemy : MovingObject {

	public int playerDamage;
    public int visionRange;
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
        if (outOfSight) enemyDirection = Random.Range(1, 5);
        Debug.Log(enemyDirection);
        switch (enemyDirection)
        {
            case 4: //down
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteDown;
                if (gameObject.transform.position.x == GameObject.Find("Player").transform.position.x && gameObject.transform.position.y - GameObject.Find("Player").transform.position.y <= visionRange)
                {
                    MovingEnemy();
                    outOfSight = false;
                }
                else outOfSight = true;
                break;
            case 3: //left
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteLeft;
                if (gameObject.transform.position.y == GameObject.Find("Player").transform.position.y && gameObject.transform.position.x - GameObject.Find("Player").transform.position.x <= visionRange)
                {
                    outOfSight = false;
                    MovingEnemy();
                }
                else outOfSight = true;
                break;
            case 2: //up
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteUp;
                if (gameObject.transform.position.x == GameObject.Find("Player").transform.position.x && GameObject.Find("Player").transform.position.y - gameObject.transform.position.y <= visionRange)
                {
                    outOfSight = false;
                    MovingEnemy();
                }
                else outOfSight = true;
                break;
            case 1: //right
                gameObject.GetComponent<SpriteRenderer>().sprite = spriteRight;
                if (gameObject.transform.position.y == GameObject.Find("Player").transform.position.y && GameObject.Find("Player").transform.position.x - gameObject.transform.position.x <= visionRange)
                {
                    outOfSight = false;
                    MovingEnemy();
                }
                else outOfSight = true;
                break;
        }
    }

	public void MovingEnemy()
	{
        Debug.Log("Seeing Enemy");
		int xDir = 0;
		int yDir = 0;

		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
			yDir = target.position.y > transform.position.y ? 1 : -1;
		else
			xDir = target.position.x > transform.position.x ? 1 : -1;

		AttemptMove <Player> (xDir, yDir);
	}

	protected override void OnCantMove <T> (T component)
	{
		Player hitPlayer = component as Player;

		hitPlayer.LoseFood (playerDamage);

		animator.SetTrigger ("enemyAttack");

		SoundManager.instance.RandomizeSfx (enemyAttack1, enemyAttack2);
	}
}
