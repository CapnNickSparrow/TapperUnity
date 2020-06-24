using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Connects to certain Game Objects
    private Rigidbody2D rBody;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    public Animator animator;
    
    public GameObject BeerPrefab;

    public LayerMask blockingLayer;
    public LayerMask itemsLayer;    
    
    public AudioSource Fill1;
    public AudioSource Fill2;
    public AudioSource Full;
    public AudioSource Win;
    public AudioSource Lose;
    public AudioSource Up;
    public AudioSource Throw;
    public AudioSource NewLife;

    // Ints
    private int levelScore;
    private int horizontalInput;
    private int verticalInput;
    public int CurrentTapIndex;
    public int FillPercent;
    
    // Floats
    public float ShiftDelay;
    public float ShiftSpeed;
    public float RunSpeed;
    public float ServeDelay;
    public float FillSpeed;
    
    public float FillOffset
    {
        get
        {
            return (float) FillPercent / Constants.FILL_FULL;
        }
    }
    
    // Bools
    public bool IsFillingBeer;
    public bool IsIdleWithBeer;
    private bool pourPressed;
    private bool servePressed;
    public bool IsRunning;
    public bool IsFacingLeft;
    public bool IsShifting;
    public bool IsServing;
    public bool IsAtCurrentBarTap;

    // Start is called before the first frame update
    public void Start()
    {
        // Sets Default Values for Certain Variables
        ShiftDelay = Constants.SHIFT_DELAY;
        ShiftSpeed = Constants.SHIFT_SPEED;
        RunSpeed = Constants.RUN_SPEED;
        ServeDelay = Constants.SERVE_DELAY;
        FillSpeed = Constants.FILL_SPEED;
        FillPercent = Constants.FILL_PERCENT;

        // Every time the Player starts it will play a "You used a life" Track 
        NewLife.Play();
        
        // Connects to the Components
        boxCollider = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();    
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Sets Default Values to false to prevent misbehaviour 
        IsRunning = false;
        IsShifting = false;
        IsFacingLeft = false;
        IsFillingBeer = false;
        IsAtCurrentBarTap = false;

        // Sets the Animator Values according to the Default Bool Values
        IsIdleWithBeer = false;
        animator.SetBool("isIdleWithBeer", IsIdleWithBeer);

        IsFillingBeer = false;
        animator.SetBool("isFillingBeer", IsFillingBeer);

        // Get Current Bartap and set Default Position
        BarTap currentTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(CurrentTapIndex);
        rBody.transform.position = currentTap.GetShiftPositionVector();
    }

    // Update is called once per frame
    void Update()
    {
        // If the player makes a mistake zet the Oops bool to true to stop all processes
        if (GameManager.instance.HasLevelStarted && (GameManager.instance.levelManager.PlayerMissedCustomer || GameManager.instance.levelManager.PlayerMissedEmptyMug || GameManager.instance.levelManager.PlayerThrewExtraMug))
        {
            GameManager.instance.Oops = true;
        }
        
        // If the level has started and Tapper hasn't won or lost yet he can move
        if (GameManager.instance.HasLevelStarted && !GameManager.instance.Oops && animator.GetBool("hasLost") == false && animator.GetBool("hasWon") == false) 
        {
            // Set Running Bool to False to prevent move animation
            IsRunning = false;

            // Set Hortizontal Input and Vertical Input (WASD and Arrows)
            horizontalInput = (int)Input.GetAxisRaw("Horizontal");
            verticalInput = (int)Input.GetAxisRaw("Vertical");
            
            // Set bool for Pour and Serving using to GameInputManager
            pourPressed = GameInputManager.GetKeyDown("Pour");
            servePressed = GameInputManager.GetKeyDown("Serve");

            // If moving stop filling bottle
            StopFillingBeerIfMoving(horizontalInput, verticalInput);
        
            // Attempt Move if pressing the Horizontal or Vertical Input
            AttemptMove(horizontalInput, verticalInput);
            
            // Set IsRunning Animator Bool the same as the current local IsRunning Bool
            animator.SetBool("isRunning", IsRunning);

            // Check if Player is at the Tap with this function
            CheckIfAtBarTap();

            // Flip the Sprite using the Horizontalinput
            FlipSpriteBasedOnHorizontalInput(horizontalInput);
        }
        
        // If level has started and the player made mistake Trigger The Lost Animation
        else if (GameManager.instance.HasLevelStarted && GameManager.instance.Oops && !GameManager.instance.OopsC) 
        {
            TriggerLostAnimation();
            // Set OopsC to true to not retrigger the Lost Animation
            GameManager.instance.OopsC = true;
        }
    }

    // Checks after update frames
    void LateUpdate()
    {
        // Set Float for the FillOffSet Animator Transitions
        animator.SetFloat("fillOffset", FillOffset);

        // If player isn't moving or shifting and is pressing the pour button fill bottle and hide current tap if feeling
        if (horizontalInput == Constants.ZERO && verticalInput == Constants.ZERO && !IsShifting)
        {
            FillBeerIfPourPressed(pourPressed);
            HideCurrentTapIfFilling();

            // If pour isn't pressed check if the bottle is ready to serve
            // require the user to stop pouring before they can serve (prevents them from just holding the buttons down)
            if (!pourPressed)
            {
                StartCoroutine(ServeBeerIfReady());
            }
        }
    }

    // Check if bottle is ready to serve
    protected IEnumerator ServeBeerIfReady()
    {
        // If fill percentage is less then 100% or the serve button isn't being pressed or the player is shifting then break the actiom
        if (!servePressed || FillPercent < Constants.FILL_FULL || IsShifting)
        {
            yield break;
        }
        
        // Play a serve sound
        Throw.Play();

        // Set the correct variables to true to get the proper animation
        IsServing = true;
        
        // Set the Fill Percent to 0 so the function doesn't repeat
        FillPercent = Constants.FILL_PERCENT;
        
        // Set the following variables to false to get the proper animation and doesn't let the stystem repeat
        IsFillingBeer = false;
        IsIdleWithBeer = false;

        // Sets the animator variables to get correct animation
        animator.SetTrigger("playerServe");
        animator.SetBool("isServing", IsServing);
        animator.SetBool("isFillingBeer", IsFillingBeer);
        animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
        animator.SetFloat("fillOffset", FillOffset);

        // Sets the Bottle Direction
        int beerDir = Constants.MIN_ONE;

        // Gets Current Bartap
        BarTap currentTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(CurrentTapIndex);

        // If the Current Tap is Flipped set new Bottle direction 
        if (currentTap.IsFlipped)
        {
            beerDir = Constants.ONE;
        }

        // Sets Bottle Offsets
        float beerOffsetX = Constants.BOTTLE_OFFSET_X_P;
        float beerOffsetY = Constants.BOTTLE_OFFSET_Y_P;

        // Instantiate the bottle prefab and get that component 
        GameObject beerObj = Instantiate(BeerPrefab, transform.position + new Vector3(beerOffsetX * beerDir, beerOffsetY, Constants.ZERO), transform.rotation);
        Beer beer = beerObj.GetComponent<Beer>();
        
        // Set the Horizontal Direction, Correct Speed and get Current Tap and Set the IsFilled Bool to true
        beer.HorionztalDir = beerDir;
        beer.IsFilled = true;
        beer.Speed = GameManager.instance.levelManager.GetPlayerBeerSpeed();
        beer.TapIndex = this.CurrentTapIndex;

        // Wait for the Serve Delay
        yield return new WaitForSeconds(ServeDelay);

        // Set is Serving Bool to false and set the Animator Bool to current is Serving Bool
        IsServing = false;
        animator.SetBool("isServing", IsServing);
    }
    
    private void HideCurrentTapIfFilling()
    {
        // If it isn't being filled don't go further
        if (!IsFillingBeer && !IsIdleWithBeer)
        {
            return;
        }

        // Get CurrentBarTap 
        BarTap currentTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(CurrentTapIndex);

        // If Player is at the Current Tap set the CurrentTap Sprite to false
        if (currentTap.IsPlayerAtTap && !IsShifting)
        {
            currentTap.GetComponent<SpriteRenderer>().enabled = false;
            
            Vector3 theScale = transform.localScale;
            theScale.x = currentTap.transform.localScale.x;
            transform.localScale = theScale;
        }
    }

    // Check if Player is at the Tab by checking layers 
    private void CheckIfAtBarTap()
    {
        RaycastHit2D hit;
        BarTap touchingTap = null;

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(transform.position, transform.position, itemsLayer);
        boxCollider.enabled = true;

        // If you collide with layer of the bartap set is at current bartap to true otherwise to false
        if (hit.transform != null && hit.transform.tag == "BarTap")
        {
            touchingTap = hit.transform.GetComponent<BarTap>();
            touchingTap.IsPlayerAtTap = true;
            IsAtCurrentBarTap = true;
        }
        else
        {
            IsAtCurrentBarTap = false;
        }
        
        // mark other taps as the player not being there
        List<BarTap> taps = GameManager.instance.levelManager.GetBarTaps();
        foreach (BarTap otherTap in taps)
        {
            if (touchingTap != null && touchingTap.TapIndex != otherTap.TapIndex)
            {
                otherTap.IsPlayerAtTap = false;
            }
            else if (touchingTap == null)
            {
                otherTap.IsPlayerAtTap = false;
            }
        }
    }

    // If you are moving while filling bottle set the fill bools to false and call the animator with the new values to get the correct animation
    private void StopFillingBeerIfMoving(int horizontal, int vertical)
    {
        if ((horizontal != Constants.ZERO || vertical != Constants.ZERO) && IsFillingBeer)
        {
            IsFillingBeer = false;
            IsIdleWithBeer = false;
            
            // Set Fill Value to 0 again 
            FillPercent = Constants.FILL_PERCENT;
            
            animator.SetBool("isFillingBeer", IsFillingBeer);
            animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
        }
    }

    // Fill bottle with proper functions
    private void FillBeerIfPourPressed(bool pourPressed)
    {
        // If the Pour Button is pressed and you are not filling the bottle and not being idle with the bottle and not serving
        // Get Current Bar Tap and check the Player is at that BarTap if not move to it if yes set the IsFilling bool to true and starting filling
        if (pourPressed && !IsFillingBeer && !IsIdleWithBeer && !IsServing)
        {
            // pouring and has not started filling bottle -> start filling bottle
            BarTap currentTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(CurrentTapIndex);
            if (!IsAtCurrentBarTap)
            {
                StartCoroutine(ShiftToBarTapAndFillBeer());
            }
            else
            {
                IsFillingBeer = true;
                StartCoroutine(FillBeer());
            }
            
            // Set CurrentTap on flipped so the other tap image will show
            HorizontalFlipSpriteBasedOnBool(currentTap.IsFlipped);
        }
        
        // If you were filling but stopped you can resume it only if you are still idle and set correct bool for Animator
        else if (pourPressed && IsFillingBeer && IsIdleWithBeer && !IsServing)
        {
            // pour pressed while being idle (paused filling) -> resume filling the bottle
            IsIdleWithBeer = false;
            animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
            StartCoroutine(FillBeer());
        }
        
        // If you stop pressing the pour button it will go to idle and set correct bool for Animator
        else if (!pourPressed && IsFillingBeer && !IsIdleWithBeer && !IsServing)
        {
            // stopped pouring in the middle of filling bottle -> user becomes idle
            IsIdleWithBeer = true;
            animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
        }
    }

    // Move to Bar if you press pour when moving horizontally
    protected IEnumerator ShiftToBarTapAndFillBeer()
    {
        // Play the move SFX and Shift to the Bar
        Up.Play();
        ShiftToNextBarTap(Constants.ZERO); // 0 will cause to shift to current bar tap

        // wait until done shifting before filling bottle
        while (IsShifting)
        {
            yield return null;
            
        }        

        // Start filling the bottle and set the bool to true
        IsFillingBeer = true;
        StartCoroutine(FillBeer());
    }

    // Start Filling the bottle
    protected IEnumerator FillBeer()
    {
        // Set the Animator Bool to true with the previous changed bool to true
        animator.SetBool("isFillingBeer", IsFillingBeer);

        // While you are filling, not shifting and the bottle is not full keep adding 20% on each tap
        if (IsFillingBeer && !IsIdleWithBeer && FillPercent <= Constants.FILL_FULL && !IsShifting)
        {
            FillPercent += Constants.FILL_X_PERCENT;
            yield return new WaitForSeconds(FillSpeed);
        }

        // If the bottle is below 50% full use the first Fill SFX variant
        if (FillPercent >= Constants.ONE && FillPercent <= Constants.FILL_49_PERCENT)
        {
            Fill1.Play();
        }

        // If the bottle is below 100% full use the second Fill SFX variant
        else if (FillPercent >= Constants.FILL_50_PERCENT && FillPercent <= Constants.FILL_99_PERCENT)
        {
            Fill2.Play();
        }
        
        // If the bottle is full use the last Fill SFX variant
        else if (FillPercent >= Constants.FILL_FULL)
        {
            Full.Play();
            FillPercent = Constants.FILL_FULL;
        }

        // Set the Animator Bool to true with the previous changed bool to that state
        animator.SetBool("isFillingBeer", IsFillingBeer);
    }

    // Flip Sprites on Horizontal Input
    void FlipSpriteBasedOnHorizontalInput(int horizontalDir)
    {
        // If Horizontal Dir is on 0 do nothing
        if (horizontalDir == Constants.ZERO)
        {
            return;
        }

        // If Horizontal Dir is below 0 and isn't facing left set it to be facing to the left
        if (!IsFacingLeft && horizontalDir < Constants.ZERO)
        {
            IsFacingLeft = true;
        }
        
        // If Horizontal Dir is above 0 and is facing left set it to be not facing to the left
        else if (IsFacingLeft && horizontalDir > Constants.ZERO)
        {
            IsFacingLeft = false;
        }

        // Set the flip based on the bool
        HorizontalFlipSpriteBasedOnBool(IsFacingLeft);
    }

    // Set the flip based on the bool IsFacingLeft
    private void HorizontalFlipSpriteBasedOnBool(bool condition)
    {
        spriteRenderer.flipX = condition;
    }
    
    // Move according to the Horizontal Input and Verticel Input
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        // Get current position and set the end position
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        RaycastHit2D itemHit;

        // Get components
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        itemHit = Physics2D.Linecast(start, end, itemsLayer);
        boxCollider.enabled = true;

        // If it hasn't hit the wall you can move, otherwise you can only one way
        if (hit.transform == null)
        {
            // If you move vertically play the Move SFX and go to the next tap
            if (!IsShifting && yDir != Constants.ZERO)
            {
                Up.Play();
                ShiftToNextBarTap(yDir);
            } 
            
            // If you move horizontally play the Move SFX and run through the scene
            else if (xDir != Constants.ZERO && !IsFillingBeer)
            {
                // Set the running bool to true and start moving toward the end and set the Animator bools which are false
                IsRunning = true;
                Vector3 newPosition = Vector3.MoveTowards(rBody.position, end, RunSpeed * Time.deltaTime);
                rBody.MovePosition(newPosition);

                animator.SetBool("isFillingBeer", IsFillingBeer);
                animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
            }

            return true;
        }

        return false;
    }

    // Shift to next Bar Tap
    private void ShiftToNextBarTap(int yDir)
    {
        if (IsShifting)
        {
            return; // don't do anything if already shifting
        }
     
        // Get Next Tap Index
        int nextTapIndex = CurrentTapIndex;

        // If you are going down go to the next bar
        if (yDir < Constants.ZERO)
        {
            nextTapIndex ++;
        }
        
        // If you go up go to the previous bar
        else if (yDir > Constants.ZERO)
        {
            nextTapIndex --;
        }


        // Get all Bars
        BarTap foundTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(nextTapIndex);
        BarTap firstTap = GameManager.instance.levelManager.GetFirstBarTap();
        BarTap lastTap = GameManager.instance.levelManager.GetLastBarTap();

        // Is found is only found if it found a tap
        bool isFound = (foundTap != null);

        // wrap to beginning or last tap if needed
        if (!isFound && nextTapIndex < firstTap.TapIndex) 
        {
            isFound = true;
            nextTapIndex = lastTap.TapIndex;
            foundTap = lastTap;
        } 
        else if (!isFound && nextTapIndex > lastTap.TapIndex)
        {
            isFound = true;
            nextTapIndex = firstTap.TapIndex;
            foundTap = firstTap;
        }

        // If player is in the scene but not at the tap still Shift but also resets it's position according to where the tap is
        if (isFound && !foundTap.IsPlayerAtTap)
        {
            CurrentTapIndex = nextTapIndex;

            BoxCollider2D tapCollider = foundTap.GetComponent<BoxCollider2D>();
            Vector3 newPos = foundTap.GetShiftPositionVector();

            StartCoroutine(DoShift(newPos));
        }
    }

    // Shift to the next bar but also reset it's position according to the bartap
    protected IEnumerator DoShift(Vector3 end)
    {
        float halfShiftTime = ShiftDelay * Constants.HALF_SPEED;

        IsShifting = true;
        animator.SetTrigger("playerShift");

        yield return new WaitForSeconds(halfShiftTime);

        Vector3 newPosition = Vector3.MoveTowards(rBody.position, end, ShiftSpeed);
        rBody.MovePosition(newPosition);

        yield return new WaitForSeconds(halfShiftTime);

        IsShifting = false;
    }

    // Try moving according the new Direction
    protected bool AttemptMove (int xDir, int yDir)
    {
        if (IsShifting || (xDir == Constants.ZERO && yDir == Constants.ZERO))
        {
            // don't move if shifting or no input in X or Y direction
            return false;
        }

        if (xDir != 0 && IsFillingBeer)
        {
            // don't allow running when filling beer
            return false;
        }

        if (IsAtCurrentBarTap && xDir != Constants.ZERO)
        {
            // don't allow player to move pass bar tap
            BarTap currentTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(CurrentTapIndex);
            if (xDir > Constants.ZERO && !currentTap.IsFlipped)
            {
                return false;
            }

            if (xDir < Constants.ZERO && currentTap.IsFlipped)
            {
                return false;
            }
        }

        // If you move around the scene set ydir back to his now default position
        if (xDir != Constants.ZERO)
        {
            yDir = Constants.ZERO;
        }

        RaycastHit2D hit;
        return Move(xDir, yDir, out hit);

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // If colliding with a Bottle get the Bottle component and if the Bottle isn't filled Destroy the bottle and add Playerscore for catching an empty bottle
        if (collider.gameObject.CompareTag("Beer"))
        {
            Beer beer = collider.gameObject.GetComponent<Beer>();

            if (!beer.IsFilled)
            {
                Destroy(beer.gameObject);
                GameManager.instance.AddToCurrentPlayerScore(ScoreKey.EmptyMug);                
            }
        }
        
        // If colliding with a Tip get the Tip component and destroy the tip and add Playerscore for taking the Tip
        if (collider.gameObject.CompareTag("Tip"))
        {
            Tip tip = collider.gameObject.GetComponent<Tip>();
            Destroy(tip.gameObject);
            GameManager.instance.AddToCurrentPlayerScore(ScoreKey.Tip);
        }
    }
    
    
    // Triggers the PlayerMiss Animation Trigger
    public void TriggerMissAnimation()
    {
        animator.SetTrigger("playerMiss");
    }
    
    // Plays the Lose Tune and sets the hasLost bool to true and start starts the Loss Animation
    public void TriggerLostAnimation()
    {
        Lose.Play();
        animator.SetBool("hasLost", true);
        StartCoroutine("LostAnim");
    }

    // Sets the IsGameWon bool to false and waits 15 seconds till the animation is finished and sets the hasLost Bool to false and triggers the PlayerLost function in the GameManager
    protected IEnumerator LostAnim()
    {
        GameManager.instance.IsGameWon = false;
        yield return new WaitForSeconds(Constants.WAIT_15_SEC);
        animator.SetBool("hasLost", false);
        GameManager.instance.PlayerLost();
    }
}
