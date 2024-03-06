using PLANetary.Services;
using PLANetary.ViewModels;
using PLANetary.ViewModels.Connection;
using PLANetary.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace PLANetary
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            LoadAvalonEditHighlighter();
        }

        private void LoadAvalonEditHighlighter()
        {
            var sinfo = Application.GetResourceStream(new Uri("Resources/PlanetaryQL.xshd", UriKind.Relative));

            using (var stream = sinfo.Stream)
            {
                using (var reader = new System.Xml.XmlTextReader(stream))
                {
                    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance.RegisterHighlighting("PlanetaryQL", new string[0],
                        ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                            ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance));
                }
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // todo: use IoC-Container
            IDialogService dialogService = new DefaultDialogService();
            IChartDataFactory chartDataFactory = new LVCChartDataFactory();

            this.MainWindow = new MainWindow();
            Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            var mvm = new MainViewModel(dialogService, chartDataFactory);

            MainWindow.DataContext = mvm;
            MainWindow.Show();

            mvm.ConnectCommand.Execute(null);
        }
    }

}
