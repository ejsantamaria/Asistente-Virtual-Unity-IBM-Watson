using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    
    public static T instance 
    {
        get 
        {
            if (_instance == null) 
            {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }
}