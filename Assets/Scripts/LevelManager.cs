using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // All Callables for the LevelManagement
    public LevelSettings CurrentLevel;

    public List<LevelSettings> AllLevels;

    // Bools to check if the Player made a Mistake
    public bool PlayerMissedCustomer;
    public bool PlayerMissedEmptyMug;
    public bool PlayerThrewExtraMug;

    // Standard Level is 1
    public int level = Constants.ONE;
    
    // On Awake Define Level Settings
    void Awake()
    {
        InitAllLevelSettings();
    }
    
    // Update is called once per frame
    void Update()
    {
        // Show the Bar Tabs at all Time
        ShowAllBarTaps();
    }
    

    private void ShowAllBarTaps()
    {
        // Set every BarTap available on Enabled
        foreach (BarTap tap in GetBarTaps())
        {
            tap.GetComponent<SpriteRenderer>().enabled = true;   
        }
    }

    // Check at which BarTap Player is
    public bool IsPlayerAtBarTap(int tapIndex)
    {
        return GetBarTapAtTapIndex(tapIndex).IsPlayerAtTap;
    }
    
    // Get every BarTap Script of Every BarTap
    public List<BarTap> GetBarTaps()
    {
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("BarTap");
        List<BarTap> taps = new List<BarTap>();

        foreach (GameObject obj in taggedObjects)
        {
            taps.Add(obj.GetComponent<BarTap>());
        }

        return taps;
    }

    // Get Bar of the Tap Index
    public BarTap GetBarTapAtTapIndex(int tapIndex)
    {
        List<BarTap> availableTaps = GetBarTaps();

        return availableTaps.Where(t => t.TapIndex == tapIndex).FirstOrDefault();
    }

    // Get the First BarTap
    public BarTap GetFirstBarTap()
    {
        List<BarTap> availableTaps = GetBarTaps();

        return availableTaps.Where(t => t.TapIndex == Constants.ONE).FirstOrDefault();
    }

    // Get the Last BarTap
    public BarTap GetLastBarTap()
    {
        List<BarTap> availableTaps = GetBarTaps();

        int maxIndex = availableTaps.Max(t => t.TapIndex);

        return availableTaps.Where(t => t.TapIndex == maxIndex).FirstOrDefault();
    }

    // Get and Return the Player Bottle Speed Value
    public float GetPlayerBeerSpeed()
    {
        return CurrentLevel.PlayerBeerSpeed;
    }

    // Get and Return the Customer Move Speed Value
    public float GetCustomerMoveSpeed()
    {
        return CurrentLevel.CustomerMoveSpeed;
    }

    // Get and Return the Customer Slide Speed Value
    public float GetCustomerSlideSpeed()
    {
        return CurrentLevel.CustomerSlideSpeed;
    }

    // Get and Return the Customer Minimal Stop Time Value
    public float GetMinCustomerStopTime()
    {
        return CurrentLevel.CustomerStopTimes[Constants.ZERO];
    }

    // Get and Return the Customer Maximal Stop Time Value
    public float GetMaxCustomerStopTime()
    {
        return CurrentLevel.CustomerStopTimes[Constants.ONE];
    }

    // Get and Return the Customer Minimal Move Time Value
    public float GetMinCustomerMoveTime()
    {
        return CurrentLevel.CustomerMoveTimes[Constants.ZERO];
    }

    // Get and Return the Customer Maximal Move Time Value
    public float GetMaxCustomerMoveTime()
    {
        return CurrentLevel.CustomerMoveTimes[Constants.ONE];
    }

    // Get and Return the Customer Minimal Slide Distance Value
    public float GetMinCustomerSlideDistance()
    {
        return CurrentLevel.CustomerSlideDistances[Constants.ZERO];
    }

    // Get and Return the Customer Maximal Slide Distance Value
    public float GetMaxCustomerSlideDistance()
    {
        return CurrentLevel.CustomerSlideDistances[Constants.ONE];
    }

    // Set Per Level Information
    private void InitAllLevelSettings()
    {
        AllLevels = new List<LevelSettings>();

        // Level 1 Information
        LevelSettings level1 = new LevelSettings()
        {
            Level = Constants.LVL_1,
            PlayerBeerSpeed = Constants.BOTTLE_SPEED,
            CustomerMoveSpeed = Constants.MOVE_SPEED_1,
            CustomerSlideSpeed = Constants.SLIDE_SPEED
        };
        level1.SetCustomerMoveTimes(Constants.MOVE_MIN_1, Constants.MOVE_MAX_1);
        level1.SetCustomerStopTimes(Constants.STOP_MIN_1, Constants.STOP_MAX_1);
        level1.SetCustomerDrinkTimes(Constants.DRINK_MIN_1, Constants.DRINK_MAX_1);
        level1.SetCustomerSlideDistances(Constants.SLIDE_MIN, Constants.SLIDE_MAX);

        level1.AddCustomersToBarTap(Constants.BARTAP_1, new List<float>() { Constants.WAVE_1 });
        level1.AddCustomersToBarTap(Constants.BARTAP_2, new List<float>() { Constants.WAVE_1 });
        level1.AddCustomersToBarTap(Constants.BARTAP_3, new List<float>() { Constants.WAVE_1 });
        level1.AddCustomersToBarTap(Constants.BARTAP_4, new List<float>() { Constants.WAVE_1 });

        AllLevels.Add(level1);

        // Level 2 Information
        LevelSettings level2 = new LevelSettings()
        {
            Level = Constants.LVL_2,
            PlayerBeerSpeed = Constants.BOTTLE_SPEED,
            CustomerMoveSpeed = Constants.MOVE_SPEED_2,
            CustomerSlideSpeed = Constants.SLIDE_SPEED
        };
        level2.SetCustomerMoveTimes(Constants.MOVE_MIN_2, Constants.MOVE_MAX_2);
        level2.SetCustomerStopTimes(Constants.STOP_MIN_1, Constants.STOP_MAX_2);
        level2.SetCustomerDrinkTimes(Constants.DRINK_MIN_1, Constants.DRINK_MAX_2);
        level2.SetCustomerSlideDistances(Constants.SLIDE_MIN, Constants.SLIDE_MAX);

        level2.AddCustomersToBarTap(Constants.BARTAP_1, new List<float>() { Constants.WAVE_1, Constants.WAVE_3 });
        level2.AddCustomersToBarTap(Constants.BARTAP_2, new List<float>() { Constants.WAVE_1, Constants.WAVE_3 });
        level2.AddCustomersToBarTap(Constants.BARTAP_3, new List<float>() { Constants.WAVE_1, Constants.WAVE_3 });
        level2.AddCustomersToBarTap(Constants.BARTAP_4, new List<float>() { Constants.WAVE_1, Constants.WAVE_3 });

        AllLevels.Add(level2);
        
        // Level 3 Information
        LevelSettings level3 = new LevelSettings()
        {
            Level = Constants.LVL_3,
            PlayerBeerSpeed = Constants.BOTTLE_SPEED,
            CustomerMoveSpeed = Constants.MOVE_SPEED_3,
            CustomerSlideSpeed = Constants.SLIDE_SPEED
        };
        level3.SetCustomerMoveTimes(Constants.MOVE_MIN_3, Constants.MOVE_MAX_3);
        level3.SetCustomerStopTimes(Constants.STOP_MIN_2, Constants.STOP_MAX_3);
        level3.SetCustomerDrinkTimes(Constants.DRINK_MIN_2, Constants.DRINK_MAX_3);
        level3.SetCustomerSlideDistances(Constants.SLIDE_MIN, Constants.SLIDE_MAX);

        level3.AddCustomersToBarTap(Constants.BARTAP_1, new List<float>() { Constants.WAVE_2, Constants.WAVE_4, Constants.WAVE_5 });
        level3.AddCustomersToBarTap(Constants.BARTAP_2, new List<float>() { Constants.WAVE_2, Constants.WAVE_4, Constants.WAVE_5 });
        level3.AddCustomersToBarTap(Constants.BARTAP_3, new List<float>() { Constants.WAVE_2, Constants.WAVE_4, Constants.WAVE_5 });
        level3.AddCustomersToBarTap(Constants.BARTAP_4, new List<float>() { Constants.WAVE_2, Constants.WAVE_4, Constants.WAVE_5 });

        AllLevels.Add(level3);
    }
}

// Sets the Settings for Current Level
public class LevelSettings
{
    public int Level;

    public Dictionary<int, List<float>> StartingCustomers;

    public float PlayerBeerSpeed = Constants.BOTTLE_SPEED;

    public float CustomerMoveSpeed;

    public float CustomerSlideSpeed;

    public List<float> CustomerMoveTimes;

    public List<float> CustomerStopTimes; 

    public List<float> CustomerDrinkTimes;

    public List<float> CustomerSlideDistances;

    // Sets the new Customer Move Times
    public void SetCustomerMoveTimes(float min, float max)
    {
        CustomerMoveTimes = new List<float>();
        CustomerMoveTimes.Add(min);
        CustomerMoveTimes.Add(max);
    }

    // Sets the new Customer Stop Times
    public void SetCustomerStopTimes(float min, float max)
    {
        CustomerStopTimes = new List<float>();
        CustomerStopTimes.Add(min);
        CustomerStopTimes.Add(max);
    }

    // Sets the new Custome Drink Times
    public void SetCustomerDrinkTimes(float min, float max)
    {
        CustomerDrinkTimes = new List<float>();
        CustomerDrinkTimes.Add(min);
        CustomerDrinkTimes.Add(max);
    }

    // Sets the new Customer Slide Distances
    public void SetCustomerSlideDistances(float min, float max)
    {
        CustomerSlideDistances = new List<float>();
        CustomerSlideDistances.Add(min);
        CustomerSlideDistances.Add(max);
    }

    // Sets the First Customers
    public LevelSettings()
    {
        StartingCustomers = new Dictionary<int, List<float>>();
    }

    // Adds Customer to the Correct Tap and Sets Position
    public void AddCustomersToBarTap(int bartap_index, List<float> customer_offsets)
    {
        if (StartingCustomers == null)
        {
            StartingCustomers = new Dictionary<int, List<float>>();
        }

        if (StartingCustomers.ContainsKey(bartap_index) == false)
        {
            StartingCustomers.Add(bartap_index, customer_offsets);
        }
    }
}