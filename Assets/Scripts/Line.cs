using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private LineRenderer _renderer;
    [SerializeField] private EdgeCollider2D _collider;

    private readonly List<Vector2> _points = new List<Vector2>();
    private readonly List<float> _timestamps = new List<float>();

    private float _startTime;

    private void Start()
    {
        _collider.transform.position -= transform.position;
        _startTime = Time.time;
    }

    private void Update()
    {
        float currentTime = Time.time;
        bool pointsRemoved = false;

        // Remover pontos com mais de 1 segundo
        while (_timestamps.Count > 0 && currentTime - _timestamps[0] > 2.5f)
        {
            _timestamps.RemoveAt(0);
            _points.RemoveAt(0);
            pointsRemoved = true;
        }

        // Atualizar visual e colisão se houve alteração
        if (pointsRemoved)
        {
            _renderer.positionCount = _points.Count;
            _renderer.SetPositions(_points.ConvertAll(p => (Vector3)p).ToArray());

            _collider.points = _points.ToArray();
        }

        // Destruir linha se não restar nada
        if (_points.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetPosition(Vector2 pos)
    {
        if (!CanAppend(pos)) return;

        _points.Add(pos);
        _timestamps.Add(Time.time);

        _renderer.positionCount = _points.Count;
        _renderer.SetPosition(_renderer.positionCount - 1, pos);

        _collider.points = _points.ToArray();
    }

    private bool CanAppend(Vector2 pos)
    {
        if (_points.Count == 0) return true;

        return Vector2.Distance(_points[_points.Count - 1], pos) > DrawManager.RESOLUTION;
    }
}