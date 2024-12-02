using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzySet
{
    public float Min { get; set; }
    public float Mid { get; set; }
    public float Max { get; set; }

    public FuzzySet(float min, float mid, float max)
    {
        Min = min;
        Mid = mid;
        Max = max;
    }

    public float GetMembership(float value)
    {
        if (value <= Min || value >= Max)
            return 0.0f;
        else if (value == Mid)
            return 1.0f;
        else if (value > Min && value < Mid)
            return (value - Min) / (Mid - Min);
        else
            return (Max - value) / (Max - Mid);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
