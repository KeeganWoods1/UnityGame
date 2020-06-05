using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TanOscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector;
    [SerializeField] float period;
    [Range(0, 1)] [SerializeField] float movementFactor; // 0 not moved, 1 fully moved

    Vector3 startingPos;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        Oscillate();
    }

    private void Oscillate()
    {
        if (period <= Mathf.Epsilon) { return; } // NaN guard. Epsilon to avoid float-to-float comparison uncertainty

        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2f; //2PI aka a full cycle in radians
        float rawSinWave = Mathf.Tan(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f; // raw SW oscillates between -1 and 1, divide by 2 --> -0.5 and 0.5. plus 0.5 --> 0 and 1
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
