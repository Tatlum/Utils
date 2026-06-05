using UnityEngine;

namespace ErmineGames.Utils
{
    public static class GameObjectUtils
    {
        public static void DestroyAllChildren(this GameObject gameObject) 
        {
            if (gameObject == null)
            {
                return;
            }

            var transform = gameObject.transform;
            
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
}
