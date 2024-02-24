using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagement : MonoBehaviour
{
    //This class will automatically create and switch between level objects

    public int level;
    public int timeLimit;

    public int numLevels;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void randomizeCriteria()
    {
// should return random speed, cost, distance, but with same seed
    }

    public void optimalSolution()
    {
// AI generated optimal solution?? Or just let them get within limit?
    }

    //Internal Level Class
    public class iLevel
    {
        public int level;
        public int speed;
        public int cost;
        public int distance;
        //timer, calls parent switch level?
    }
}
