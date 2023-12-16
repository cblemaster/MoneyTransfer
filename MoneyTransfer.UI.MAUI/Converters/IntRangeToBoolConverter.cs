using System.Globalization;

namespace MoneyTransfer.UI.MAUI.Converters
{
    internal class IntRangeToBoolConverter : IValueConverter
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            value is int eval && eval >= MinValue && eval <= MaxValue;

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
