namespace Common;

public abstract class Singleton<T> where T : class, new()
{
    private static T single = null;
    public static T Single
    {
        get
        {
            if (single == null)
                single = new T();
            return single;
        }
    }
}