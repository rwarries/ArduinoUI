using System;
using System.Globalization;
using System.Windows.Controls;

namespace WpfApplication1
{
    // @see http://tech.pro/tutorial/948/wpf-tutorial-binding-validation-rules
  public class IPValidationRule : ValidationRule
  {
    public override ValidationResult Validate(object value, 
      CultureInfo cultureInfo)
    {
      var str = value as string;
      if (String.IsNullOrEmpty(str))
      {
        return new ValidationResult(false,
          "Enter IP Address.");
      }

      var parts = str.Split('.');
      if (parts.Length != 4)
      {
        return new ValidationResult(false,
          "IP Address should be four octets, seperated by decimals.");
      }

      foreach (var p in parts)
      {
        int intPart;
        if (!int.TryParse(p, NumberStyles.Integer,
          cultureInfo.NumberFormat, out intPart))
        {
          return new ValidationResult(false,
            "Each octet of an IP Address should be a number.");
        }

        if (intPart < 0 || intPart > 255)
        {
          return new ValidationResult(false,
            "Each octet of an IP Address should be between 0 and 255.");
        }
      }

      return new ValidationResult(true, null);
    }
  }
}
