using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using PLANetary.Core.Types;
using PLANetaryQL.Parser.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetaryQL.Parser
{
    public class PQLParser
    {
        public Query ParseText(string pqlStatement)
        {
            var context = Parse(pqlStatement);
            if (TryParse(pqlStatement, out ParseResult presult))
            {
                return Transform(presult.Query);
            }
            else
                throw new ParseException(presult.Errors);
        }

        public Query Transform(ParseResult presult)
        {
            return Transform(presult.Query);
        }

        /// <summary>
        /// Transforms a QueryContext parsed from PLANetaryQL to a PLANetary Query Object
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Query Transform(PlanetaryQLParser.QueryContext context)
        {
            Query q = new Query();

            // transform SENSE
            if (context.sense_stm().IsPresent())
            {
                TransformSENSE(context.sense_stm(), q);
            }

            if (context.into_store().IsPresent())
            {
                TransformINTO(context.into_store(), q);
            }

            // transform RAISE

            // transform ACT
            if (context.act_stm().IsPresent())
            {
                TransformACT(context.act_stm(), q);
            }

            // transform STORE
            if (context.store_stm().IsPresent())
            {
                TransformSTORE(context.store_stm(), q);
            }

            // transform WHERE
            if (context.where_stm().IsPresent())
            {
                TransformWHERE(context.where_stm(), q);
            }

            if (context.at_stm().IsPresent())
            {
                TransformAT(context.at_stm(), q);
            } else
            {
                q.VirtualTableName = "sensors";
            }

            // transform EVERY
            if (context.every_stm().IsPresent())
            {
                TransformEVERY(context.every_stm(), q);
            }

            return q;
        }

        private void TransformINTO(PlanetaryQLParser.Into_storeContext context, Query targetQuery)
        {
            targetQuery.StoreTarget = (new StoreSelection(
                        new Store(
                            context.storename().IDENTIFIER().GetText(), ""),
                        SelectionFunction.Single
                        )
                    );
        }

        private void TransformSTORE(PlanetaryQLParser.Store_stmContext context, Query targetQuery)
        {
            if (context.drop().IsPresent())
            {
                targetQuery.Initiations.Add(new StoreInitiation(
                    new Store(context.drop().store().IDENTIFIER().GetText(), ""),
                    "Drop",
                    0));
            }
            else
            {
                // create store
                string storeFunction = context.create().storefunc().func().IDENTIFIER().GetText();
                targetQuery.Initiations.Add(new StoreInitiation(
                    new Store(context.create().store().IDENTIFIER().GetText(), ""),
                    storeFunction,
                    Double.Parse(context.create().storefunc().funcparam().GetText())
                    ));
            }
        }

        private void TransformSENSE(PlanetaryQLParser.Sense_stmContext context, Query targetQuery)
        {
            foreach (var aggr in context.sensorlist().aggregation())
            {
                if (aggr.sensor().IsPresent())
                {
                    // no aggregation function
                    targetQuery.Selections.Add(new ValueSelection(new Sensor(aggr.sensor().IDENTIFIER().GetText(), ""), SelectionFunction.Single));
                }
                else if (aggr.sensorfunc().IsPresent()) 
                {
                    // sensor + aggregation function
                    SelectionFunction selfunc = SelectionFunctionExtensions.FromSqlFuncName(aggr.sensorfunc().func().IDENTIFIER().GetText());
                    if (selfunc == (SelectionFunction)(-1))
                    {
                        // TODO: change to custom exception
                        throw new Exception("Invalid aggregation function");
                    }
                    targetQuery.Selections.Add(new ValueSelection(new Sensor(aggr.sensorfunc().sensor().IDENTIFIER().GetText(), ""), selfunc));
                }
                else
                {
                    //store
                    targetQuery.StoreSources.Add(new StoreSelection(
                            new Store(aggr.storename().IDENTIFIER().GetText(), ""),
                            SelectionFunction.Single,
                            Double.Parse(aggr.funcparam().GetText()))
                        );
                }
            }
        }

        private void TransformACT(PlanetaryQLParser.Act_stmContext context, Query targetQuery)
        {
            foreach (var actor in context.actorlist().actorfunc())
            {
                targetQuery.Actuators.Add(new ActuatorFunc(new Actuator(actor.func().IDENTIFIER().GetText(), ""), actor.funcparam().Select(x => int.Parse(x.GetText())).ToArray()));
            }
        }

        private void TransformAT(PlanetaryQLParser.At_stmContext context, Query targetQuery)
        {
            targetQuery.VirtualTableName = context.table().IDENTIFIER().GetText();
        }

        private void TransformWHERE(PlanetaryQLParser.Where_stmContext context, Query targetQuery)
        {
            if (context.conditionlist().condition_link().Select(x => x.GetText()).Distinct().Count() > 1)
            {
                throw new SemanticException("Cannot mix condition links in one group");
            }

            targetQuery.Conditions.ConditionLink = context.conditionlist().condition_link().FirstOrDefault()?.GetText() == "OR" ? BooleanLink.OR : BooleanLink.AND;

            List<IQueryCondition> conditions = new List<IQueryCondition>();

            foreach (var condition in context.conditionlist().conditiongroup())
            {
                conditions.Add(TransformConditionGroup(condition));
            }

            targetQuery.Conditions.Conditions.AddRange(conditions);
            FlattenConditionGroup(targetQuery.Conditions);
        }

        private void TransformEVERY(PlanetaryQLParser.Every_stmContext context, Query targetQuery)
        {
            TimeSpan period = ConvertTimeContextToTimespan(context.time());

            targetQuery.PeriodInMS = (int)period.TotalMilliseconds;
            targetQuery.Periodic = targetQuery.PeriodInMS > 0;            
        }

        private TimeSpan ConvertTimeContextToTimespan(PlanetaryQLParser.TimeContext context)
        {
            string number = context.NUMBER().GetText();
            string unit = context.timeunit().GetText();

            if (float.TryParse(number, out float f))
            {
                switch (unit.ToLower())
                {
                    case "d":
                    case "day":
                    case "days":
                        return TimeSpan.FromDays(f);

                    case "h":
                    case "hour":
                    case "hours":
                        return TimeSpan.FromMinutes(f);

                    case "m":
                    case "min":
                    case "minute":
                    case "minutes":
                        return TimeSpan.FromMinutes(f);

                    case "ms":
                    case "msec":
                    case "millis":
                    case "millisecond":
                    case "milliseconds":
                        return TimeSpan.FromMilliseconds(f);

                    case "s":
                    case "sec":
                    case "second":
                    case "seconds":
                    default:
                        return TimeSpan.FromSeconds(f);
                }
            }
            else
                throw new SemanticException($"'{number} {unit}' is not a valid timespan specifier.");
        }

        /// <summary>
        /// Remove nesting of groups, when group only contains a single other group
        /// </summary>        
        private void FlattenConditionGroup(ConditionGroup group)
        {
            foreach (var subgroup in group.Conditions.OfType<ConditionGroup>())
                FlattenConditionGroup(subgroup);

            // unpack groups which only contain a single group
            if (group.Count == 1 && group.Conditions[0] is ConditionGroup)
            {
                ConditionGroup extractedGroup = (ConditionGroup)group.Conditions[0];
                group.Conditions.Clear();
                group.ConditionLink = extractedGroup.ConditionLink;
                group.Conditions.AddRange(extractedGroup.Conditions);
            }
        }

        private ConditionGroup TransformConditionGroup(PlanetaryQLParser.ConditiongroupContext context)
        {
            if (context.condition_link().Select(x => x.GetText()).Distinct().Count() > 1)
            {
                throw new SemanticException("Cannot mix condition links in one group");
            }

            ConditionGroup group = new ConditionGroup();
            group.ConditionLink = context.condition_link().FirstOrDefault()?.GetText() == "OR" ? BooleanLink.OR : BooleanLink.AND;

            if (context.condition().Count() > 0)
            {
                // non-nested
                foreach (var condition in context.condition())
                {
                    group.Conditions.Add(TransformCondition(condition));
                }
            }
            else
            {
                // nested
                foreach (var subgroup in context.conditiongroup())
                {
                    group.Conditions.Add(TransformConditionGroup(subgroup));
                }
            }

            return group;
        }

        private IQueryCondition TransformCondition(PlanetaryQLParser.ConditionContext context)
        {
            if (context.sensor().IsPresent())
            {
                return new SensorCondition(new Sensor(context.sensor().IDENTIFIER().GetText(), ""), ConditionOperatorExtensions.FromMathSymbol(context.OPERATOR().GetText()), int.Parse(context.NUMBER().GetText()));
            }

            if (context.eventname().IsPresent())
            {
                if (context.OPERATOR().IsPresent())
                {
                    return new EventCondition(new Event(context.eventname().IDENTIFIER().GetText()), ConditionOperatorExtensions.FromMathSymbol(context.OPERATOR().GetText()), int.Parse(context.NUMBER().GetText()));
                }
                else
                {
                    // only event name given -> evt > 0
                    return new EventCondition(new Event(context.eventname().IDENTIFIER().GetText()), ConditionOperator.OP_GREATER, 0);
                }
            }

            // should have been caught by the parser already
            throw new ParseException(new ParseError(0, 0, "Unknown condition '" + context.GetText() + "'"));
        }

        /// <summary>
        /// Parses a PlanetaryQL statement and returns the resulting PlanetaryQLParser.QueryContext 
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="ParseException" />
        public PlanetaryQLParser.QueryContext Parse(string text)
        {
            ParseResult result;
            if (TryParse(text, out result))
            {
                return result.Query;
            }
            else
            {
                throw new ParseException(result.Errors);
            }
        }

        /// <summary>
        /// Tries to parse a PlanetaryQL statement and returns a ParseResult which contains
        /// the resulting PlanetaryQLParser.QueryContext and any parsing errors which occured
        /// </summary>
        /// <returns>True if the parsing was successful, false if result contains errors</returns>
        public bool TryParse(string text, out ParseResult result)
        {
            LexerErrorListener errorListener = new LexerErrorListener();

            AntlrInputStream stream = new AntlrInputStream(text);
            PlanetaryQLLexer lexer = new PlanetaryQLLexer(stream);
            lexer.AddErrorListener(errorListener);

            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            PlanetaryQLParser parser = new PlanetaryQLParser(tokenStream);
            parser.AddErrorListener(errorListener);

            result = new ParseResult(parser.query(), errorListener.Errors);
            return !result.HasErrors;
        }

        class LexerErrorListener : IAntlrErrorListener<int>, IAntlrErrorListener<IToken>
        {
            public List<ParseError> Errors { get; } = new List<ParseError>();

            public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] int offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
            {
                Errors.Add(new ParseError(line, charPositionInLine, msg));
            }

            public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
            {
                Errors.Add(new ParseError(line, charPositionInLine, msg));
            }
        }
    }

    public class ParseResult
    {
        public PlanetaryQLParser.QueryContext Query { get; }

        public List<ParseError> Errors { get; } = new List<ParseError>();

        public bool HasErrors => Errors.Count > 0;

        public ParseResult(PlanetaryQLParser.QueryContext query, IEnumerable<ParseError> errors)
        {
            Query = query;
            Errors.AddRange(errors);
        }
    }

    public class ParseError
    {
        public int Line { get; }
        public int CharPosition { get; }

        public string Message { get; }

        public ParseError(int line, int charPosition, string message)
        {
            Line = line;
            CharPosition = charPosition;
            Message = message;
        }
    }


}
