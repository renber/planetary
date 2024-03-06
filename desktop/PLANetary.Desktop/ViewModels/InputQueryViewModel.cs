using PLANetary.Core.Types;
using PLANetary.Interaction;
using PLANetary.Types;
using PLANetaryQL.Parser;
using PLANetaryQL.Parser.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PLANetary.ViewModels
{
    class InputQueryViewModel : ViewModelBase
    {
        string queryText = "";
        public string QueryText { get => queryText; set => ChangeProperty(ref queryText, value); }

        string parserErrorText = "";
        public string ParserErrorText { get => parserErrorText; set => ChangeProperty(ref parserErrorText, value); }

        public ICommand ExecuteQueryCommand { get; }  
        
        public ICommand ClearQueryTextCommand { get; }

        public event EventHandler<QueryCreatedEventArgs> QueryCreated;

        public InputQueryViewModel()
        {
            ExecuteQueryCommand = new RelayCommand(ExecuteQueryCommand_Execute, () => !String.IsNullOrEmpty(QueryText));

            ClearQueryTextCommand = new RelayCommand(() => { QueryText = ""; ParserErrorText = ""; }, () => !String.IsNullOrEmpty(QueryText));
        }

        protected void ExecuteQueryCommand_Execute()
        {
            PQLParser parser = new PQLParser();

            // parse the query            
            try
            {
                Query query = parser.ParseText(QueryText);                
                ParserErrorText = "";

                if (OnQueryCreated(query))
                {
                    ClearQueryTextCommand.Execute(null);
                }
            }
            catch (ParseException exc)
            {
                ParserErrorText = String.Join("\n", exc.Errors.Select(x => "Line " + x.Line + "@" + x.CharPosition + ": " + x.Message));
            }
            catch (Exception exc)
            {
                ParserErrorText = exc.Message;
            }
        }

        protected bool OnQueryCreated(Query query)
        {
            QueryCreatedEventArgs args = new QueryCreatedEventArgs(query);
            QueryCreated?.Invoke(this, args);
            return args.Handled;
        }
    }
}
