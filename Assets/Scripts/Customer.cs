using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    // Connects the Gameobjects like Audio and Prefabs
    private AudioSource Drink;

    public GameObject BeerPrefab;
    public GameObject Tip;
    
    private GameObject Player;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rBody;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    // Ints
    public int TapIndex;
    
    public int HorionztalDir = 1;

    private int Char;
    
    // Floats
    public float MoveSpeed;

    public float SlideSpeed;

    public float MinSlideDistance;
    public float MaxSlideDistance;

    public float MinDrinkTime;
    public float MaxDrinkTime;
    
    private float currentMoveTime;
    public float RandomMoveTime;

    public float MinMoveTime;
    public float MaxMoveTime;

    public float MinStopTime;
    public float MaxStopTime;

    // Bools
    public bool IsSliding;
    public bool IsDistracted;
    public bool IsDrinking;
    public bool HasDrunk;
    private bool ReturnBeerOnNextUpdate;
    public bool IsStopped;
    
    public bool CanDrink
    {
        get { return !IsSliding && !IsDrinking && !IsDistracted; }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        // Find the player object and sets certain bools to false otherwise basic functionality get stuck in phases
        Player = GameObject.Find("Player");
        IsDistracted = false;
        IsDrinking = false;
        IsSliding = false;
        IsStopped = false;
        HasDrunk = false;
        ReturnBeerOnNextUpdate = false;

        // Sets time to Standard 0
        currentMoveTime = Constants.ZERO;
        RandomMoveTime = Constants.ZERO;

        // Connect the components 
        boxCollider = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        Drink = GetComponent<AudioSource>();
        
        // Generates a random number between the first customer (0) and the last customer (8), the last customer number does not count
        Char = Random.Range(Constants.CUSTOMER_FIRST, Constants.CUSTOMER_LAST);
        
        // Set the animator and thus the character based on the Customer Generated Number, 8 different customers so 8 different trees in the animation controller 
        animator.SetInteger("Idle", Char);
        
        // Renders the Sprite
        spriteRenderer.flipX = (HorionztalDir == Constants.MIN_ONE);
    }

    // Update is called once per frame
    void Update()
    {
        // If the Bottle is ready to return in the next update and the player hasn't made mistake or won Spawn an Empty Bottle and set the variable for now on false
        if (ReturnBeerOnNextUpdate && !GameManager.instance.Oops && Player.GetComponent<Animator>().GetBool("hasWon") == false)
        {
            StartCoroutine(SpawnEmptyBeerMug());
            ReturnBeerOnNextUpdate = false;
            return;
        }

        // If the MoveTime is set to 0 and the player hasn't won or made a mistake zet a new RandomMoveTime
        if (RandomMoveTime == 0 && !GameManager.instance.Oops && Player.GetComponent<Animator>().GetBool("hasWon") == false)
        {
            RandomMoveTime = Random.Range(MinMoveTime, MaxMoveTime);
        }

        // Start the move forward function
        MoveForward();

        // If current MoveTime overrules the RandomMoveTime and the player hasn't made a mistake or won start the DelayMovement function
        if (currentMoveTime >= RandomMoveTime && !GameManager.instance.Oops && Player.GetComponent<Animator>().GetBool("hasWon") == false)
        {
            StartCoroutine(DelayMovement());
        }
    }
    
    // Delays the next movement to catch up
    protected IEnumerator DelayMovement()
    {
        // Set the character stop bool on true
        IsStopped = true;
        
        // Sets the currentMoveTime on 0 and starts setting a new RandomMoveTime
        currentMoveTime = Constants.ZERO;
        RandomMoveTime = Random.Range(MinMoveTime, MaxMoveTime);

        // Creates a random stop time to use to wait and then sets the character stop bool to false
        float randomStopTime = Random.Range(MinStopTime, MaxStopTime);
        
        yield return new WaitForSeconds(randomStopTime);

        IsStopped = false;
    }

    // Let's make the customer move forward shall we
    void MoveForward()
    {
        // Gives a return function of one of these 3 bools are happening
        if (IsDrinking || IsSliding || IsDistracted)
        {
            return;
        }

        // Also gives a return function when the player has stopped
        if (IsStopped)
        {
            return;
        }

        // If the player hasn't won or made a mistake start the moving process
        if (!GameManager.instance.Oops && Player.GetComponent<Animator>().GetBool("hasWon") == false)
        {
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(HorionztalDir, Constants.ZERO);

            Vector3 newPos = Vector3.MoveTowards(rBody.position, end, MoveSpeed * Time.deltaTime);
            rBody.MovePosition(newPos);

            currentMoveTime += Time.deltaTime;
        }
    }

    // Let the customer slide
    void StartSliding()
    {
        // Checks if one of those bool is happening and gives the value back and starts Sliding Process
        if (IsDrinking || IsSliding)
        {
            return;
        }

        StartCoroutine(SlideBack());
    }

    protected IEnumerator SlideBack()
    {
        // If the character hasn't made a mistake or won yet start setting things up and slide back
        if (!GameManager.instance.Oops && Player.GetComponent<Animator>().GetBool("hasWon") == false)
        {
            // The customer will be able to slide
            IsSliding = true;

            // Sets the random slide distance 
            float randomSlideDist = Random.Range(MinSlideDistance, MaxSlideDistance);

            Vector2 startPosition = transform.position;
            Vector2 finalPosition = startPosition + new Vector2(HorionztalDir * Constants.MIN_ONE * randomSlideDist, Constants.ZERO);

            // If the customer hasn't reached his position yet keep moving
            while (Vector2.Distance(transform.position, finalPosition) > Constants.DISTANCE_THRESHOLD)
            {
                Vector2 currentPos = transform.position;
                Vector2 end = currentPos + new Vector2(HorionztalDir * Constants.MIN_ONE, Constants.ZERO);

                Vector2 nextPosition = Vector3.MoveTowards(rBody.position, end, SlideSpeed * Time.deltaTime);
                rBody.MovePosition(nextPosition);

                yield return null;
            }

            // Customer can't move anymore
            IsSliding = false;

            // While sliding back drink cola
            yield return DrinkBeer();
        }

    }

    // Let the customer drink (No animations for yet), though system is ready to be implemented
    protected IEnumerator DrinkBeer()
    {
        // Sets the Drinking bool to true and let's the animator start the drinking animation
        IsDrinking = true;
        animator.SetBool("isDrinking", IsDrinking);

        // Sets the max amount of time he drinks and wait for those seconds
        float randomDrinkTime = Random.Range(MinDrinkTime, MaxDrinkTime);
        yield return new WaitForSeconds(randomDrinkTime);

        // Set the is drinking animation and drinking bool to false
        IsDrinking = false;
        animator.SetBool("isDrinking", IsDrinking);

        // On the next update spawn a empty bottle
        ReturnBeerOnNextUpdate = true;
    }

    // Spawn a empty bottle
    protected IEnumerator SpawnEmptyBeerMug()
    {
        // wait until customer is finished sliding or drinking before spawning bottle
        if (!GameManager.instance.Oops && !GameManager.instance.NotDone && Player.GetComponent<Animator>().GetBool("hasWon") == false && Player.GetComponent<Animator>().GetBool("hasLost") == false)
        {
            while (IsSliding || IsDrinking)
            {
                yield return null;
            }

            // Spawn the bottle prefab
            GameObject beerObj = Instantiate(BeerPrefab, transform.position + new Vector3(Constants.BOTTLE_OFFSET_X * HorionztalDir, Constants.BOTTLE_OFFSET_Y, Constants.ZERO), transform.rotation);
         
            // Get information about the bottle and set direction and use the unfilled variant
            Beer beer = beerObj.GetComponent<Beer>();
            beer.HorionztalDir = HorionztalDir;
            beer.IsFilled = false;
            
            // Sets the speed of the bottle and uses the same Tap as were the customer was
            beer.Speed = GameManager.instance.levelManager.GetPlayerBeerSpeed() * Constants.HALF_SPEED;
            beer.TapIndex = this.TapIndex;
        }
    }

    // Checks on collision with other object to react to it
    private void OnTriggerEnter2D(Collider2D collider)
    {

        // If you the customer collides with a bottle and he is able to drink and the player hasn't made a mistake yet
        if (collider.gameObject.CompareTag("Beer") && CanDrink && !GameManager.instance.NotDone)
        {
            // Get the collider of the bottle
            Beer beer = collider.GetComponent<Beer>();
            
            // If the bottle is full
            if (beer.IsFilled)
            {
                // Set the has drunk bool to true en begin the drink SFX
                HasDrunk = true;
                Drink.Play();
                
                // Destroy the bottle and start sliding
                Destroy(beer.gameObject);
                StartSliding();
                
                // Set a new int called chances, the number is between 0 and 10, 10 does not count
                int Chances = Random.Range(Constants.ZERO, Constants.CUSTOMER_LAST_CHANCE);
                
                // If you the chances number is higher than the Lucky Chance number 7 the customer will drop a tip for Tapper the collect
                if (Chances > Constants.CUSTOMER_LUCKY_CHANCE)
                {
                    Instantiate(Tip, transform.position + new Vector3(Constants.ZERO, Constants.BOTTLE_HIGH_Y, Constants.ZERO), Quaternion.identity);
                }
            }
        }
        
        // If the customer has drunk and reaches the exit based on the customer rarity it will give score, some give 50, some 75 and some 100
        if (collider.gameObject.CompareTag("Exit") && HasDrunk)
        {
            if (Char >= Constants.CUSTOMER_FIRST && Char < Constants.CUSTOMER_LOW)
            {
                GameManager.instance.AddToCurrentPlayerScore(ScoreKey.Customer);
            }

            if (Char >= Constants.CUSTOMER_LOW && Char < Constants.CUSTOMER_MED)
            {
                GameManager.instance.AddToCurrentPlayerScore(ScoreKey.HardCustomer);
            }

            if (Char >= Constants.CUSTOMER_MED && Char < Constants.CUSTOMER_LAST)
            {
                GameManager.instance.AddToCurrentPlayerScore(ScoreKey.HarderCustomer);
            }

            // Adds a HappyCustomer to the list to get through the level
            GameManager.instance.HappyCustomer++;
            
            // Destroys the Customer
            Destroy(this.gameObject);
        }

        // If the angry customers gets at the end you failed to serve and you will now make a mistake and lose
        if (collider.gameObject.CompareTag("BarEnd") && !IsDrinking) 
        {
            GameManager.instance.levelManager.PlayerMissedCustomer = true;
        }
    }
}
