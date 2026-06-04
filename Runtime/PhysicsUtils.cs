using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ErmineGames.Utils
{
    public static class PhysicsUtils
    {
        private const float VelocityConversionCoefficient = 3.6f;

        public static float ToKmH(float velocity)
        {
            return velocity * VelocityConversionCoefficient;
        }

        public static float ToMSec(float velocity)
        {
            return velocity / VelocityConversionCoefficient;
        }
        
        public static float CalculateAccelerationTime(float startVelocity, float endVelocity, float acceleration)
        {
            if (Mathf.Approximately(acceleration, 0f))
            {
                return 0f;
            }
            
            return (endVelocity - startVelocity) / acceleration;
        }

        public static Vector3 GetRandomPoint(this Collider collider, Vector3 minEdgeOffset = new())
        {
            return collider switch
            {
                BoxCollider boxCollider => boxCollider.GetRandomPoint(minEdgeOffset),
                SphereCollider sphereCollider => sphereCollider.GetRandomPoint(minEdgeOffset.x),
                _ => throw new ArgumentException("Unsupported collider type")
            };
        }

        public static Vector3 GetRandomPoint(this BoxCollider collider, Vector3 minEdgeOffset = default)
        {
            var extents = collider.size / 2f;
            extents.x = Mathf.Max(0f, extents.x - minEdgeOffset.x);
            extents.y = Mathf.Max(0f, extents.y - minEdgeOffset.y);
            extents.z = Mathf.Max(0f, extents.z - minEdgeOffset.z);
            
            var localPoint = new Vector3(
                Random.Range(-extents.x, extents.x),
                Random.Range(-extents.y, extents.y),
                Random.Range(-extents.z, extents.z)
            );
            
            localPoint += collider.center;
            
            return collider.transform.TransformPoint(localPoint);
        }

        public static Vector3 GetRandomPoint(this SphereCollider collider, float minEdgeOffset = 0f)
        {
            var radius = Mathf.Max(0f, collider.radius - minEdgeOffset);
            var localPoint = Random.insideUnitSphere * radius;
            localPoint += collider.center;

            return collider.transform.TransformPoint(localPoint);
        }
    }
}
