using UnityEngine;

namespace ErmineGames.Utils
{
    public static class GameObjectUtils
    {
        public static void DestroyAllChildren(this GameObject gameObject) 
        {
            foreach(Transform child in gameObject.transform)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}
