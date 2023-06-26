namespace Jiro.Commands.Models
{
    public class CommandResponse
    {
        public string? CommandName { get; set; }
        public CommandType CommandType { get; set; }
        public ICommandResult? Result { get; set; }
        public bool IsSuccess { get; set; }
    }
}