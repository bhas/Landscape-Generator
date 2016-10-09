using UnityEngine;

public class Modulus : MonoBehaviour {
    private float number = 10;
    private float size = 10;
    private float result;
    private GameObject objecting;
    public float bla;

    void Start()
    {
        objecting = new GameObject();
    }

    void OnValidate()
    {
        objecting.SetActive(false);
    }

	void OnValidate2()
    {
        if (number < 0)
        {
            result = (size + number) % size;
        }
        else
        {
            result = number % size;
        }
    }
}