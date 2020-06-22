﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject BeerPrefab;

    public LayerMask blockingLayer;
    public LayerMask itemsLayer;    

    public float restartLevelDelay = 1.5f;
    public float ShiftDelay = 0.5f;
    public float ShiftSpeed = 50f;
    public float RunSpeed = 4f;
    public bool IsRunning;
    public bool IsFacingLeft;
    public bool IsShifting;
    public bool IsServing;
    public float ServeDelay = 0.2f;
    public int CurrentTapIndex;
    public bool IsAtCurrentBarTap;
    
    public float FillSpeed = 0.075f;
    public int FillPercent = 0;

    public AudioSource Fill1;
    public AudioSource Fill2;
    public AudioSource Full;
    public AudioSource Win;
    public AudioSource Lose;
    public AudioSource Up;
    public AudioSource Throw;
    public AudioSource NewLife;
    
    public float FillOffset
    {
        get
        {
            return (float) FillPercent / 100;
        }
    }
    public bool IsFillingBeer;
    public bool IsIdleWithBeer;

    private Rigidbody2D rBody;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    public Animator animator;
    private int levelScore;

    private int horizontalInput;
    private int verticalInput;
    private bool pourPressed;
    private bool servePressed;

    // Start is called before the first frame update
    public void Start()
    {
        NewLife.Play();
        boxCollider = GetComponent<BoxCollider2D>();
        rBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();    
        spriteRenderer = GetComponent<SpriteRenderer>();

        IsRunning = false;
        IsShifting = false;
        IsFacingLeft = false;
        IsFillingBeer = false;
        IsAtCurrentBarTap = false;

        IsIdleWithBeer = false;
        animator.SetBool("isIdleWithBeer", IsIdleWithBeer);

        IsFillingBeer = false;
        animator.SetBool("isFillingBeer", IsFillingBeer);

        BarTap currentTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(CurrentTapIndex);
        rBody.transform.position = currentTap.GetShiftPositionVector();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.HasLevelStarted && (GameManager.instance.levelManager.PlayerMissedCustomer ||
                                                     GameManager.instance.levelManager.PlayerMissedEmptyMug ||
                                                     GameManager.instance.levelManager.PlayerThrewExtraMug))
        {
            GameManager.instance.Oops = true;
        }
        if (GameManager.instance.HasLevelStarted && !GameManager.instance.Oops && animator.GetBool("hasLost") == false && animator.GetBool("hasWon") == false) 
        {
            IsRunning = false;
            IsRunning = false;

            horizontalInput = (int)Input.GetAxisRaw("Horizontal");
            verticalInput = (int)Input.GetAxisRaw("Vertical");
            
            pourPressed = GameInputManager.GetKeyDown("Pour");
            servePressed = GameInputManager.GetKeyDown("Serve");

            StopFillingBeerIfMoving(horizontalInput, verticalInput);
        
            AttemptMove(horizontalInput, verticalInput);
            animator.SetBool("isRunning", IsRunning);

            CheckIfAtBarTap();

            FlipSpriteBasedOnHorizontalInput(horizontalInput);
        }
        
        else if (GameManager.instance.HasLevelStarted && GameManager.instance.Oops && !GameManager.instance.OopsC) 
        {
            TriggerLostAnimation();
            GameManager.instance.OopsC = true;
        }
    }

    void LateUpdate()
    {
        animator.SetFloat("fillOffset", FillOffset);

        if (horizontalInput == 0 && verticalInput == 0 && !IsShifting)
        {
            FillBeerIfPourPressed(pourPressed);
            HideCurrentTapIfFilling();

            // require the user to stop pouring before they can serve (prevents them from just holding the buttons down)
            if (!pourPressed)
            {
                StartCoroutine(ServeBeerIfReady());
            }
        }
    }

    protected IEnumerator ServeBeerIfReady()
    {
        if (!servePressed || FillPercent < 100 || IsShifting)
        {
            yield break;
        }
        Throw.Play();
        
        IsServing = true;
        FillPercent = 0;
        IsFillingBeer = false;
        IsIdleWithBeer = false;

        animator.SetTrigger("playerServe");
        animator.SetBool("isServing", IsServing);
        animator.SetBool("isFillingBeer", IsFillingBeer);
        animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
        animator.SetFloat("fillOffset", FillOffset);


        int beerDir = -1;

        BarTap currentTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(CurrentTapIndex);

        if (currentTap.IsFlipped)
        {
            beerDir = 1;
        }

        float beerOffsetX = 1.6f;
        float beerOffsetY = 0.7f;


        GameObject beerObj = Instantiate(BeerPrefab, transform.position + new Vector3(beerOffsetX * beerDir, beerOffsetY, 0), transform.rotation);
        Beer beer = beerObj.GetComponent<Beer>();
        beer.HorionztalDir = beerDir;
        beer.IsFilled = true;
        beer.Speed = GameManager.instance.levelManager.GetPlayerBeerSpeed();
        beer.TapIndex = this.CurrentTapIndex;

        yield return new WaitForSeconds(ServeDelay);

        IsServing = false;
        animator.SetBool("isServing", IsServing);
    }

    private void HideCurrentTapIfFilling()
    {
        if (!IsFillingBeer && !IsIdleWithBeer)
        {
            return;
        }

        BarTap currentTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(CurrentTapIndex);

        if (currentTap.IsPlayerAtTap && !IsShifting)
        {
            currentTap.GetComponent<SpriteRenderer>().enabled = false;
            
            Vector3 theScale = transform.localScale;
            theScale.x = currentTap.transform.localScale.x;
            transform.localScale = theScale;
        }
    }

    private void CheckIfAtBarTap()
    {
        RaycastHit2D hit;
        BarTap touchingTap = null;

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(transform.position, transform.position, itemsLayer);
        boxCollider.enabled = true;

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

    private void StopFillingBeerIfMoving(int horizontal, int vertical)
    {
        if ((horizontal != 0 || vertical != 0) && IsFillingBeer)
        {
            IsFillingBeer = false;
            IsIdleWithBeer = false;
            FillPercent = 0;
            animator.SetBool("isFillingBeer", IsFillingBeer);
            animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
        }
    }

    private void FillBeerIfPourPressed(bool pourPressed)
    {
        if (pourPressed && !IsFillingBeer && !IsIdleWithBeer && !IsServing)
        {
            // pouring and has not started filling beer -> start filling beer
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
            
            HorizontalFlipSpriteBasedOnBool(currentTap.IsFlipped);
        }
        else if (pourPressed && IsFillingBeer && IsIdleWithBeer && !IsServing)
        {
            // pour pressed while being idle (paused filling) -> resume filling the beer
            IsIdleWithBeer = false;
            animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
            StartCoroutine(FillBeer());
        }
        else if (!pourPressed && IsFillingBeer && !IsIdleWithBeer && !IsServing)
        {
            // stopped pouring in the middle of filling beer -> user becomes idle
            IsIdleWithBeer = true;
            animator.SetBool("isIdleWithBeer", IsIdleWithBeer);
        }
    }

    protected IEnumerator ShiftToBarTapAndFillBeer()
    {
        Up.Play();
        ShiftToNextBarTap(0); // 0 will cause to shift to current bar tap

        // wait until done shifting before filling beer
        while (IsShifting)
        {
            yield return null;
        }        

        IsFillingBeer = true;
        StartCoroutine(FillBeer());
    }

    protected IEnumerator FillBeer()
    {
        animator.SetBool("isFillingBeer", IsFillingBeer);

        while (IsFillingBeer && !IsIdleWithBeer && FillPercent <= 100 && !IsShifting)
        {
            FillPercent += 20;
            yield return new WaitForSeconds(FillSpeed);
        }

        if (FillPercent >=1 && FillPercent <=49)
        {
            Fill1.Play();
        }

        if (FillPercent >=50 && FillPercent <=99)
        {
            Fill2.Play();
        }
        
        if (FillPercent >= 100)
        {
            Full.Play();
            FillPercent = 100;
        }

        animator.SetBool("isFillingBeer", IsFillingBeer);
    }

    void FlipSpriteBasedOnHorizontalInput(int horizontalDir)
    {
        if (horizontalDir == 0)
        {
            return;
        }

        if (!IsFacingLeft && horizontalDir < 0)
        {
            IsFacingLeft = true;
        }
        else if (IsFacingLeft && horizontalDir > 0)
        {
            IsFacingLeft = false;
        }

        HorizontalFlipSpriteBasedOnBool(IsFacingLeft);
    }

    private void HorizontalFlipSpriteBasedOnBool(bool condition)
    {
        spriteRenderer.flipX = condition;
    }
    
    protected bool Move (int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        RaycastHit2D itemHit;

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        itemHit = Physics2D.Linecast(start, end, itemsLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            if (!IsShifting && yDir != 0)
            {
                Up.Play();
                ShiftToNextBarTap(yDir);
            } 
            else if (xDir != 0 && !IsFillingBeer)
            {
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

    private void ShiftToNextBarTap(int yDir)
    {
        if (IsShifting)
        {
            return; // don't do anything if already shifting
        }
        
        int nextTapIndex = CurrentTapIndex;

        if (yDir < 0)
        {
            nextTapIndex ++;
        }
        else if (yDir > 0)
        {
            nextTapIndex --;
        }


        BarTap foundTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(nextTapIndex);
        BarTap firstTap = GameManager.instance.levelManager.GetFirstBarTap();
        BarTap lastTap = GameManager.instance.levelManager.GetLastBarTap();

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

        if (isFound && !foundTap.IsPlayerAtTap)
        {
            CurrentTapIndex = nextTapIndex;

            BoxCollider2D tapCollider = foundTap.GetComponent<BoxCollider2D>();
            Vector3 newPos = foundTap.GetShiftPositionVector();

            StartCoroutine(DoShift(newPos));
        }
    }

    protected IEnumerator DoShift(Vector3 end)
    {
        float halfShiftTime = ShiftDelay * 0.5f;

        IsShifting = true;
        animator.SetTrigger("playerShift");

        yield return new WaitForSeconds(halfShiftTime);

        Vector3 newPosition = Vector3.MoveTowards(rBody.position, end, ShiftSpeed);
        rBody.MovePosition(newPosition);

        yield return new WaitForSeconds(halfShiftTime);

        IsShifting = false;
    }

    protected bool AttemptMove (int xDir, int yDir)
    {
        if (IsShifting || (xDir == 0 && yDir == 0))
        {
            // don't move if shifting or no input in X or Y direction
            return false;
        }

        if (xDir != 0 && IsFillingBeer)
        {
            // don't allow running when filling beer
            return false;
        }

        if (IsAtCurrentBarTap && xDir != 0)
        {
            // don't allow player to move pass bar tap
            BarTap currentTap = GameManager.instance.levelManager.GetBarTapAtTapIndex(CurrentTapIndex);
            if (xDir > 0 && !currentTap.IsFlipped)
            {
                return false;
            }

            if (xDir < 0 && currentTap.IsFlipped)
            {
                return false;
            }
        }

        if (xDir != 0)
        {
            yDir = 0;
        }

        RaycastHit2D hit;
        return Move(xDir, yDir, out hit);

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Beer"))
        {
            Beer beer = collider.gameObject.GetComponent<Beer>();

            if (!beer.IsFilled)
            {
                Destroy(beer.gameObject);
                GameManager.instance.AddToCurrentPlayerScore(ScoreKey.EmptyMug);                
            }
        }
        
        if (collider.gameObject.CompareTag("Tip"))
        {
            Tip tip = collider.gameObject.GetComponent<Tip>();
            GameManager.instance.AddToCurrentPlayerScore(ScoreKey.Tip);
            Destroy(tip.gameObject);
        }
    }

    public void TriggerMissAnimation()
    {
        animator.SetTrigger("playerMiss");
    }
    
    public void TriggerLostAnimation()
    {
        Lose.Play();
        animator.SetBool("hasLost", true);
        StartCoroutine("LostAnim");
    }

    public void WinTune()
    {
        Win.Play();
    }
    
    protected IEnumerator LostAnim()
    {
        GameManager.instance.IsGameWon = false;
        yield return new WaitForSeconds(15);
        animator.SetBool("hasLost", false);
        GameManager.instance.PlayerLost();
    }
}
