using System;
using UnityEngine;
using UnityEngine.Events;


public class Score : MonoBehaviour
{
    [SerializeField] private string _scoreDisplayName = "Score";
    [SerializeField] private float _initialValue = 0;
    [SerializeField] private float _maxValue = float.MaxValue;
    [SerializeField] private float _minValue = float.MinValue;
    [SerializeField] private bool _saveValue = false;

    [HideInInspector] public UnityEvent<float> OnScoreZero;

    /// <summary>
    /// float deltaChange, float valueBefore
    /// </summary>
    [HideInInspector] public UnityEvent<float, float> OnChangeValue;

    [HideInInspector] public UnityEvent<float, float, bool> OnChangeMaxValue;
    [HideInInspector] public UnityEvent<float, float, bool> OnChangeMinValue;

    private float _currentValue;

    private string SavePrefix => "Score." + _scoreDisplayName + ".";

    public float CurrentValue => _currentValue;
    public float MaxValue => _maxValue;
    public float MinValue => _minValue;

    public string ScoreDisplayName => _scoreDisplayName;

    public void Initialize(bool save = false)
    {
        _saveValue = save;
        ChangeValue(_initialValue);
    }

    public void Initialize(string displayName, bool save = false)
    {
        _scoreDisplayName = displayName;
        _saveValue = save;
        ChangeValue(_initialValue);
    }

    public void Initialize(string displayName, float initial, float max, float min, bool save = false)
    {
        _initialValue = initial;
        _maxValue = max;
        _minValue = min;
        _saveValue = save;
        _scoreDisplayName = displayName;
        ChangeValue(_initialValue);
    }

    public void SetScoreToZero()
    {
        ReduceScore(_currentValue);
    }

    public void AddScore(float gainAmount)
    {
        ChangeValue(Math.Abs(gainAmount));
    }

    public void ReduceScore(float lossAmount)
    {
        ChangeValue(-Math.Abs(lossAmount));
    }

    public void ChangeMaxValue(float newMaxValue, bool changeToMax = false)
    {
        float deltaChange = newMaxValue - _maxValue;
        _maxValue = newMaxValue;
        OnChangeMaxValue.Invoke(newMaxValue, _currentValue, changeToMax);
        if (changeToMax)
            ChangeValue(deltaChange);
    }

    public void ChangeMinValue(float newMinValue, bool changeToMin = false)
    {
        float deltaChange = newMinValue - _minValue;
        _minValue = newMinValue;
        OnChangeMinValue.Invoke(newMinValue, _currentValue, changeToMin);
        if (changeToMin)
            ChangeValue(deltaChange);
    }

    public void ChangeValue(float changeValue)
    {
        if (!Application.isPlaying) return;

        //lost
        if (changeValue < 0)
        {
            Debug.Log(_scoreDisplayName + " reduced by " + changeValue);
        }
        else if (changeValue > 0) //gain
        {
            Debug.Log(_scoreDisplayName + " increased by " + changeValue);
        }
        else return;

        //Change current value
        _currentValue += changeValue;

        //Avoid negative number and number above max value
        if (_currentValue > _maxValue) _currentValue = _maxValue;
        if (_currentValue < _minValue) _currentValue = _minValue;

        //Call OnChangeValue event
        OnChangeValue.Invoke(changeValue, _currentValue);

        //Handle zero
        if (_currentValue <= 0 && _minValue == 0)
        {
            Debug.Log(_scoreDisplayName + " reached zero");
            OnScoreZero.Invoke(_currentValue);
        }
    }

    public void SetScore(float newScore)
    {
        ChangeValue(newScore - _currentValue);
    }
}