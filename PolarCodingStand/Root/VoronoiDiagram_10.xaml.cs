using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Root
{
    /// <summary>
    /// Логика взаимодействия для VoronoiDiagram_10.xaml
    /// </summary>
    public partial class VoronoiDiagram_10 : Window
    {
        public VoronoiDiagram_10()
        {
            InitializeComponent();

            MinWidth = 460;
            MinHeight = 400;

            string VoronoiDiagram_10_Path = "Resources/Диаграмма Вороного для первого нулевого бита.png";

            string absoluteVoronoiDiagram_10_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VoronoiDiagram_10_Path);

            BitmapImage VoronoiDiagram_10_Image = new BitmapImage(new Uri(absoluteVoronoiDiagram_10_Path));
            VoronoiDiagram_10_ImageOut.Source = VoronoiDiagram_10_Image;
        }
    }
}