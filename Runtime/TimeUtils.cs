namespace ErmineGames.Utils
{
    public static class TimeUtils
    {
        private const float Epsilon = 0.00001f;
        
        public static int ToMs(float time)
        {
            return (int)(time * 1000f + Epsilon);
        }

        public static float ToSeconds(int milliseconds)
        {
            return milliseconds / 1000f;
        }
    }
}
