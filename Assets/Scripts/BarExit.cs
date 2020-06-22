 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarExit : MonoBehaviour
{
    public int TapIndex;

    public bool IsFlipped;

    public int CustomerLimit;
    
    [SerializeField]
    private float SpawnCoolDownTime;
    [SerializeField]
    private float cooldownTimer;

    // Connects to certain GameObjects
    public GameObject CustomerPrefab;
    private GameObject Player;


    // Start is called before the first frame update
    void Start()
    {
        // Checks the current level to set the proper Spawn Cool Down Timer
        if (GameManager.instance.levelManager.level == Constants.LVL_1)
        {
            SpawnCoolDownTime = Constants.SPAWN_COOLDOWN_TIMER_LVL1;
        }
        else if (GameManager.instance.levelManager.level == Constants.LVL_2)
        {
            SpawnCoolDownTime = Constants.SPAWN_COOLDOWN_TIMER_LVL2;
        }
        else if (GameManager.instance.levelManager.level == Constants.LVL_3)
        {
            SpawnCoolDownTime = Constants.SPAWN_COOLDOWN_TIMER_LVL3;
        }
        
        // Checks what the player is en renders the bar Sprite
        Player = GameObject.Find("Player");  
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        IsFlipped = renderer.flipX;   
        
        // Cooltime starts at 0 so the customers can begin to spawn
        cooldownTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Checks if we are below the max amount of customers we can have and if the Cooldown is 0 again and if the player hasn't made a mistake or is busy with a animation
        if (GetCustomerCount() < CustomerLimit && cooldownTimer >= SpawnCoolDownTime && GameManager.instance.HasLevelStarted && !GameManager.instance.Oops && Player.GetComponent<Animator>().GetBool("hasWon") == false)
        {
            // Spawns another customer and set the timer to 0 again
            SpawnCustomer();
            cooldownTimer = 0;
        }
        else
        {
            // Count up time
            cooldownTimer += Time.deltaTime;
        }
    }

    int GetCustomerCount()
    {
        // Standard is and for each customer with the tag "Customer" it will count 1+ and will return a value
        int customerCount = 0;
        GameObject[] customers = GameObject.FindGameObjectsWithTag("Customer");

        foreach (GameObject customer in customers)
        {
            Customer component = customer.GetComponent<Customer>();
            if (component.TapIndex == this.TapIndex)
            {
                customerCount++;
            }
        }

        return customerCount;
    }

    // Spawns Customer
    void SpawnCustomer()
    {
        int customerDir = IsFlipped ? -1 : 1;

        float customerOffsetX = Random.Range(Constants.MIN_OFFSET_X, Constants.MAX_OFFSET_X);

        // Spawns the GameObject
        GameObject customerObj = Instantiate(CustomerPrefab, transform.position + new Vector3(customerDir * customerOffsetX, Constants.CUSTOMER_OFFSET_Y, 0), transform.rotation);
        
        
        // Sets the values for the customer
        Customer customer = customerObj.GetComponent<Customer>();
        customer.MoveSpeed = GameManager.instance.levelManager.GetCustomerMoveSpeed();
        customer.SlideSpeed = GameManager.instance.levelManager.GetCustomerSlideSpeed();
        customer.TapIndex = this.TapIndex;
        customer.HorionztalDir = customerDir;
        
        customer.MinMoveTime = GameManager.instance.levelManager.GetMinCustomerMoveTime();
        customer.MaxMoveTime = GameManager.instance.levelManager.GetMaxCustomerMoveTime();

        customer.MinStopTime = GameManager.instance.levelManager.GetMinCustomerStopTime();
        customer.MaxStopTime = GameManager.instance.levelManager.GetMaxCustomerStopTime();

        customer.MinSlideDistance = GameManager.instance.levelManager.GetMinCustomerSlideDistance();
        customer.MaxSlideDistance = GameManager.instance.levelManager.GetMaxCustomerSlideDistance();
    }
}
