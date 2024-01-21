using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Root
{
    /// <summary>
    /// Логика взаимодействия для VoronoiDiagram_11.xaml
    /// </summary>
    public partial class VoronoiDiagram_11 : Window
    {
        public VoronoiDiagram_11()
        {
            InitializeComponent();

            MinWidth = 460;
            MinHeight = 400;

            string VoronoiDiagram_11_Path = "Resources/Диаграмма Вороного для первого единичного бита.png";

            string absoluteVoronoiDiagram_11_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VoronoiDiagram_11_Path);

            BitmapImage VoronoiDiagram_11_Image = new BitmapImage(new Uri(absoluteVoronoiDiagram_11_Path));
            VoronoiDiagram_11_ImageOut.Source = VoronoiDiagram_11_Image;
        }
    }
}