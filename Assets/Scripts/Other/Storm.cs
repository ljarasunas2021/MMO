using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using B83.ExpressionParser;

public class Storm : MonoBehaviour
{
    [SerializeField] private int stormCycles;
    [SerializeField] private string cycleVar;
    [SerializeField] public string stormSpeedEquation, stormSizeEquation, waitTimeEquation, damageTimeEquation, stormMaxRadiusEquation;

    private int currentStormCycle = 0, fps = 30;
    private float startingScale;
    private StormTimer stormTimer;

    [HideInInspector] public float damage;

    private void Start()
    {
        stormTimer = GameObject.FindObjectOfType<StormTimer>();
    }

    public void StartStorm()
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

        var parser1 = new ExpressionParser();
        Expression damageExpression = parser.EvaluateExpression(damageTimeEquation);
        damageExpression.Parameters[cycleVar].Value = currentStormCycle;
        damage = (float)damageExpression.Value;

        int minLeft = (int)Mathf.Floor((float)time.Value / 60);
        int secLeft = (int)time.Value % 60;

        while (minLeft >= 0)
        {
            if (secLeft > 0) secLeft -= 1;
            else
            {
                minLeft -= 1;
                secLeft = 59;
            }
            stormTimer.SetTime(minLeft, secLeft);
            yield return new WaitForSeconds(1);
        }

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

        var parser2 = new ExpressionParser();
        Expression stormSpeed = parser2.EvaluateExpression(stormSpeedEquation);
        stormSpeed.Parameters[cycleVar].Value = currentStormCycle;

        float theta = Random.Range(0, 2 * Mathf.PI);
        Vector3 nextDisplacement = new Vector3(Mathf.Sin(theta), 0, Mathf.Cos(theta)) * Random.Range(0, (float)maxRadius.Value);

        float stormSpeedValue = (float)stormSpeed.Value;

        float seconds = Mathf.Floor((currentRadius - futureRadius) / stormSpeedValue);

        int minLeft = (int)Mathf.Floor((seconds) / 60);
        int secLeft = (int)Mathf.Floor((seconds) % 60);
        stormTimer.SetTime(minLeft, secLeft);

        float frameCount = ((seconds) % 60) % 1;

        while (minLeft >= 0)
        {
            if (frameCount <= 0)
            {
                if (secLeft > 0) secLeft -= 1;
                else
                {
                    minLeft -= 1;
                    secLeft = 59;
                }
                stormTimer.SetTime(minLeft, secLeft);
                frameCount = 1;
            }

            transform.localScale -= new Vector3(1, 0, 1) * stormSpeedValue * Time.deltaTime;
            transform.position += nextDisplacement * Time.deltaTime / seconds;
            frameCount -= Time.deltaTime;

            yield return 0;
        }

        StartCoroutine(WaitForNextStorm());
    }
}
