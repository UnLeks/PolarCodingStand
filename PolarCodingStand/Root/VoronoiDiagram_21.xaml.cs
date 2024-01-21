using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Root
{
    /// <summary>
    /// Логика взаимодействия для VoronoiDiagram_21.xaml
    /// </summary>
    public partial class VoronoiDiagram_21 : Window
    {
        public VoronoiDiagram_21()
        {
            InitializeComponent();

            MinWidth = 460;
            MinHeight = 400;

            string VoronoiDiagram_21_Path = "Resources/Диаграмма Вороного для второго единичного бита.png";

            string absoluteVoronoiDiagram_21_Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, VoronoiDiagram_21_Path);

            BitmapImage VoronoiDiagram_21_Image = new BitmapImage(new Uri(absoluteVoronoiDiagram_21_Path));
            VoronoiDiagram_21_ImageOut.Source = VoronoiDiagram_21_Image;
        }
    }
}