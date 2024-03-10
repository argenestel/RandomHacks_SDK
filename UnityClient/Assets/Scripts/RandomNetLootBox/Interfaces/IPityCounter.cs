using UnityEngine;

public interface IPityCounter
{
    void Reset();
    void Increase();
    bool IsMax();
}

public class PityCounter : IPityCounter
{
    private int counter = 0;
    private readonly int maxCounter;

    public PityCounter(int maxCounter)
    {
        this.maxCounter = maxCounter;
    }

    public void Reset()
    {
        counter = 0;
    }

    public void Increase()
    {
        counter++;
        if (counter >= maxCounter)
        {
            Reset();
        }
    }

    public bool IsMax()
    {
        return counter >= maxCounter;
    }
}