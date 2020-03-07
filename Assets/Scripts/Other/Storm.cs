using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B83.ExpressionParser;

public class Storm : MonoBehaviour
{
    [SerializeField] private int stormCycles;
    [SerializeField] private float stormSpeed;
    [SerializeField] private string cycleVar;
    [SerializeField] public string stormSizeEquation, waitTimeEquation, damageTimeEquation, stormMaxRadiusEquation;

    private int currentStormCycle = 0;
    private float startingScale;

    void Start()
    {
        startingScale = transform.localScale.x;
        StartCoroutine(WaitForNextStorm());
    }

    private IEnumerator WaitForNextStorm()
    {
        currentStormCycle++;
        var parser = new ExpressionParser();
        Expression time = parser.EvaluateExpression(waitTimeEquation);
        time.Parameters[cycleVar].Value = currentStormCycle;

        yield return new WaitForSeconds((float)time.Value);
        StartCoroutine(ShrinkStorm());
    }

    private IEnumerator ShrinkStorm()
    {
        var parser = new ExpressionParser();
        Expression size = parser.EvaluateExpression(stormSizeEquation);
        size.Parameters[cycleVar].Value = currentStormCycle;

        float currentRadius = transform.localScale.x;
        float futureRadius = (float)size.Value * startingScale;

        var parser1 = new ExpressionParser();
        Expression maxRadius = parser1.EvaluateExpression(stormMaxRadiusEquation);
        maxRadius.Parameters[cycleVar].Value = currentStormCycle;
        maxRadius.Parameters["d"].Value = currentRadius - futureRadius;

        float theta = Random.Range(0, 2 * Mathf.PI);
        Vector3 nextDisplacement = new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta)) * Random.Range(0, (float)maxRadius.Value);

        float frames = Mathf.Floor((currentRadius - futureRadius) / stormSpeed);

        while (transform.localScale.x > futureRadius)
        {
            transform.localScale -= new Vector3(1, 0, 1) * stormSpeed;
            transform.position += nextDisplacement / frames;
            yield return 0;
        }
        StartCoroutine(WaitForNextStorm());
    }
}
