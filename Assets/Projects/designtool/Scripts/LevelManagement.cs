using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignerAssets;

public class LevelManagement : MonoBehaviour
{
    //This class will automatically create and switch between level objects

    public int timeLimit = 11; //time limit (in seconds)
    public DateTime dt;
    public DateTime sdt;
    public int state;
    public float theTime;
    public int theSecond;
    public float startTime;
    bool overriden = false;

    public const int numLevels = 6;
    public iLevel theLevel;
    private bool asleep = true;

    public static string BASEVEHICLECONFIG = "*aMM0+++++*bNM2+++*cMN1+++*dLM2+++*eML1+++^ab^ac^ad^ae,5,3";

    public UAVDesigner uavd;

    public int[,] criteria = new int[numLevels, 4]
    {
        //range, capacity, cost, velocity
        {9, 5, 3370, 18},
        {0, 15, 0, 0},
        {0, 10, 0, 0},
        {0, 7, 0, 0},
        {0, 20, 0, 0},
        {0, 23, 0, 0}
    };
        

    public void randomizeCriteria()
    {
    // should return random speed, cost, distance, but with same seed
    // OR we just have a list of the levels we manually create
    }

    public void optimalSolution()
    {
        // AI generated optimal solution? Or just let them get within limit of reaching criteria?
    }

    //finds uavd and passes awake function for title screen button
    void Awake()
    {
        uavd = GameObject.FindObjectOfType(typeof(UAVDesigner)) as UAVDesigner;
        Action funcPointer = null;
        funcPointer = secondAwake;
        Action endEarly = null;
        endEarly = EndEarly;
        uavd.awakeFunc(funcPointer, endEarly);
    }

    //second awake to start levels
    void secondAwake()
    {
        
        theLevel = new iLevel(0);
        Debug.Log("Started!");
        dt = DateTime.Now;
        dt = dt.AddSeconds(timeLimit - 10); //start of level
        StartLevel();
        state = 0;
        asleep = false;
    }

    void Update()
    {
        if (!asleep)
        {
            TimeSpan ts = DateTime.Now.Subtract(sdt);
            if (state != -1)
            {
                uavd.UpdateSecond((ts.Seconds + 60 * ts.Minutes));
            }
            if (overriden)
            {
                if (state == 0)
                {
                    Debug.Log("10 Seconds Left");
                    Warning10Seconds();
                    dt = dt.AddSeconds(10);
                    state = 1;
                }
                else if (state == 1)
                {
                    Debug.Log("Level End!");
                    if (theLevel.level > numLevels)
                    {
                        //display end title screen
                        uavd.end();
                        state = -1;
                    }
                    else
                    {
                        EndLevel();
                        //pass criteria and resume function pointer to uavd
                        int[] theCriteria = new int[4] { criteria[theLevel.level, 0], criteria[theLevel.level, 1], criteria[theLevel.level, 2], criteria[theLevel.level, 3] };
                        Action funcPointer = null;
                        funcPointer = ResumeLevel;
                        uavd.pause(theCriteria, funcPointer);
                        state = -1;
                    }
                }
                else if (state == 2)
                {
                    Debug.Log("Start of Level");
                    StartLevel();
                    state = 0;
                    overriden = false;
                }
            }
        }
    }

    //calls Initialize() in UAVDesigner
    //resets time
    public void StartLevel()
    {
        int[] theCriteria = new int[4] { criteria[theLevel.level, 0], criteria[theLevel.level, 1], criteria[theLevel.level, 2], criteria[theLevel.level, 3] };
        uavd.Initialize(BASEVEHICLECONFIG, true, theCriteria, theLevel.level);
        dt = DateTime.Now;
        dt = dt.AddSeconds(timeLimit - 10);
        sdt = DateTime.Now;
        theSecond = 0;
    }

    //creates new iLevel at end of level
    public void EndLevel()
    {
        theLevel = new iLevel(theLevel.level + 1);
    }

    //resets state to 2 as a callback function in resume() in UAVDesigner
    public void ResumeLevel()
    {
        state = 2;
    }

    //called when there are 10 seconds remaining
    public void Warning10Seconds()
    {
        uavd.Warning10Seconds();
    }

    public void EndEarly()
    {
        state = 1;
        overriden = true;
    }

    //Internal Level Class
    public class iLevel
    {
        public int level;
        public int speed;
        public int cost;
        public int distance;
        public iLevel(int level)
        {
            this.level = level;
        }
    }
    void OnDestroy()
    {
        Debug.Log("Destroyed");
    }
}
