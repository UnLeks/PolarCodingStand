using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Root
{
    /// <summary>
    /// Логика взаимодействия для VoronoiDiagram_31.xaml
    /// </summary>
    public partial class VoronoiDiagram_31 : Window
    {
        public VoronoiDiagram_31()
        {
            InitializeComponent();

            MinWidth = 460;
            MinHeight = 400;

            string VoronoiDiagram_31_Path = "Resources/Диаграмма Вороного для третьего единичного бита.png";

            string absoluteVoronoiDiagram_31_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VoronoiDiagram_31_Path);

            BitmapImage VoronoiDiagram_31_Image = new BitmapImage(new Uri(absoluteVoronoiDiagram_31_Path));
            VoronoiDiagram_31_ImageOut.Source = VoronoiDiagram_31_Image;
        }
    }
}