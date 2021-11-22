namespace OregonTrail.UserInput
{
    public class GetInputFromConsole : IGetUserInput
    {
        public string GetInput() => Console.ReadLine();
    }
}
