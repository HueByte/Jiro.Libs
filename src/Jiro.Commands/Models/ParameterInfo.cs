namespace Jiro.Commands.Models
{
    /// <summary>
    /// Represents information about a command parameter, including its type and parser.
    /// </summary>
    public class ParameterInfo
    {
        /// <summary>
        /// Gets the type of the parameter.
        /// </summary>
        public Type ParamType { get; }

        /// <summary>
        /// Gets the parser used to parse the parameter value.
        /// </summary>
        public TypeParser? Parser { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterInfo"/> class.
        /// </summary>
        /// <param name="type">The type of the parameter.</param>
        /// <param name="parser">The parser for the parameter.</param>
        public ParameterInfo(Type type, TypeParser parser)
        {
            ParamType = type;
            Parser = parser;
        }
    }
}