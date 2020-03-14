using Avalonia.Data.Converters;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MASLAB.Converters
{
    public class SeverityToBoolConverter : IValueConverter
    {
        public bool CheckWarning { get; set; }

        public bool CheckError { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (CheckWarning)
                return ((DiagnosticSeverity)value) == DiagnosticSeverity.Warning;
            else if (CheckError)
                return ((DiagnosticSeverity)value) == DiagnosticSeverity.Error;
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
