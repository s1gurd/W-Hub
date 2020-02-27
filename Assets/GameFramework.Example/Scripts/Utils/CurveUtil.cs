using GameFramework.Example.Common;

namespace GameFramework.Example.Utils
{
    public static class CurveUtil
    {
        public static float Evaluate(this Curve curve, float t)
        {
            t = MathUtils.Clamp(t, 0f, 1f);
            
            float distanceTime = curve.keyframe1.time - curve.keyframe0.time;
 
            float m0 = curve.keyframe0.outTangent * distanceTime;
            float m1 = curve.keyframe1.inTangent * distanceTime;
 
            float t2 = t * t;
            float t3 = t2 * t;
 
            float a = 2 * t3 - 3 * t2 + 1;
            float b = t3 - 2 * t2 + t;
            float c = t3 - t2;
            float d = -2 * t3 + 3 * t2;
 
            return a * curve.keyframe0.value + b * m0 + c * m1 + d * curve.keyframe1.value;
        }
    }
}