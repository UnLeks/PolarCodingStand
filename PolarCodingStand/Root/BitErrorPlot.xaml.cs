using OxyPlot;
using System.Windows;

namespace Root
{
    /// <summary>
    /// Логика взаимодействия для BitErrorPlot.xaml
    /// </summary>
    public partial class BitErrorPlot : Window
    {
        public BitErrorPlot()
        {
            InitializeComponent();

            // Установка минимальных размеров окна
            MinWidth = 350;
            MinHeight = 350;
        }

        public void SetPlotModel(PlotModel plotModel)
        {
            bitErrorPlot.Model = plotModel;
        }
    }
}