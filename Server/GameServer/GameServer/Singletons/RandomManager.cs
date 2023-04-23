namespace GameServer.Singletons
{
    public class RandomManager
    {
        private readonly Random _random;

        public RandomManager()
        {
            this._random = new Random();
        }

        public string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new string(
                Enumerable.Repeat(chars, length)
                    .Select(s => s[this._random.Next(s.Length)])
                    .ToArray());
            return result;
        }
    }
}
