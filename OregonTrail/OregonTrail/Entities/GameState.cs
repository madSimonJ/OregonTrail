
namespace OregonTrail.Entities
{
    public record GameState
    {
        public int TurnNumber { get; set; } = 1;
        public bool IsGameFinished { get; set; }
        public IEnumerable<string> Text { get; set; } = Enumerable.Empty<string>();
    }
}
