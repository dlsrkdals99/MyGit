using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuzzyRule
{
    public FuzzySet Input1 { get; set; }
    public FuzzySet Input2 { get; set; }
    public FuzzySet Output { get; set; }

    public FuzzyRule(FuzzySet input1, FuzzySet input2, FuzzySet output)
    {
        Input1 = input1;
        Input2 = input2;
        Output = output;
    }

    public float Evaluate(float value1, float value2)
    {
        return Mathf.Min(Input1.GetMembership(value1), Input2.GetMembership(value2));
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
