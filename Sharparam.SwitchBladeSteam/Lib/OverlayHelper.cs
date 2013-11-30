using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace Sharparam.SwitchBladeSteam.Lib
{
    public class OverlayHelper
    {
        private const string LabelFormat = "New message from {0}";

        private static readonly Brush OverlayNormalColor = new SolidColorBrush(Color.FromRgb(116, 116, 116));
        private static readonly Brush OverlayFlashColor = new SolidColorBrush(Color.FromRgb(185, 185, 185));
        private static readonly Brush OverlayNormalTextColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        private static readonly Brush OverlayFlashTextColor = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        private static readonly TimeSpan FlashDelay = TimeSpan.FromMilliseconds(800);

        private readonly Grid _overlay;
        private readonly Label _label;
        private readonly DispatcherTimer _flashTimer;
        private readonly DispatcherTimer _hideTimer;

        private bool _flashed;

        public OverlayHelper(Grid overlay, Label label)
        {
            _overlay = overlay;
            _label = label;

            _flashTimer = new DispatcherTimer(FlashDelay, DispatcherPriority.Render, OnFlashTimerTick, _overlay.Dispatcher);
            if (_flashTimer.IsEnabled)
                _flashTimer.Stop();

            _hideTimer = new DispatcherTimer(DispatcherPriority.Background, _overlay.Dispatcher);
            _hideTimer.Tick += HideTimerOnTick;
        }

        public void Show(string name)
        {
            if (!_overlay.Dispatcher.CheckAccess())
            {
                _overlay.Dispatcher.Invoke((Action) (() => Show(name)));
                return;
            }

            if (_hideTimer.IsEnabled)
                _hideTimer.Stop();

            ResetOverlayColors();

            _label.Content = String.Format(LabelFormat, name);
            _overlay.Visibility = Visibility.Visible;

            _flashTimer.Start();
        }

        public void Show(string name, int milliseconds)
        {
            Show(name);
            HideAfter(milliseconds);
        }

        public void Hide()
        {
            if (!_overlay.Dispatcher.CheckAccess())
            {
                _overlay.Dispatcher.Invoke((Action) Hide);
                return;
            }

            _flashTimer.Stop();

            _overlay.Visibility = Visibility.Collapsed;
        }

        public void HideAfter(int milliseconds)
        {
            if (_hideTimer.IsEnabled)
                _hideTimer.Stop();

            _hideTimer.Interval = TimeSpan.FromMilliseconds(milliseconds);
            _hideTimer.Start();
        }

        public bool IsPointInsideOverlay(Point pos, Visual root)
        {
            var bounds = GetOverlayBounds(root);

            return pos.X >= bounds.X && pos.X <= bounds.X + bounds.Width &&
                   pos.Y >= bounds.Y && pos.Y <= bounds.Y + bounds.Height;
        }

        private Rectangle GetOverlayBounds(Visual root)
        {
            if (!_overlay.Dispatcher.CheckAccess())
                return
                    (Rectangle)
                    _overlay.Dispatcher.Invoke((Func<Visual, Rectangle>) GetOverlayBounds, DispatcherPriority.Send, root);

            Point pos = _overlay.TransformToAncestor(root).Transform(new Point(0, 0));
            var width = (int) _overlay.ActualWidth;
            var height = (int) _overlay.ActualHeight;

            return new Rectangle((int) pos.X, (int) pos.Y, width, height);
        }

        private void ResetOverlayColors()
        {
            if (!_overlay.Dispatcher.CheckAccess())
            {
                _overlay.Dispatcher.Invoke((Action)ResetOverlayColors);
                return;
            }

            _overlay.Background = OverlayNormalColor;
            _label.Foreground = OverlayNormalTextColor;

            _flashed = false;
        }

        private void InvertOverlayColors()
        {
            if (!_overlay.Dispatcher.CheckAccess())
            {
                _overlay.Dispatcher.Invoke((Action) InvertOverlayColors);
                return;
            }

            if (_flashed)
            {
                _overlay.Background = OverlayNormalColor;
                _label.Foreground = OverlayNormalTextColor;
            }
            else
            {
                _overlay.Background = OverlayFlashColor;
                _label.Foreground = OverlayFlashTextColor;
            }

            _flashed = !_flashed;
        }

        private void OnFlashTimerTick(object sender, EventArgs e)
        {
            InvertOverlayColors();
        }

        private void HideTimerOnTick(object sender, EventArgs eventArgs)
        {
            _hideTimer.Stop();
            Hide();
        }
    }
}
