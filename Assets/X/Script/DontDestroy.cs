using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static GameObject[] persistenObjects = new GameObject[3];
    public int objectIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (persistenObjects[objectIndex] == null)
        {
            persistenObjects[objectIndex] = gameObject;
            DontDestroyOnLoad(gameObject);
        }

        else if (persistenObjects[objectIndex] != gameObject)
        {
            Destroy(gameObject);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
