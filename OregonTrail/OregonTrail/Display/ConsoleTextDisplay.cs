namespace OregonTrail.Display
{
    public class ConsoleTextDisplay : IDisplayText
    {
        public void DisplayToUser(IEnumerable<string> text) =>
            text.ToList().ForEach(Console.WriteLine);
    }
}
