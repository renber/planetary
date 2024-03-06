using PLANetary.Core.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Services
{
    interface IDialogService
    {

        void ShowErrorMessage(String message);

        bool ShowConnectionDialog(out IPlanetaryConnection connection);
    }
}
