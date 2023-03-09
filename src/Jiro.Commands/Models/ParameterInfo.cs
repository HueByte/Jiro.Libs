namespace Jiro.Commands.Models
{
    public class ParameterInfo
    {
        public Type ParamType { get; }
        public TypeParser? Parser { get; }

        public ParameterInfo(Type type, TypeParser parser)
        {
            ParamType = type;
            Parser = parser;
        }
    }
}