using UnityEngine;

public class Line : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Debug.Log(this_name + "が消えました");
            Destroy(this.gameObject);
        }
    }
}
