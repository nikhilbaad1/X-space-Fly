using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oscillator : MonoBehaviour
{
    public Vector3 movementVector=new Vector3(10f,10f,10f);
    public Vector3 startingposition;
    [Range(0, 1)]
    public float movementFactor;
    float period = 2f;
    const float tau = Mathf.PI * 2f;//2 pi radian
    void Start()
    {
        startingposition = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period;
        float rawSinWave = Mathf.Sin(cycles*tau);
        movementFactor = rawSinWave / 2f  +0.5f;
        Vector3 offset = movementVector * movementFactor;
        this.transform.position = startingposition + offset;

    }
}
