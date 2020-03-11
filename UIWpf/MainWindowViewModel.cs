using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace UIWpf
{
    public class MainWindowViewModel : BindableBase
    {
        private List<string> _recordsFullpaths;
        private List<string> _recordsOnlyNames;

        public List<string> RecordsFullpaths
        {
            get => _recordsFullpaths;
            set
            {
                SetProperty(ref _recordsFullpaths, value);
                var onlyNames = new List<string>();
                try
                {
                    _recordsFullpaths.ForEach(item =>
                    {
                        var fileName = new Regex(@"[/\\][^/\\]*$").Match(item)?.Value?.Trim('\\');
                        onlyNames.Add(fileName);
                    });
                    RecordsOnlyNames = onlyNames;
                }
                catch { }
            }
        }

        public List<string> RecordsOnlyNames
        {
            get => _recordsOnlyNames;
            set
            {
                SetProperty(ref _recordsOnlyNames, value);
            }
        }
    }
}
