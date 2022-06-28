namespace CryptoAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Globals.Instance.StartUp().GetAwaiter().GetResult();
        }
    }
}
