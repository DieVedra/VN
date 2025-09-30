using UnityEngine;

public class PhoneContactsColors
{
    private const float _r1 = 0.7662924f;
    private const float _g1 = 0.7921569f;
    private const float _b1 = 0.3033961f;

    private const float _r2 = 0.3033961f;
    private const float _g2 = 0.7196182f;
    private const float _b2 = 0.7921569f;
    
    private const float _r3 = 0.252698f;
    private const float _g3 = 0.7921569f;
    private const float _b3 = 0.3592866f;
    
    private const float _r4 = 0.7921569f;
    private const float _g4 = 0.6236352f;
    private const float _b4 = 0.2828f;
    
    private const float _r5 = 0.7921569f;
    private const float _g5 = 0.3929098f;
    private const float _b5 = 0.6923451f;

    private const int _minNumber = 1;
    private const int _maxNumber = 6;

    private static int _previousNumber1 = 0;
    private static int _previousNumber2 = 0;
    private static bool _key = false;
    public static Color GetColor()
    {
        switch (GenerateRandomNumber())
        {
            case 1:
                return new Color(_r1, _g1, _b1);
            case 2:
                return new Color(_r2, _g2, _b2);
            case 3:
                return new Color(_r3, _g3, _b3);
            case 4:
                return new Color(_r4, _g4, _b4);
            case 5:
                return new Color(_r5, _g5, _b5);
            default:
                return new Color(_r1, _g1, _b1);
        }
    }

    private static int GenerateRandomNumber()
    {
        int number = Random.Range(_minNumber, _maxNumber);
        if (number == _previousNumber1 && number == _previousNumber2)
        {
            number = GenerateRandomNumber();
        }

        if (_key == false)
        {
            _key = true;
            _previousNumber1 = number;
        }
        else
        {
            _key = false;
            _previousNumber2 = number;
        }
        return number;
    }
}