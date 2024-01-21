using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Root
{
    /// <summary>
    /// Логика взаимодействия для VoronoiDiagram_20.xaml
    /// </summary>
    public partial class VoronoiDiagram_20 : Window
    {
        public VoronoiDiagram_20()
        {
            InitializeComponent();

            MinWidth = 460;
            MinHeight = 400;

            string VoronoiDiagram_20_Path = "Resources/Диаграмма Вороного для второго нулевого бита.png";

            string absoluteVoronoiDiagram_20_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VoronoiDiagram_20_Path);

            BitmapImage VoronoiDiagram_20_Image = new BitmapImage(new Uri(absoluteVoronoiDiagram_20_Path));
            VoronoiDiagram_20_ImageOut.Source = VoronoiDiagram_20_Image;
        }
    }
}