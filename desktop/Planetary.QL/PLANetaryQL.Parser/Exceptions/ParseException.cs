using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetaryQL.Parser.Exceptions
{
    public class ParseException : Exception
    {
        public List<ParseError> Errors { get; } = new List<ParseError>();

        public ParseException(ParseError singleError)
            : this (new List<ParseError>() { singleError })
        {

        }
        public ParseException(IEnumerable<ParseError> errors)
            : base("Parsing failed due to the following errors: \n" + String.Join("\n", errors.Select(x => x.Message)))
        {
            Errors.AddRange(errors);
        }
    }
}
