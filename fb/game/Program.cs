namespace fb
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var game = new Game())
            {

                game.Title = "Flappy Bird";
                game.Run(60.0);
            }
        }
    }
}
