namespace FIFAWebAppWithWorkers.Shared;

public class Utils
{
    public static int GetNextPrice(int currentPrice)
    {
        if (currentPrice < 1000) return currentPrice += 50;
        if (currentPrice < 10000) return currentPrice += 100;
        if (currentPrice < 50000) return currentPrice += 250;
        if (currentPrice < 100000) return currentPrice += 500;
        return currentPrice += 1000;
    }

    public static int GetPreviousPrice(int currentPrice)
    {
        if (currentPrice <= 1000) return currentPrice -= 50;
        if (currentPrice <= 10000) return currentPrice -= 100;
        if (currentPrice <= 50000) return currentPrice -= 250;
        if (currentPrice <= 100000) return currentPrice -= 500;
        return currentPrice -= 1000;
    }
}
