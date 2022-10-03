namespace OregonTrail2.PlayerInteraction
{
    public interface IConsole
    {
        Operation WriteLine(params string[] message);
        Maybe<string> ReadLine();
    }
}
