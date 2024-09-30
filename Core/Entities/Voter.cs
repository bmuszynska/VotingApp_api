namespace Core.Entities
{
    public class Voter : Person
    {
        public bool HasVoted { get; set; } = false;
    }
}