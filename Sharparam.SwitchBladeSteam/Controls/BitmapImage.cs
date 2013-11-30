using System.Drawing;
using System.Windows;
using Sharparam.SwitchBladeSteam.Lib;
using Image = System.Windows.Controls.Image;

namespace Sharparam.SwitchBladeSteam.Controls
{
    public class BitmapImage : Image
    {
        public static readonly DependencyProperty BitmapProperty =
            DependencyProperty.Register("Bitmap", typeof (Bitmap), typeof (BitmapImage), new PropertyMetadata(default(Bitmap)));

        public Bitmap Bitmap
        {
            get { return (Bitmap) GetValue(BitmapProperty); }
            set
            {
                SetValue(BitmapProperty, value);
                //SetValue(SourceProperty, Helper.BitmapToSource(value));
                Source = Helper.BitmapToSource(value);
            }
        }
    }
}
