namespace fb.game
{
    public class Score
    {
        private int currentScore = 0;

        public void Scorepassed()
        {
            currentScore++;
        }

        public void Reset()
        {
            currentScore = 0; 
        }



        public void Draw(TextureManager textureManager, int windowWidth, int windowHeight)
        {


            string scoreString = currentScore.ToString(); 
            float xPosition = windowWidth - (scoreString.Length * 30) + 480; 
            float yPosition = windowHeight + 600; 

            foreach (char digit in scoreString)
            {
                int digitValue = digit - '0'; 
                int textureId = textureManager.GetDigitTexture(digitValue); 

                textureManager.RenderTexture(textureId, xPosition, yPosition); 
                xPosition += 80; 
            }
        }

    }
}
