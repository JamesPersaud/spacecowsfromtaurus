using UnityEngine;

public class PRNG
{
    private int savedSeed = 0;
    private Random random = new Random();

    public int GetSeed()
    {
        return savedSeed;
    }

    public PRNG()
    {
        //Random.seed = new System.Random().Next();
    }

    public void Seed(int seed)
    {
        savedSeed = seed;
        Random.seed = seed;
    }

    public int GetInt(int min_inclusive, int max_inclusive)
    {
        return Random.Range(min_inclusive, max_inclusive +1);           
    }

    public float GetFloat()
    {    
        return Random.value;
    }

    public float GetFloat(float lowerBoundary, float upperBoundary)
    {
        return Random.Range(lowerBoundary, upperBoundary);
    }

    public double GetDouble()
    {
        return (double)Random.value;
    }
}

