using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    // Player
    public const float SHIFT_DELAY = 0.4f;
    public const float RUN_SPEED = 3.5f;
    public const float SERVE_DELAY = 0.4f;
    public const float FILL_SPEED = 0.05f;
    
    public const int SHIFT_SPEED = 50;
    public const int FILL_PERCENT = 0;
    public const int FILL_X_PERCENT = 20;
    public const int FILL_FULL = 100;
    
    public const int FILL_ONE_PERCENT = 1;
    public const int FILL_49_PERCENT = 49;
    public const int FILL_50_PERCENT = 50;
    public const int FILL_99_PERCENT = 99;
    
    // Bar Exit Variables
    public const float MIN_OFFSET_X = 0.25f;
    public const float MAX_OFFSET_X = 1.2f;
    
    public const float CUSTOMER_OFFSET_Y = 0.75f;
    
    public const float SPAWN_COOLDOWN_TIMER_LVL1 = 7.8f;
    public const float SPAWN_COOLDOWN_TIMER_LVL2 = 6.6f;
    public const float SPAWN_COOLDOWN_TIMER_LVL3 = 5.8f;
    
    // Sound Management
    public const float MIN_VOLUME = 0.0001f;
    
    public const int MAX_VOLUME = 1;
    public const int DECIBEL_CONVERT = 20;
    
    // Level Management
    public const int LVL_1 = 1;
    public const int LVL_2 = 2; 
    public const int LVL_3 = 3;
    
    public const float WAVE_1 = 0.25f;
    public const float WAVE_2 = 0.5f; 
    public const int WAVE_3 = 1;
    public const float WAVE_4 = 1.25f;
    public const float WAVE_5 = 1.5f;

    public const float MOVE_SPEED_1 = 1.25f;
    public const float MOVE_SPEED_2 = 1.5f;
    public const float MOVE_SPEED_3 = 1.75f;
    
    public const float MOVE_MIN_1 = 0.5f;
    public const float MOVE_MIN_2 = 0.6f;
    public const float MOVE_MIN_3 = 0.75f;
    
    public const int MOVE_MAX_1 = 1;
    public const float MOVE_MAX_2 = 1.2f;
    public const float MOVE_MAX_3 = 1.25f;
    
    public const int STOP_MIN_1 = 2;
    public const float STOP_MIN_2 = 1.75f;
    
    public const int STOP_MAX_1 = 3;
    public const float STOP_MAX_2 = 2.5f;
    public const float STOP_MAX_3 = 2.25f;
    
    public const int DRINK_MIN_1 = 1;
    public const float DRINK_MIN_2 = 0.75f;

    public const float DRINK_MAX_1 = 1.75f;
    public const int DRINK_MAX_2 = 2;
    public const float DRINK_MAX_3 = 2.25f;

    public const int SLIDE_MIN = 3;
    public const int SLIDE_MAX = 6;
    
    public const int BARTAP_1 = 1;
    public const int BARTAP_2 = 2;
    public const int BARTAP_3 = 3;
    public const int BARTAP_4 = 4;
    
    public const int BOTTLE_SPEED = 6;

    public const int SLIDE_SPEED = 5;
    
    // Wait x Second Variables
    public const int WAIT_1_SEC = 1;
    public const int WAIT_2_SEC = 2;
    public const int WAIT_10_SEC = 10;
    public const int WAIT_15_SEC = 15;
    
    public const float WAIT_3Q_SEC = 0.75f;
    public const float WAIT_45T_SEC = 0.45f;
    
    // Binary
    public const int ZERO = 0;
    public const int ONE = 1;
    public const int MIN_ONE = -1;
    
    // Customer
    public const int PROMOTION = 20;
    public const int CUSTOMER_FIRST = 0;
    public const int CUSTOMER_LAST = 8;
    public const int CUSTOMER_LOW = 3;
    public const int CUSTOMER_MED = 6;
    public const int CUSTOMER_LAST_CHANCE = 10;
    public const int CUSTOMER_LUCKY_CHANCE = 7;
    
    public const float DISTANCE_THRESHOLD = 0.15f;
    
    // Bottle
    public const float BOTTLE_OFFSET_X_P = 1.6f;
    public const float BOTTLE_OFFSET_Y_P = 0.7f;
    public const float BOTTLE_OFFSET_X = 0.25f;
    public const float BOTTLE_OFFSET_Y = -0.35f;
    public const float BOTTLE_HIGH_Y = -0.5f;
    public const float HALF_SPEED = 0.5f;
    
    // InputManager
    public const int KEYMAPS = 2;

    // Scores
    public const int MUG_HARD = 100;
    public const int TIP_FINISH = 250;
    public const int STANDARD = 50;
    public const int UNCOMMON = 75;
    
    // Player Information
    public const int PLAYER_ONE = 1;
    public const int PLAYER_TWO = 2;
    public const int STANDARD_LIVES = 3;

    // UI
    public const int DISPLAY_TIME = 2;
    public const int FONT_BG_1 = 16;
    public const int FONT_BG_2 = 32;
    public const int FONT_BG_3 = 48;
    public const int LVL_2_NUMB = 10;
    public const int LVL_3_NUMB = 100;
    
    // Main Menu
    public const int SAMPLES = 60;
    public const int BEFORE_SHOW = 22;
    public const int AFTER_SHOW = 39;
    public const float SPEED = 0.05f;
}
