using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignerAssets;

public class LevelManagement : MonoBehaviour
{
    //This class will automatically create and switch between level objects

    public int timeLimit = 120; //seconds of the level

    public const int numLevels = 6;
    public iLevel theLevel;

    public static string BASEVEHICLECONFIG = "*aMM0+++++*bNM2+++*cMN1+++*dLM2+++*eML1+++^ab^ac^ad^ae,5,3";

    public UAVDesigner uavd;

    public int[,] criteria = new int[numLevels, 4]
    {
        //range, capacity, cost, velocity
        {10, 5, 3470, 20},
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

    void Awake()
    {
        uavd = GameObject.FindObjectOfType(typeof(UAVDesigner)) as UAVDesigner;
        theLevel = new iLevel(0);
        Debug.Log("Started!");
        StartCoroutine(levelWait());
    }

    public void levelEnd()
    {
        theLevel = new iLevel(theLevel.level + 1);
        Debug.Log("Level! " + theLevel.level);
        StartCoroutine(levelWait());
    }

    //Internal Level Class
    public class iLevel
    {
        public int level;
        public int speed;
        public int cost;
        public int distance;
        // this class can handle some logging, criteria, etc
        public iLevel(int level)
        {
            this.level = level;
        }
    }

    IEnumerator levelWait()
    {
        //run level
        uavd.Initialize(BASEVEHICLECONFIG, true, criteria[theLevel.level,1]);
        yield return new WaitForSeconds(timeLimit);
        levelEnd();
    }

}
