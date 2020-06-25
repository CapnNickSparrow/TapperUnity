using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beer : MonoBehaviour
{
    // Audio Variable
    private AudioSource Break;
    
    // Bools
    public bool IsFilled;

    public bool IsShattered;
    public bool IsFalling;

    // Ints
    public int HorionztalDir;

    public int TapIndex;
    
    // Floats
    public float Speed;

    // Component Variables
    public Sprite EmptyBeerMugSprite; 
    
    private Rigidbody2D rBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private GameObject Player;


    // Start is called before the first frame update
    void Start()
    {
        // Connects to the components
        rBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Player = GameObject.Find("Player");
        
        Break = GetComponent<AudioSource>();
        
        // If it isn't filled show the empty bottle sprite
        if (!IsFilled)
        {
            animator.SetTrigger("emptyBeer");
        }

        spriteRenderer.flipX = (HorionztalDir == Constants.MIN_ONE);

        // Flip on the X as
        if (spriteRenderer.flipX)
        {
            // flip offset if sprite is flipped
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            Vector2 offset = collider.offset;
            offset.x *= HorionztalDir;
            collider.offset = offset;
        }

        // Sets some bools false otherwise it will cause problems
        IsShattered = false;
        IsFalling = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If there is nothing to worry about keep moving the bottle
        if (!GameManager.instance.Oops && !GameManager.instance.NotDone)
        {
            Move();   
        }
    }

    // Moves the bottle across the bar
    void Move()
    {
        if (!GameManager.instance.Oops && !GameManager.instance.NotDone && Player.GetComponent<Animator>().GetBool("hasWon") == false && Player.GetComponent<Animator>().GetBool("hasLost") == false)
        {
            if (IsShattered || IsFalling)
            {
                return;
            }

            // Sets the start and end position it ends to go to
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(HorionztalDir, Constants.ZERO);

            // Says it needs to move the the end position
            Vector3 newPos = Vector3.MoveTowards(rBody.position, end, Speed * Time.deltaTime);
            rBody.MovePosition(newPos);
        }
    }

    // Checks for collision
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Of the bottle collides with the barexit and if the bottle is full then it it will shatter and sets the Oops and NotDone variable to true so movement stops
        if (collider.gameObject.CompareTag("Exit") && IsFilled)
        {
            GameManager.instance.NotDone = true;
            GameManager.instance.Oops = true;
            Break.Play();
            StartCoroutine(ShatterBeer());
        }

        // If the a empty bottle reaches the end of the bar and the player isn't at that bar to catch it let the bottle shatter and sets the Oops and NotDone variable to true so movement stops
        // If the player does catch the bottle destroy the bottle and gives player a some score
        if (collider.gameObject.CompareTag("BarEnd") && !IsFilled && !GameManager.instance.NotDone)
        {
            if (GameManager.instance.levelManager.IsPlayerAtBarTap(TapIndex) && !GameManager.instance.NotDone)
            {
                GameManager.instance.AddToCurrentPlayerScore(ScoreKey.EmptyMug);
                Destroy(this.gameObject);
            }
            else
            {
                GameManager.instance.NotDone = true;
                GameManager.instance.Oops = true;
                StartCoroutine(DropBeerFromBar());
            }
        }
    }

    // Animation Trigger if the Bottle has reached the end of the bar and the player wasn't there
    protected IEnumerator DropBeerFromBar()
    {
        // Set's a variable to true
        GameManager.instance.levelManager.PlayerMissedEmptyMug = true;
        
        // Able to still move until it is to late
        float waitTime = Constants.ZERO;
        while (waitTime < Constants.WAIT_45T_SEC)
        {
            yield return null;
            waitTime += Time.deltaTime;
            Move();  
        }

        // Run the animation when the player makes a mistake
        TriggerPlayerMissAnimation();
        
        // The bottle is falling and set the animator falling trigger to true
        IsFalling = true;
        animator.SetTrigger("beerFall");

        waitTime = Constants.ZERO;
    
        // Let the bottle move a bit and after one second let bottle shatter
        while (waitTime < Constants.WAIT_1_SEC)
        {
            yield return null;
            waitTime += Time.deltaTime;

            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(Constants.ZERO, Constants.MIN_ONE);

            Vector3 newPos = Vector3.MoveTowards(rBody.position, end, Speed * Time.deltaTime);
            rBody.MovePosition(newPos);   
        }

        animator.SetTrigger("shatterBeer");
        
        // Wait three quarter second to destroy the gameobject
        yield return new WaitForSeconds(Constants.WAIT_3Q_SEC);

        Destroy(this.gameObject);
    }

    // Lets the bottle shatter
    protected IEnumerator ShatterBeer()
    {
        // Sets the shattered trigger for the animator to true
        IsShattered = true;
        animator.SetTrigger("shatterBeer");

        // Let the player use his mistake animation
        TriggerPlayerMissAnimation();
        
        // Let the shatter audio play 
        Break.Play();
        
        // Wait a seond until you destroy the bottle
        yield return new WaitForSeconds(Constants.WAIT_1_SEC);

        GameManager.instance.levelManager.PlayerThrewExtraMug = true;

        Destroy(this.gameObject);
    }

    // Triggers the player mistake animation
    private void TriggerPlayerMissAnimation()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerObj.GetComponent<Player>().TriggerMissAnimation();
        }
    }
}
