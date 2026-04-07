using System.Windows;
using System.Windows.Media;

namespace PortBridgeShipping.Core.DependencyProperties
{
    public static class ImageIconProperties
    {
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.RegisterAttached(
                "IconSource",                   // Имя кастомного свойства
                typeof(ImageSource),            // Тип свойства
                typeof(ImageIconProperties),    // Класс, к которому относится свойство
                new PropertyMetadata(null));    // Значение по умолчанию - ноль

        public static ImageSource GetIconSource(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(IconSourceProperty);
        }

        public static void SetIconSource(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(IconSourceProperty, value);
        }
    }
}
