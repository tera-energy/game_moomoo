using UnityEngine;

public class TrWanderAnimal : MonoBehaviour
{
    Vector2[] _target;
    float _speed;
    float _currValue;
    float _beforeX;
    ParticleSystem _ps;

    bool _isOrderMove;

    public void zSetMove(Vector2[] pos, float speed){
        _target = pos;
        _speed = speed;
        _currValue = 0;
        _isOrderMove = true;
        _ps.Play();
    }

    private void Awake(){
        _ps = GetComponentInChildren<ParticleSystem>();
        _isOrderMove = false;
        _target = new Vector2[4];
        _ps.Stop();
    }


    void FixedUpdate()
    {
        if (!GameManager.xInstance._isGameStarted || !_isOrderMove) return;
        _currValue += Time.fixedDeltaTime * _speed;
        transform.position = TT.zBezierCurve(_target[0], _target[1], _target[2], _target[3], _currValue);

        if (transform.position.x < _beforeX)
            transform.localScale = new Vector3(1, 1, 1);
        //_sr.flipX = false;
        else if (transform.position.x > _beforeX)
            transform.localScale = new Vector3(-1, 1, 1);
        //_sr.flipX = true;
        _beforeX = transform.position.x;

        if(_currValue >= 1f){
            _isOrderMove = false;
            TrPuzzleAnimals.xInstance.zSetWanderMove();
            _ps.Stop();
        }
    }
}

/*[CanEditMultipleObjects]
[CustomEditor(typeof(TrWanderAnimal))]
public class LerpTest : Editor
{
    private void OnSceneGUI()
    {
        TrWanderAnimal dot = (TrWanderAnimal)target;

        dot._target[0] = Handles.PositionHandle(dot._target[0], Quaternion.identity);
        dot._target[1] = Handles.PositionHandle(dot._target[1], Quaternion.identity);
        dot._target[2] = Handles.PositionHandle(dot._target[2], Quaternion.identity);
        dot._target[3] = Handles.PositionHandle(dot._target[3], Quaternion.identity);


        Handles.DrawLine(dot._target[0], dot._target[1]);
        Handles.DrawLine(dot._target[2], dot._target[3]);


        int count = 50;
        for (int i = 0; i < count; i++)
        {
            float value_Before = (float)i / count;
            Vector3 before = TT.zBezierCurve(dot._target[0], dot._target[1], dot._target[2], dot._target[3], value_Before);
            float value_After = (float)(i + 1) / count;
            Vector3 after = TT.zBezierCurve(dot._target[0], dot._target[1], dot._target[2], dot._target[3], value_After);

            Handles.DrawLine(before, after);
        }
    }
}*/