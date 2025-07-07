using JJ.Net.Core.Extensoes;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowCheck.Domain.Extensoes
{
    public static class SolidColorBrushExtension
    {
        public static SolidColorBrush HexadecimalToSolidColorBrush(this string hexColor)
        {
            if (hexColor.ObterValorOuPadrao("").Trim() == "")
                return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));

            hexColor = hexColor.Replace("#", "");
            if (hexColor.Length == 6)
                hexColor = "FF" + hexColor;

            byte a = Convert.ToByte(hexColor.Substring(0, 2), 16);
            byte r = Convert.ToByte(hexColor.Substring(2, 2), 16);
            byte g = Convert.ToByte(hexColor.Substring(4, 2), 16);
            byte b = Convert.ToByte(hexColor.Substring(6, 2), 16);

            Windows.UI.Color color = Windows.UI.Color.FromArgb(a, r, g, b);
            return new SolidColorBrush(color);
        }

    }
}
