using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Root
{
    /// <summary>
    /// Логика взаимодействия для VoronoiDiagram_30.xaml
    /// </summary>
    public partial class VoronoiDiagram_30 : Window
    {
        public VoronoiDiagram_30()
        {
            InitializeComponent();

            MinWidth = 460;
            MinHeight = 400;

            string VoronoiDiagram_30_Path = "Resources/Диаграмма Вороного для третьего нулевого бита.png";

            string absoluteVoronoiDiagram_30_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VoronoiDiagram_30_Path);

            BitmapImage VoronoiDiagram_30_Image = new BitmapImage(new Uri(absoluteVoronoiDiagram_30_Path));
            VoronoiDiagram_30_ImageOut.Source = VoronoiDiagram_30_Image;
        }
    }
}