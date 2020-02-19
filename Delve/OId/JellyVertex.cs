using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyVertex
{
    public int verticeIndex;
    public Vector3 initialVertexPosition;
    public Vector3 currentVertexPosition;

    public Vector3 currentVelocity;

    public JellyVertex(int _verticeIndex, Vector3 _initialVertexPosition, Vector3 _currentVertexPosition, Vector3 _currentVelocity)
    {

        verticeIndex = _verticeIndex;
        initialVertexPosition = _initialVertexPosition;
        currentVertexPosition = _currentVertexPosition;
        currentVelocity = _currentVelocity;

    }

    public Vector3 GetCurrentDisplacement()
    {

        return currentVertexPosition - initialVertexPosition;

    }

    public void UpdateVelocity(float _bounceSpeed)
    {

        currentVelocity = currentVelocity - GetCurrentDisplacement() * _bounceSpeed * Time.fixedDeltaTime;

    }

    public void Settle(float _stiffness)
    {

        currentVelocity *= 1f - _stiffness * Time.fixedDeltaTime;

    }

    public void ApplyPressureToVertex(Transform _transform, Vector3 _position, float _pressure)
    {

        Vector3 distanceVerticePoint = currentVertexPosition - _transform.InverseTransformPoint(_position);
        float adaptedPressure = _pressure / (1f + distanceVerticePoint.sqrMagnitude);
        float velocity = adaptedPressure * Time.fixedDeltaTime;
        currentVelocity += distanceVerticePoint.normalized * velocity;

    }
}
