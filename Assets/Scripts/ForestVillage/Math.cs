using UnityEngine;

namespace ForestVillage
{
    public class Math
    {
        public static void RotateCharacter(float x, float y, Transform transform)
        // https://www.youtube.com/watch?app=desktop&v=xHoRkZR61JQ
        {
            var targetAngle = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
        }

        public static Quaternion RotateCharacter(float x, float y)
        // https://www.youtube.com/watch?app=desktop&v=xHoRkZR61JQ
        {
            var targetAngle = Mathf.Atan2(x, y) * Mathf.Rad2Deg;
            return Quaternion.Euler(0, targetAngle, 0);
        }
    }
}

