namespace ErmineGames.Utils
{
    public class SequentialIDGenerator
    {
        private int nextId = 1;

        public SequentialIDGenerator()
        {
        }
        
        public SequentialIDGenerator(int startingId)
        {
            nextId = startingId;
        }

        public int Generate()
        {
            return nextId++;
        }
    }
}
