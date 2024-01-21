using BhattacharyaParametersDLL;
using ConstellationDLL;
using DemodulatorDLL;
using DirectDecoderDLL;
using GmatrixDLL;
using ModulatorDLL;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using PolarEncoderDLL;
using RecursiveDecoderDLL;
using StraightLinesDLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Root
{
    /// <summary>
    /// Логика взаимодействия для PolarCodingStandWindow.xaml
    /// </summary>
    public partial class PolarCodingStandWindow : Window
    {
        private BackgroundWorker backgroundWorker;

        private List<Window> childWindows = new List<Window>();

        private int numberOfRuns; // количество запусков модели
        private int numberOfPackets; // количество тестов в одной точке
        private int messageLength; // длина кодового сообщения
        private double[] noise; // инициализация вектора шума в канале
        private double noiseStep; // шаг между значениями вектора шума
        double[] demodulationError; // вектор ошибок после демодуляции
        double[] decodingError; // вектор ошибок после декодирования

        private double tempFirstValueOfNoise; // сохранение значения левой границы в случае сброса numberOfRuns

        private int selectedDecodingMethod; // выбранный метод декодирования

        // Флаги, обозначающие нажатие кнопок сброса параметров моделирования
        private bool ResetButton_numberOfRuns_click_called = false;
        private bool ResetButton_numberOfPackets_click_called = false;
        private bool ResetButton_messageLength_click_called = false;
        private bool ResetButton_firstValueOfNoise_click_called = false;
        private bool ResetButton_noiseStep_click_called = false;

        private bool resetModelingSignal = false;

        private bool manualModeIsEnabled = false;

        private bool showStepsInAutomaticMode_if_1 = true;
        private bool showStepsInAutomaticMode_if_1_2 = true;
        private bool showStepsInAutomaticMode_if_2 = true;
        private bool showStepsInAutomaticMode_if_3 = true;
        private bool showStepsInAutomaticMode_if_4 = true;
        private bool showStepsInAutomaticMode_if_5 = true;
        private bool showStepsInAutomaticMode_if_6 = true;
        private bool showStepsInAutomaticMode_if_7 = true;
        private bool showStepsInAutomaticMode_if_8 = true;
        private bool showStepsInAutomaticMode_if_9 = true;
        private bool showStepsInAutomaticMode_if_10 = true;

        public PolarCodingStandWindow()
        {
            InitializeComponent();

            Closing += MainWindowClosing;

            // Скрытие элементов, не относящихся к экрану #1
            settingModelingParameters_Text.Visibility = Visibility.Collapsed;
            codingMethod_StackPanel.Visibility = Visibility.Collapsed;
            modulationMethod_StackPanel.Visibility = Visibility.Collapsed;
            noiseExposureMethod_StackPanel.Visibility = Visibility.Collapsed;
            demodulationMethod_StackPanel.Visibility = Visibility.Collapsed;
            decodingMethod_StackPanel.Visibility = Visibility.Collapsed;
            numberOfRuns_StackPanel.Visibility = Visibility.Collapsed;
            numberOfPackets_StackPanel.Visibility = Visibility.Collapsed;
            messageLength_StackPanel.Visibility = Visibility.Collapsed;
            firstValueOfNoise_StackPanel.Visibility = Visibility.Collapsed;
            noiseStep_StackPanel.Visibility = Visibility.Collapsed;
            previousStepButton_screen2.Visibility = Visibility.Collapsed;
            nextStepButton_screen2.Visibility = Visibility.Collapsed;

            leftSide_StackPanel.Visibility = Visibility.Collapsed;
            previousStepButton_screen3.Visibility = Visibility.Collapsed;

            step0_StackPanel.Visibility = Visibility.Collapsed;
            nextStepButton_step0.Visibility = Visibility.Collapsed;
            manualInputMode_StackPanel.Visibility = Visibility.Collapsed;

            step1_StackPanel.Visibility = Visibility.Collapsed;
            nextStepButton_step1.Visibility = Visibility.Collapsed;
            previousStepButton_step1.Visibility = Visibility.Collapsed;

            step05_StackPanel.Visibility = Visibility.Collapsed;
            nextStepButton_step05.Visibility = Visibility.Collapsed;
            previousStepButton_step05.Visibility = Visibility.Collapsed;

            step2_StackPanel.Visibility = Visibility.Collapsed;
            nextStepButton_step2.Visibility = Visibility.Collapsed;
            previousStepButton_step2.Visibility = Visibility.Collapsed;

            step3_StackPanel.Visibility = Visibility.Collapsed;
            nextStepButton_step3.Visibility = Visibility.Collapsed;
            previousStepButton_step3.Visibility = Visibility.Collapsed;

            step4_StackPanel.Visibility = Visibility.Collapsed;
            nextStepButton_step4.Visibility = Visibility.Collapsed;
            previousStepButton_step4.Visibility = Visibility.Collapsed;

            step5_StackPanel.Visibility = Visibility.Collapsed;
            nextStepButton_step5.Visibility = Visibility.Collapsed;
            previousStepButton_step5.Visibility = Visibility.Collapsed;

            step6_StackPanel.Visibility = Visibility.Collapsed;
            nextStepButton_step6.Visibility = Visibility.Collapsed;
            previousStepButton_step6.Visibility = Visibility.Collapsed;

            step7_StackPanel.Visibility = Visibility.Collapsed;
            previousStepButton_step7.Visibility = Visibility.Collapsed;

            manualInputMode.Visibility = Visibility.Collapsed;
            automaticMode.Visibility = Visibility.Collapsed;

            help_step0.Visibility = Visibility.Collapsed;
            help_step05.Visibility = Visibility.Collapsed;
            help_step1.Visibility = Visibility.Collapsed;
            help_step2.Visibility = Visibility.Collapsed;
            help_step3.Visibility = Visibility.Collapsed;
            help_step4.Visibility = Visibility.Collapsed;
            help_step5.Visibility = Visibility.Collapsed;
            help_step6.Visibility = Visibility.Collapsed;
            help_step7.Visibility = Visibility.Collapsed;

            // Отображение элементов экрана #1: стартового сообщения, информации об авторе и кнопки старта
            startMessage.Text = "Данная программа предназначена для моделирования работы канала связи с кодированием сообщения методом полярного кодирования, восьмипозиционной фазовой модуляцией (8-PSK), воздействием аддитивного белого гауссовсого шума (АБГШ) на передаваемое сообщение, демодуляцией методом построения обалстей Вороного и декодированием прямым и рекурсивным методами.";
            author.Text = "Автор: Пономарева К. Е. (ТБ18-01)";

            startMessage.Visibility = Visibility.Visible;
            author.Visibility = Visibility.Visible;
            startButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Кнопка перехода с экрана #1 (стартовое окно) на экран #2 (задание параметров моделирования)
        /// </summary>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Скрытие элементов, не относящихся к экрану #2
            startMessage.Visibility = Visibility.Collapsed;
            author.Visibility = Visibility.Collapsed;
            startButton.Visibility = Visibility.Collapsed;

            // Отображение элементов экрана #2:
            settingModelingParameters_Text.Text = "Задание параметров моделирования";

            codingMethod_Text.Content = "Метод кодирования:";
            polarCoding_ComboBox_Text.Content = "Полярное кодирование";

            modulationMethod_Text.Content = "Метод модуляции:";
            eightPSK_ComboBox_Text.Content = "Восьмипозиционная фазовая модуляция";

            noiseExposureMethod_Text.Content = "Метод воздействия шума:";
            AWGN_ComboBox_Text.Content = "Аддитивный белый гауссовский шум";

            demodulationMethod_Text.Content = "Метод демодуляции:";
            VoronoiDiagram_ComboBox_Text.Content = "Построение областей Вороного";

            decodingMethod_Text.Content = "Метод декодирования:";
            directEncoding_ComboBox_Text.Content = "Прямой метод";
            recursiveEncoding_ComboBox_Text.Content = "Рекурсивный метод";

            numberOfRuns_Text.Content = "Количество запусков модели:";
            numberOfPackets_Text.Content = "Количество тестов в одной точке:";
            messageLength_Text.Content = "Длина кода:";
            firstValueOfNoise_Text.Content = "Левая граница (первое значение вектора шума):";
            noiseStep_Text.Content = "Шаг между значениями вектора шума:";

            settingModelingParameters_Text.Visibility = Visibility.Visible;
            codingMethod_StackPanel.Visibility = Visibility.Visible;
            modulationMethod_StackPanel.Visibility = Visibility.Visible;
            noiseExposureMethod_StackPanel.Visibility = Visibility.Visible;
            demodulationMethod_StackPanel.Visibility = Visibility.Visible;
            decodingMethod_StackPanel.Visibility = Visibility.Visible;
            numberOfRuns_StackPanel.Visibility = Visibility.Visible;
            numberOfPackets_StackPanel.Visibility = Visibility.Visible;
            messageLength_StackPanel.Visibility = Visibility.Visible;
            firstValueOfNoise_StackPanel.Visibility = Visibility.Visible;
            noiseStep_StackPanel.Visibility = Visibility.Visible;
            previousStepButton_screen2.Visibility = Visibility.Visible;
            nextStepButton_screen2.Visibility = Visibility.Visible;

            numberOfRuns_resetButton.IsEnabled = !numberOfRuns_confirmButton.IsEnabled;
            numberOfPackets_resetButton.IsEnabled = !numberOfPackets_confirmButton.IsEnabled;
            messageLength_resetButton.IsEnabled = !messageLength_confirmButton.IsEnabled;
            firstValueOfNoise_resetButton.IsEnabled = !firstValueOfNoise_confirmButton.IsEnabled;
            noiseStep_resetButton.IsEnabled = !noiseStep_confirmButton.IsEnabled;

            // Проверка наличия несброшенных значений в заданных параметрах моделирования
            CheckModelingParameters();
        }

        private void ConfirmButton_numberOfRuns_click(object sender, RoutedEventArgs e)
        {
            // Попытка преобразовать текст из numberOfRuns_TextBox в число
            if (int.TryParse(numberOfRuns_TextBox.Text, out int enteredNumber))
            {
                // Если успешно преобразовано, проверяем валидность введенного числа
                if (enteredNumber <= 0)
                {
                    numberOfRuns_validationMessage.Text = "Введите число больше 0.";
                    numberOfRuns_resetedMessage.Text = "";
                    numberOfRuns_confirmedMessage.Text = "";

                    numberOfRuns_TextBox.Text = "";
                }
                else if (enteredNumber <= 100)
                {
                    // Присваиваем введенное число переменной numberOfRuns
                    numberOfRuns = enteredNumber;
                    numberOfRuns_validationMessage.Text = "";
                    numberOfRuns_resetedMessage.Text = "";

                    numberOfRuns_confirmedMessage.Text = $"Выбранное значение: {numberOfRuns}";

                    // Очистка numberOfRuns_TextBox
                    numberOfRuns_TextBox.Text = "";
                    numberOfRuns_TextBox.IsEnabled = false;
                    numberOfRuns_confirmButton.IsEnabled = false;

                    numberOfRuns_resetButton.IsEnabled = true;

                    noise = new double[numberOfRuns];
                }
                else
                {
                    // Выводим сообщение об ошибке, если введенное число больше 100
                    numberOfRuns_validationMessage.Text = "Слишком много запусков. Введите число менее 100.";
                    numberOfRuns_resetedMessage.Text = "";
                    numberOfRuns_confirmedMessage.Text = "";

                    numberOfRuns_TextBox.Text = "";
                }
            }
            else
            {
                numberOfRuns_validationMessage.Text = "Введите корректное число.";
                numberOfRuns_resetedMessage.Text = "";
                numberOfRuns_confirmedMessage.Text = "";

                numberOfRuns_TextBox.Text = "";
            }

            // Проверка наличия несброшенных значений в заданных параметрах моделирования
            CheckModelingParameters();
        }

        private void ResetButton_numberOfRuns_click(object sender, RoutedEventArgs e)
        {
            numberOfRuns = 0;
            numberOfRuns_TextBox.IsEnabled = true;
            numberOfRuns_confirmButton.IsEnabled = true;

            numberOfRuns_confirmedMessage.Text = "";
            numberOfRuns_resetedMessage.Text = "Значение сброшено.";
            numberOfRuns_validationMessage.Text = "";

            numberOfRuns_resetButton.IsEnabled = false;

            // Проверка наличия несброшенных значений в заданных параметрах моделирования
            CheckModelingParameters();

            ResetButton_numberOfRuns_click_called = true;
        }

        private void ConfirmButton_numberOfPackets_click(object sender, RoutedEventArgs e)
        {
            // Попытка преобразовать текст из numberOfPackets_TextBox в число
            if (int.TryParse(numberOfPackets_TextBox.Text, out int enteredNumber))
            {
                // Если успешно преобразовано, проверяем валидность введенного числа
                if (enteredNumber <= 0)
                {
                    numberOfPackets_validationMessage.Text = "Введите число больше 0.";
                    numberOfPackets_resetedMessage.Text = "";
                    numberOfPackets_confirmedMessage.Text = "";

                    numberOfPackets_TextBox.Text = "";
                }
                else if (enteredNumber <= 100000)
                {
                    // Присваиваем введенное число переменной numberOfPackets
                    numberOfPackets = enteredNumber;
                    numberOfPackets_validationMessage.Text = "";
                    numberOfPackets_resetedMessage.Text = "";
                    numberOfPackets_confirmedMessage.Text = $"Выбранное значение: {numberOfPackets}";

                    // Очистка numberOfPackets_TextBox
                    numberOfPackets_TextBox.Text = "";
                    numberOfPackets_TextBox.IsEnabled = false;
                    numberOfPackets_confirmButton.IsEnabled = false;

                    numberOfPackets_resetButton.IsEnabled = true;
                }
                else
                {
                    // Выводим сообщение об ошибке, если введенное число больше 100000
                    numberOfPackets_validationMessage.Text = "Слишком много тестов. Введите число менее 100000.";
                    numberOfPackets_resetedMessage.Text = "";
                    numberOfPackets_confirmedMessage.Text = "";

                    numberOfPackets_TextBox.Text = "";
                }
            }
            else
            {
                numberOfPackets_validationMessage.Text = "Введите корректное число.";
                numberOfPackets_resetedMessage.Text = "";
                numberOfPackets_confirmedMessage.Text = "";

                numberOfPackets_TextBox.Text = "";
            }

            // Проверка наличия несброшенных значений в заданных параметрах моделирования
            CheckModelingParameters();
        }

        private void ResetButton_numberOfPackets_click(object sender, RoutedEventArgs e)
        {
            numberOfPackets = 0;
            numberOfPackets_TextBox.IsEnabled = true;
            numberOfPackets_confirmButton.IsEnabled = true;

            numberOfPackets_confirmedMessage.Text = "";
            numberOfPackets_resetedMessage.Text = "Значение сброшено.";
            numberOfPackets_validationMessage.Text = "";

            numberOfPackets_resetButton.IsEnabled = false;

            // Проверка наличия несброшенных значений в заданных параметрах моделирования
            CheckModelingParameters();

            ResetButton_numberOfPackets_click_called = true;
        }

        private void ConfirmButton_messageLength_click(object sender, RoutedEventArgs e)
        {
            // Попытка преобразовать текст из messageLength_TextBox в число
            if (int.TryParse(messageLength_TextBox.Text, out int enteredNumber))
            {
                // Если успешно преобразовано, проверяем валидность введенного числа
                if (enteredNumber <= 0)
                {
                    messageLength_validationMessage.Text = "Введите число больше 0.";
                    messageLength_resetedMessage.Text = "";
                    messageLength_confirmedMessage.Text = "";

                    messageLength_TextBox.Text = "";
                }
                else if (enteredNumber == 4 || enteredNumber == 8 || enteredNumber == 16 || enteredNumber == 32)
                {
                    // Присваиваем введенное число переменной messageLength
                    messageLength = enteredNumber;
                    messageLength_validationMessage.Text = "";
                    messageLength_resetedMessage.Text = "";
                    messageLength_confirmedMessage.Text = $"Выбранное значение: {messageLength}";

                    // Очистка messageLength_TextBox
                    messageLength_TextBox.Text = "";
                    messageLength_TextBox.IsEnabled = false;
                    messageLength_confirmButton.IsEnabled = false;

                    messageLength_resetButton.IsEnabled = true;
                }
                else
                {
                    // Выводим сообщение об ошибке, если введенное число не соответствует ожидаемым значениям
                    messageLength_validationMessage.Text = "Можно ввести только числа 4, 8, 16 или 32.";
                    messageLength_resetedMessage.Text = "";
                    messageLength_confirmedMessage.Text = "";

                    messageLength_TextBox.Text = "";
                }
            }
            else
            {
                messageLength_validationMessage.Text = "Введите корректное число.";
                messageLength_resetedMessage.Text = "";
                messageLength_confirmedMessage.Text = "";

                messageLength_TextBox.Text = "";
            }

            // Проверка наличия несброшенных значений в заданных параметрах моделирования
            CheckModelingParameters();
        }

        private void ResetButton_messageLength_click(object sender, RoutedEventArgs e)
        {
            messageLength = 0;
            messageLength_TextBox.IsEnabled = true;
            messageLength_confirmButton.IsEnabled = true;

            messageLength_confirmedMessage.Text = "";
            messageLength_resetedMessage.Text = "Значение сброшено.";
            messageLength_validationMessage.Text = "";

            messageLength_resetButton.IsEnabled = false;

            // Проверка наличия несброшенных значений в заданных параметрах моделирования
            CheckModelingParameters();

            ResetButton_messageLength_click_called = true;
        }

        private void ConfirmButton_firstValueOfNoise_click(object sender, RoutedEventArgs e)
        {
            // Попытка преобразовать текст из firstValueOfNoise_TextBox в число
            if (int.TryParse(firstValueOfNoise_TextBox.Text, out int enteredNumber))
            {
                // Если успешно преобразовано, проверяем валидность введенного числа
                if (enteredNumber < 0)
                {
                    firstValueOfNoise_validationMessage.Text = "Введите число больше 0.";
                    firstValueOfNoise_resetedMessage.Text = "";
                    firstValueOfNoise_confirmedMessage.Text = "";

                    firstValueOfNoise_TextBox.Text = "";
                }
                else
                {
                    // Присваиваем введенное число первому элементу вектора шума
                    tempFirstValueOfNoise = enteredNumber;

                    firstValueOfNoise_validationMessage.Text = "";
                    firstValueOfNoise_resetedMessage.Text = "";
                    firstValueOfNoise_confirmedMessage.Text = $"Выбранное значение: {tempFirstValueOfNoise}";

                    // Очистка firstValueOfNoise_TextBox
                    firstValueOfNoise_TextBox.Text = "";
                    firstValueOfNoise_TextBox.IsEnabled = false;
                    firstValueOfNoise_confirmButton.IsEnabled = false;

                    firstValueOfNoise_resetButton.IsEnabled = true;
                }
            }
            else
            {
                firstValueOfNoise_validationMessage.Text = "Введите корректное число.";
                firstValueOfNoise_resetedMessage.Text = "";
                firstValueOfNoise_confirmedMessage.Text = "";

                firstValueOfNoise_TextBox.Text = "";
            }

            // Проверка наличия несброшенных значений в заданных параметрах моделирования
            CheckModelingParameters();
        }

        private void ResetButton_firstValueOfNoise_click(object sender, RoutedEventArgs e)
        {
            tempFirstValueOfNoise = 0;
            firstValueOfNoise_TextBox.IsEnabled = true;
            firstValueOfNoise_confirmButton.IsEnabled = true;

            firstValueOfNoise_confirmedMessage.Text = "";
            firstValueOfNoise_resetedMessage.Text = "Значение сброшено.";
            firstValueOfNoise_validationMessage.Text = "";

            firstValueOfNoise_resetButton.IsEnabled = false;

            // Проверка наличия несброшенных значений в заданных параметрах моделирования
            CheckModelingParameters();

            ResetButton_firstValueOfNoise_click_called = true;
        }

        private void ConfirmButton_noiseStep_click(object sender, RoutedEventArgs e)
        {
            string input = noiseStep_TextBox.Text;

            if (input.Contains('.'))
            {
                input = input.Replace('.', ',');
            }

            // Попытка преобразовать текст из noiseStep_TextBox в число
            if (double.TryParse(input, out double enteredNumber))
            {
                // Если успешно преобразовано, проверяем валидность введенного числа
                if (enteredNumber <= 0)
                {
                    noiseStep_validationMessage.Text = "Введите число больше 0.";
                    noiseStep_resetedMessage.Text = "";
                    noiseStep_confirmedMessage.Text = "";

                    noiseStep_TextBox.Text = "";
                }
                else
                {
                    // Присваиваем введенное число переменной noiseStep
                    noiseStep = enteredNumber;
                    noiseStep_validationMessage.Text = "";
                    noiseStep_resetedMessage.Text = "";
                    noiseStep_confirmedMessage.Text = $"Выбранное значение: {noiseStep}";

                    // Очистка noiseStep_TextBox
                    noiseStep_TextBox.Text = "";
                    noiseStep_TextBox.IsEnabled = false;
                    noiseStep_confirmButton.IsEnabled = false;

                    noiseStep_resetButton.IsEnabled = true;
                }
            }
            else
            {
                noiseStep_validationMessage.Text = "Введите корректное число.";
                noiseStep_resetedMessage.Text = "";
                noiseStep_confirmedMessage.Text = "";

                noiseStep_TextBox.Text = "";
            }

            // Проверка наличия несброшенных значений в заданных параметрах моделирования
            CheckModelingParameters();
        }

        private void ResetButton_noiseStep_click(object sender, RoutedEventArgs e)
        {
            noiseStep = 0;
            noiseStep_TextBox.IsEnabled = true;
            noiseStep_confirmButton.IsEnabled = true;

            noiseStep_confirmedMessage.Text = "";
            noiseStep_resetedMessage.Text = "Значение сброшено.";
            noiseStep_validationMessage.Text = "";

            noiseStep_resetButton.IsEnabled = false;

            // Проверка наличия несброшенных значений в заданных параметрах моделирования
            CheckModelingParameters();

            ResetButton_noiseStep_click_called = true;
        }

        private void CheckModelingParameters()
        {
            if (numberOfRuns_confirmedMessage.Text.Contains("Выбранное значение")
                && numberOfPackets_confirmedMessage.Text.Contains("Выбранное значение")
                && messageLength_confirmedMessage.Text.Contains("Выбранное значение")
                && firstValueOfNoise_confirmedMessage.Text.Contains("Выбранное значение")
                && noiseStep_confirmedMessage.Text.Contains("Выбранное значение"))
            {
                nextStepButton_screen2.IsEnabled = true; // активация кнопки nextStepButton_screen2
            }
            else
            {
                nextStepButton_screen2.IsEnabled = false; // блокировка кнопки nextStepButton_screen2 (в случае сброса хотя бы одного из введённых значений)
            }
        }

        /// <summary>
        /// Кнопка перехода с экрана #2 (задание параметров моделирования) на экран #1 (стартовое окно)
        /// </summary>
        private void PreviousStepButton_screen2_click(object sender, RoutedEventArgs e)
        {
            // Отображение элементов экрана #1
            startMessage.Visibility = Visibility.Visible;
            author.Visibility = Visibility.Visible;
            startButton.Visibility = Visibility.Visible;

            // Скрытие элементов экрана #2:
            settingModelingParameters_Text.Visibility = Visibility.Hidden;
            codingMethod_StackPanel.Visibility = Visibility.Hidden;
            modulationMethod_StackPanel.Visibility = Visibility.Hidden;
            noiseExposureMethod_StackPanel.Visibility = Visibility.Hidden;
            demodulationMethod_StackPanel.Visibility = Visibility.Hidden;
            decodingMethod_StackPanel.Visibility = Visibility.Hidden;
            numberOfRuns_StackPanel.Visibility = Visibility.Hidden;
            numberOfPackets_StackPanel.Visibility = Visibility.Hidden;
            messageLength_StackPanel.Visibility = Visibility.Hidden;
            firstValueOfNoise_StackPanel.Visibility = Visibility.Hidden;
            noiseStep_StackPanel.Visibility = Visibility.Hidden;
            previousStepButton_screen2.Visibility = Visibility.Hidden;
            nextStepButton_screen2.Visibility = Visibility.Hidden;
        }

        private int previousSelectedDecodingMethod = -1;

        /// <summary>
        /// Кнопка перехода с экрана #2 (задание параметров моделирования) на экран #3
        /// </summary>
        private void NextStepButton_screen2_click(object sender, RoutedEventArgs e)
        {
            // Определение выбранного метода декодирования
            selectedDecodingMethod = decodingMethod_ComboBox.SelectedIndex;

            if (modelRunNumber.Text.Contains("Моделирование завершено."))
            {
                step0_StackPanel.Visibility = Visibility.Visible;

                resetModelingResults.IsEnabled = true;
                showBitErrorPlot.IsEnabled = true;
                showVoronoiDiagrams.IsEnabled = true;
                nextStepButton_step0.IsEnabled = true;
            }
            else if (modelRunNumber.Text.Contains("Моделирование остановлено."))
            {
                step0_StackPanel.Visibility = Visibility.Visible;

                resetModelingResults.IsEnabled = true;
                nextStepButton_step0.IsEnabled = true;
            }
            else
            {
                modelRunNumber.Text = ""; // Очистка содержимого TextBox
                modelRunNumber.AppendText("Программа готова к моделированию." + Environment.NewLine);
                modelRunNumber.AppendText("Нажмите «Начать моделирование»..." + Environment.NewLine);
                modelRunNumber.ScrollToEnd();

                resetModelingResults.IsEnabled = false;
                showBitErrorPlot.IsEnabled = false;
                showVoronoiDiagrams.IsEnabled = false;

                if (!manualModeIsEnabled)
                {
                    nextStepButton_step0.IsEnabled = false;
                }
                else
                {
                    nextStepButton_step0.IsEnabled = true;
                }
            }

            help_step0.Visibility = Visibility.Visible;

            if (ResetButton_numberOfRuns_click_called
                || ResetButton_numberOfPackets_click_called
                || ResetButton_messageLength_click_called
                || ResetButton_firstValueOfNoise_click_called
                || ResetButton_noiseStep_click_called
                || selectedDecodingMethod != previousSelectedDecodingMethod)
            {
                modelRunNumber.Text = ""; // Очистка содержимого TextBox
                modelRunNumber.AppendText("Программа готова к моделированию." + Environment.NewLine);
                modelRunNumber.AppendText("Нажмите «Начать моделирование»..." + Environment.NewLine);
                modelRunNumber.ScrollToEnd();

                if (!manualModeIsEnabled)
                {
                    informationMessage_step0.Text = "";
                    _BhattacharyaParameters_Text.Text = "";
                    sortedIndices_Text.Text = "";
                    definitionOfInfoAndFrozenBits.Text = "";
                    BhattacharyaParameters_frozenBitsIndices_Text.Text = "";
                    BhattacharyaParameters_infoBitsIndices_Text.Text = "";

                    step0_StackPanel.Visibility = Visibility.Hidden;

                    nextStepButton_step0.IsEnabled = false;
                }
                else
                {
                    manualInputMode_StackPanel.Visibility = Visibility.Visible;

                    help_step0.Visibility = Visibility.Hidden;
                }

                numberOfRunsProgressbar.Value = 0;
                startModeling.IsEnabled = true;
                stopModeling.IsEnabled = false;
                resetModelingResults.IsEnabled = false;
                showBitErrorPlot.IsEnabled = false;
                showVoronoiDiagrams.IsEnabled = false;

                previousSelectedDecodingMethod = selectedDecodingMethod;
            }

            // Скрытие элементов, не относящихся к экрану #3
            settingModelingParameters_Text.Visibility = Visibility.Collapsed;
            codingMethod_StackPanel.Visibility = Visibility.Collapsed;
            modulationMethod_StackPanel.Visibility = Visibility.Collapsed;
            noiseExposureMethod_StackPanel.Visibility = Visibility.Collapsed;
            demodulationMethod_StackPanel.Visibility = Visibility.Collapsed;
            decodingMethod_StackPanel.Visibility = Visibility.Collapsed;
            numberOfRuns_StackPanel.Visibility = Visibility.Collapsed;
            numberOfPackets_StackPanel.Visibility = Visibility.Collapsed;
            messageLength_StackPanel.Visibility = Visibility.Collapsed;
            firstValueOfNoise_StackPanel.Visibility = Visibility.Collapsed;
            noiseStep_StackPanel.Visibility = Visibility.Collapsed;
            previousStepButton_screen2.Visibility = Visibility.Collapsed;
            nextStepButton_screen2.Visibility = Visibility.Collapsed;

            // Отображение элементов экрана #3:
            leftSide_StackPanel.Visibility = Visibility.Visible;
            previousStepButton_screen3.Visibility = Visibility.Visible;
            nextStepButton_step0.Visibility = Visibility.Visible;

            if (!manualModeIsEnabled)
            {
                manualInputMode.Visibility = Visibility.Visible;
            }
            else
            {
                manualInputMode.Visibility = Visibility.Hidden;

                automaticMode.Visibility = Visibility.Visible;

                if (ResetButton_numberOfRuns_click_called
                || ResetButton_numberOfPackets_click_called
                || ResetButton_messageLength_click_called
                || ResetButton_firstValueOfNoise_click_called
                || ResetButton_noiseStep_click_called
                || selectedDecodingMethod != previousSelectedDecodingMethod)
                {
                    step0_StackPanel.Visibility = Visibility.Hidden;

                    ResetButton_numberOfRuns_click_called = false;
                    ResetButton_numberOfPackets_click_called = false;
                    ResetButton_messageLength_click_called = false;
                    ResetButton_firstValueOfNoise_click_called = false;
                    ResetButton_noiseStep_click_called = false;

                    sentInfoWord_TextBox.IsEnabled = true;
                    sentInfoWord_confirmButton.IsEnabled = true;
                    sentInfoWord_resetButton.IsEnabled = false;
                    nextStepButton_step0.IsEnabled = false;

                    sentInfoWord_validationMessage.Text = "";
                    sentInfoWord_resetedMessage.Text = "";
                    sentInfoWord_confirmedMessage.Text = "";

                    showSteps.Visibility = Visibility.Hidden;
                }
                else
                {
                    step0_StackPanel.Visibility = Visibility.Visible;

                    ResetButton_numberOfRuns_click_called = false;
                    ResetButton_numberOfPackets_click_called = false;
                    ResetButton_messageLength_click_called = false;
                    ResetButton_firstValueOfNoise_click_called = false;
                    ResetButton_noiseStep_click_called = false;
                }
            }

            manualInputMode_Text.Content = $"Введите информационное слово длиной {messageLength / 2}:";

            stopModeling.IsEnabled = false;
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверка, является ли введенный символ цифрой
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; // Отмена ввода символа, если это не цифра
            }
        }

        private void TextBox_PreviewTextInputForNoiseStep(object sender, TextCompositionEventArgs e)
        {
            // Проверка, является ли введенный символ цифрой, точкой или запятой
            if (!char.IsDigit(e.Text, 0) && e.Text != "." && e.Text != ",")
            {
                e.Handled = true; // Отмена ввода символа, если это не цифра
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Обновление интерфейса в основном потоке
            modelRunNumber.AppendText($"{e.ProgressPercentage} запуск модели..." + Environment.NewLine);

            // Прокрутка вниз
            modelRunNumber.ScrollToEnd();

            // Обновление ProgressBar
            numberOfRunsProgressbar.Value = e.ProgressPercentage;
        }

        private void Main(object sender, DoWorkEventArgs e)
        {
            // Задание параметров моделирования
            int modulatingPointsArray = (messageLength + 1) / 3; // количество точек в модулированном сигнале (каждая точка состоит из трёх бит)
            //int numberOfCheckBits = 4; // количество проверочных бит
            double sig = 0.5; // дисперсия плотности распределения шума
            double errorProbability = 0.19; // вероятность ошибки

            // Расчёт параметров Бхаттачарья и получение индексов замороженных и информационных бит
            (double[] _BhattacharyaParameters, int[] sortedIndices, double[] valuesOfFrozenBits, double[] valuesOfInfoBits, int[] frozenBitsIndices, int[] infoBitsIndices) = BhattacharyaParameters.FindingBhattacharyaParameters(messageLength, errorProbability);

            // Задание массива для идентификации замороженных и информационных бит
            int[] identificationOfFrozenAndInfoBits = new int[messageLength];

            // Инициализация массива идентификации замороженных и информационных бит
            for (int i = 0; i < messageLength; i++)
            {
                // Поиск индексов замороженных и информационных бит и их идентификация
                for (int j = 0; j < messageLength / 2; j++)
                {
                    if (i == frozenBitsIndices[j])
                    {
                        identificationOfFrozenAndInfoBits[i] = 0; // замороженный бит
                    }
                }
                for (int j = 0; j < messageLength / 2; j++)
                {
                    if (i == infoBitsIndices[j])
                    {
                        identificationOfFrozenAndInfoBits[i] = 1; // информационный бит
                    }
                }
            }

            // Построение матрицы G
            int[,] G = Gmatrix.ConstructionOfGmatrix(messageLength);

            // Создание сигнального созвездия
            (Complex[] pointsArray, double[] R) = Constellation.ConstellationConstruction();

            // Описание прямых для областей Вороного
            (double[] a, double[] b) = StraightLines.GettingPointCoordinatesForStraightLines(pointsArray);

            // Заполнение вектора шума значениями с заданным шагом
            for (int i = 1; i < numberOfRuns; i++)
            {
                noise[i] = noise[i - 1] + noiseStep;
            }

            demodulationError = new double[numberOfRuns]; // вектор ошибок после демодуляции
            decodingError = new double[numberOfRuns]; // вектор ошибок после декодирования

            int[] codedMessage = new int[messageLength];
            int[] precodedMessage = new int[messageLength];

            // Моделирование работы канала связи для заданного количества запусков (numberOfRuns)
            for (int i = 0; i < numberOfRuns; i++)
            {
                if (!resetModelingSignal)
                {
                    // Отправка сообщения в основной поток для обновления интерфейса
                    backgroundWorker.ReportProgress(i + 1);
                }

                // Моделирование работы канала связи для заданного количества тестов в одной точке (numberOfPackets)
                for (int j = 0; j < numberOfPackets; j++)
                {
                    if (!resetModelingSignal)
                    {
                        if (!manualModeIsEnabled)
                        {
                            if (showStepsInAutomaticMode_if_1)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    informationMessage_step0.Text = "";
                                    informationMessage_step0.Text = "Операции до начала кодирования";

                                    _BhattacharyaParameters_Text.Text = "";
                                    _BhattacharyaParameters_Text.Text = "Рассчитанные и отсортированные по возрастанию параметры Бхаттачарья:" + Environment.NewLine + Environment.NewLine;

                                    for (int k = 0; k < _BhattacharyaParameters.Length; k++)
                                    {
                                        if (k < (_BhattacharyaParameters.Length - 1))
                                        {
                                            _BhattacharyaParameters_Text.Text += _BhattacharyaParameters[k] + " | ";
                                        }
                                        else
                                        {
                                            _BhattacharyaParameters_Text.Text += _BhattacharyaParameters[k];
                                        }
                                    }

                                    sortedIndices_Text.Text = "";
                                    sortedIndices_Text.Text = "Индексы параметров Бхаттачарья после их сортировки по возрастанию:" + Environment.NewLine + Environment.NewLine;

                                    for (int k = 0; k < sortedIndices.Length; k++)
                                    {
                                        sortedIndices_Text.Text += sortedIndices[k] + "  ";
                                    }

                                    definitionOfInfoAndFrozenBits.Text = "";

                                    if (messageLength == 32 || messageLength == 16)
                                    {
                                        definitionOfInfoAndFrozenBits.Text = $"Первые {messageLength / 2} бит были определены, как информационные, вторые {messageLength / 2} бит - как замороженные:" + Environment.NewLine + Environment.NewLine;
                                    }
                                    else
                                    {
                                        definitionOfInfoAndFrozenBits.Text = $"Первые {messageLength / 2} бита были определены, как информационные, вторые {messageLength / 2} бита - как замороженные:" + Environment.NewLine + Environment.NewLine;
                                    }

                                    for (int k = 0; k < sortedIndices.Length; k++)
                                    {
                                        Run run = new Run(sortedIndices[k] + "  ");

                                        if (k < sortedIndices.Length / 2)
                                        {
                                            run.Foreground = Brushes.Green;
                                        }
                                        else
                                        {
                                            run.Foreground = Brushes.Blue;
                                        }

                                        definitionOfInfoAndFrozenBits.Inlines.Add(run);
                                    }

                                    // Отображение индексов замороженных бит
                                    BhattacharyaParameters_frozenBitsIndices_Text.Text = "";
                                    BhattacharyaParameters_frozenBitsIndices_Text.Text = "Итоговые индексы замороженных бит (по возрастанию):" + Environment.NewLine;

                                    for (int k = 0; k < frozenBitsIndices.Length; k++)
                                    {
                                        Run run = new Run(frozenBitsIndices[k] + "  ")
                                        {
                                            Foreground = Brushes.Blue
                                        };

                                        BhattacharyaParameters_frozenBitsIndices_Text.Inlines.Add(run);
                                    }

                                    // Отображение индексов информационных бит
                                    BhattacharyaParameters_infoBitsIndices_Text.Text = "";
                                    BhattacharyaParameters_infoBitsIndices_Text.Text = "Итоговые индексы информационных бит (по возрастанию):" + Environment.NewLine;

                                    for (int k = 0; k < infoBitsIndices.Length; k++)
                                    {
                                        Run run = new Run(infoBitsIndices[k] + "  ")
                                        {
                                            Foreground = Brushes.Green
                                        };

                                        BhattacharyaParameters_infoBitsIndices_Text.Inlines.Add(run);
                                    }
                                });

                                // Получаем количество строк и столбцов в матрице G
                                int rowsOfG = G.GetLength(0);
                                int colsOfG = G.GetLength(1);

                                Dispatcher.Invoke(() =>
                                {
                                    GmatrixMessage.Text = "";
                                    GmatrixMessage.Text = $"Построение порождающей матрицы (произведение перестановочной матрицы на {Math.Log(messageLength, 2)} степень кронекерова произведения матрицы (1, 0; 1, 1):";
                                });

                                Dispatcher.Invoke(() =>
                                {
                                    if (messageLength == 32)
                                    {
                                        Gmatrix_Text.FontSize = 12;
                                    }
                                    else if (messageLength == 16)
                                    {
                                        Gmatrix_Text.FontSize = 16;
                                    }
                                    else if (messageLength == 8)
                                    {
                                        Gmatrix_Text.FontSize = 20;
                                    }
                                    else if (messageLength == 4)
                                    {
                                        Gmatrix_Text.FontSize = 20;
                                    }

                                    Gmatrix_Text.Text = "";
                                });

                                // Перебираем элементы матрицы и добавляем их в Gmatrix_Text
                                for (int k = 0; k < rowsOfG; k++)
                                {
                                    for (int l = 0; l < colsOfG; l++)
                                    {
                                        Dispatcher.Invoke(() =>
                                        {
                                            // Добавляем элемент матрицы в Gmatrix_Text с пробелом после каждого элемента
                                            Gmatrix_Text.Text += G[k, l] + "  ";
                                        });
                                    }

                                    Dispatcher.Invoke(() =>
                                    {
                                        // Добавляем новую строку после каждой строки матрицы
                                        Gmatrix_Text.Text += Environment.NewLine;
                                    });
                                }

                                showStepsInAutomaticMode_if_1 = false;
                            }
                        }

                        if (!manualModeIsEnabled)
                        {
                            if (showStepsInAutomaticMode_if_1_2)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    informationMessage_step05.Text = "";
                                    informationMessage_step05.Text = "Операции до начала кодирования";
                                });

                                showStepsInAutomaticMode_if_1_2 = false;
                            }
                        }

                        // Инициализация исходного слова
                        Random randi = new Random();
                        int[] sentInfoWord = new int[infoBitsIndices.Length]; // массив для исходного слова

                        // Заполнение исходного слова случайными 0 и 1
                        for (int k = 0; k < infoBitsIndices.Length; k++)
                        {
                            sentInfoWord[k] = randi.Next(2);
                        }

                        if (!manualModeIsEnabled)
                        {
                            if (showStepsInAutomaticMode_if_2)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    informationMessage_step1.Text = "";
                                    informationMessage_step1.Text = "Поэтапная визуализация преобразования одного из сообщений моделируемого потока";

                                    sentInfoWord_Text.Text = "Исходное информационное сообщение:" + Environment.NewLine + Environment.NewLine;

                                    for (int k = 0; k < sentInfoWord.Length; k++)
                                    {
                                        sentInfoWord_Text.Text += sentInfoWord[k] + "  ";
                                    }
                                });

                                showStepsInAutomaticMode_if_2 = false;
                            }
                        }

                        // Кодирование сообщения
                        (codedMessage, precodedMessage) = PolarEncoder.PolarMessageEncoding(messageLength, sentInfoWord, frozenBitsIndices, infoBitsIndices, G);

                        if (!manualModeIsEnabled)
                        {
                            if (showStepsInAutomaticMode_if_3)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    encodingStageMessage.Text = "";
                                    encodingStageMessage.Text = "Этапы кодирования:";

                                    precodedMessage_Text.Text = "";
                                    precodedMessage_Text.Text = "----------" + Environment.NewLine;
                                    precodedMessage_Text.Text += "1) Распределение замороженных и информационных бит в сообщении в соответствии с их индексами:" + Environment.NewLine + Environment.NewLine;

                                    for (int k = 0; k < precodedMessage.Length; k++)
                                    {
                                        Run run = new Run(precodedMessage[k] + "  ");

                                        for (int l = 0; l < frozenBitsIndices.Length; l++)
                                        {
                                            if (k == frozenBitsIndices[l])
                                            {
                                                run.Foreground = Brushes.Blue;
                                            }
                                        }

                                        for (int l = 0; l < infoBitsIndices.Length; l++)
                                        {
                                            if (k == infoBitsIndices[l])
                                            {
                                                run.Foreground = Brushes.Green;
                                            }
                                        }

                                        precodedMessage_Text.Inlines.Add(run);
                                    }

                                    Span span = new Span();

                                    Run run1 = new Run("Примечание: ");
                                    span.Inlines.Add(run1);

                                    Run run2 = new Run("синим");
                                    run2.Foreground = Brushes.Blue;
                                    span.Inlines.Add(run2);

                                    Run run3 = new Run(" показаны замороженные биты, ");
                                    span.Inlines.Add(run3);

                                    Run run4 = new Run("зелёным");
                                    run4.Foreground = Brushes.Green;
                                    span.Inlines.Add(run4);

                                    Run run5 = new Run(" - информационные.");
                                    span.Inlines.Add(run5);

                                    encoding_informationMessage.Text = "";
                                    encoding_informationMessage.Inlines.Add(span);

                                    codedMessage_Text.Text = "";
                                    codedMessage_Text.Text = "----------" + Environment.NewLine;
                                    codedMessage_Text.Text += "2) Умножение сообщения на порождающую матрицу G в соответствии с правилами матричного умножения и взятие остатка от деления на 2 (поэлементно):" + Environment.NewLine + Environment.NewLine;

                                    for (int k = 0; k < codedMessage.Length; k++)
                                    {
                                        Run run = new Run(codedMessage[k] + "  ");

                                        for (int l = 0; l < frozenBitsIndices.Length; l++)
                                        {
                                            if (k == frozenBitsIndices[l])
                                            {
                                                run.Foreground = Brushes.Blue;
                                            }
                                        }

                                        for (int l = 0; l < infoBitsIndices.Length; l++)
                                        {
                                            if (k == infoBitsIndices[l])
                                            {
                                                run.Foreground = Brushes.Green;
                                            }
                                        }

                                        codedMessage_Text.Inlines.Add(run);
                                    }

                                    encoding_finalInformationMessage.Text = "";
                                    encoding_finalInformationMessage.Text = "Полученное сообщение является кодовым вектором." + Environment.NewLine;
                                    encoding_finalInformationMessage.Text += "Кодирование сообщения завершено.";
                                });

                                showStepsInAutomaticMode_if_3 = false;
                            }
                        }

                        // Модуляция сообщения
                        Complex[] modulatedMessage = Modulator.MessageModulating(messageLength, modulatingPointsArray, pointsArray, codedMessage);

                        if (!manualModeIsEnabled)
                        {
                            if (showStepsInAutomaticMode_if_4)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    informationMessage_step2.Text = "";
                                    informationMessage_step2.Text = "Поэтапная визуализация преобразования одного из сообщений моделируемого потока";

                                    modulatingStageMessage.Text = "";
                                    modulatingStageMessage.Text = "Этапы модуляции:";

                                    firstStep_modulatingStageMessage.Text = "";
                                    firstStep_modulatingStageMessage.Text = "----------" + Environment.NewLine;
                                    firstStep_modulatingStageMessage.Text += "1) Длина модулированного сигнала определяется, как длина кода + 1 / 3. Кодовый вектор разбивается на группы по три бита:" + Environment.NewLine + Environment.NewLine;

                                    for (int k = 0; k < codedMessage.Length; k++)
                                    {
                                        if ((k + 1) % 3 == 0)
                                        {
                                            firstStep_modulatingStageMessage.Text += codedMessage[k] + " | ";
                                        }
                                        else
                                        {
                                            firstStep_modulatingStageMessage.Text += codedMessage[k] + " ";
                                        }

                                        if (k == codedMessage.Length - 1)
                                        {
                                            firstStep_modulatingStageMessage.Text += "0"; // дополнительный бит, добавляемый к концу кодового вектора при модуляции
                                        }
                                    }

                                    secondStep_modulatingStageMessage.Text = "";
                                    secondStep_modulatingStageMessage.Text = "----------" + Environment.NewLine;
                                    secondStep_modulatingStageMessage.Text += "2) В качестве метода модуляции выбирается восьмипозиционная фазовая модуляция (8-PSK) с заданным сигнальным созвездием:";

                                    CreateConstellationPlot();
                                });

                                showStepsInAutomaticMode_if_4 = false;
                            }
                        }

                        if (!manualModeIsEnabled)
                        {
                            if (showStepsInAutomaticMode_if_5)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    informationMessage_step3.Text = "";
                                    informationMessage_step3.Text = "Поэтапная визуализация преобразования одного из сообщений моделируемого потока";

                                    thirdStep_modulatingStageMessage.Text = "";
                                    thirdStep_modulatingStageMessage.Text = "----------" + Environment.NewLine;
                                    thirdStep_modulatingStageMessage.Text += "3) Координаты точек представляются в комплексном виде:";

                                    complexPointsArray.Text = "";

                                    Complex pointsArray0Complex = pointsArray[0];
                                    string formattedPointsArray0Complex = $"{pointsArray0Complex.Real} + {pointsArray0Complex.Imaginary}i";
                                    complexPointsArray.Text += "Точка 100: " + formattedPointsArray0Complex + Environment.NewLine;

                                    Complex pointsArray1Complex = pointsArray[1];
                                    string formattedPointsArray1Complex = $"{pointsArray1Complex.Real} + {pointsArray1Complex.Imaginary}i";
                                    complexPointsArray.Text += "Точка 001: " + formattedPointsArray1Complex + Environment.NewLine;

                                    Complex pointsArray2Complex = pointsArray[2];
                                    string formattedPointsArray2Complex = $"{pointsArray2Complex.Real} + {pointsArray2Complex.Imaginary}i";
                                    complexPointsArray.Text += "Точка 000: " + formattedPointsArray2Complex + Environment.NewLine;

                                    Complex pointsArray3Complex = pointsArray[3];
                                    string formattedPointsArray3Complex = $"{pointsArray3Complex.Real} + {pointsArray3Complex.Imaginary}i";
                                    complexPointsArray.Text += "Точка 101: " + formattedPointsArray3Complex + Environment.NewLine;

                                    Complex pointsArray4Complex = pointsArray[4];
                                    string formattedPointsArray4Complex = $"{pointsArray4Complex.Real} + {pointsArray4Complex.Imaginary}i";
                                    complexPointsArray.Text += "Точка 010: " + formattedPointsArray4Complex + Environment.NewLine;

                                    Complex pointsArray5Complex = pointsArray[5];
                                    string formattedPointsArray5Complex = $"{pointsArray5Complex.Real} + {pointsArray5Complex.Imaginary}i";
                                    complexPointsArray.Text += "Точка 110: " + formattedPointsArray5Complex + Environment.NewLine;

                                    Complex pointsArray6Complex = pointsArray[6];
                                    string formattedPointsArray6Complex = $"{pointsArray6Complex.Real} + {pointsArray6Complex.Imaginary}i";
                                    complexPointsArray.Text += "Точка 011: " + formattedPointsArray6Complex + Environment.NewLine;

                                    Complex pointsArray7Complex = pointsArray[7];
                                    string formattedPointsArray7Complex = $"{pointsArray7Complex.Real} + {pointsArray7Complex.Imaginary}i";
                                    complexPointsArray.Text += "Точка 111: " + formattedPointsArray7Complex;

                                    fourthStep_modulatingStageMessage.Text = "";
                                    fourthStep_modulatingStageMessage.Text = "----------" + Environment.NewLine;
                                    fourthStep_modulatingStageMessage.Text += "4) Каждому значению модулированного сигнала присваивается значение одной из восьми точек созвездия:";

                                    modulatedMessage_Text.Text = "";

                                    for (int k = 0; k < modulatedMessage.Length; k++)
                                    {
                                        // Получить текущее комплексное число
                                        Complex currentComplexNumber = modulatedMessage[k];

                                        // Форматировать комплексное число как строку
                                        string formattedComplexNumber = $"{currentComplexNumber.Real} + {currentComplexNumber.Imaginary}i";

                                        // Добавить отформатированное комплексное число к тексту
                                        modulatedMessage_Text.Text += formattedComplexNumber;

                                        // Добавить пробел между числами, если это не последнее число
                                        if (k < modulatedMessage.Length - 1)
                                        {
                                            modulatedMessage_Text.Text += " | ";
                                        }
                                    }

                                    modulating_finalInformationMessage.Text = "";
                                    modulating_finalInformationMessage.Text = "Полученное сообщение является модулированным сигналом." + Environment.NewLine;
                                    modulating_finalInformationMessage.Text += "Модуляция сообщения завершена.";
                                });

                                showStepsInAutomaticMode_if_5 = false;
                            }
                        }

                        // Добавление шума к сообщению
                        int L = modulatedMessage.Length;
                        double SNR = Math.Pow(10, noise[i] / 10); // SNR в линейной шкале
                        double Esym = 0;

                        for (int k = 0; k < L; k++)
                        {
                            Esym += Complex.Abs(modulatedMessage[k]) * Complex.Abs(modulatedMessage[k]);
                        }

                        Esym /= L; // Рассчитываем фактическую энергию символа
                        double N0 = Esym / SNR; // Находим спектральную плотность шума
                        Complex[] n = new Complex[L];

                        bool isReal = true;

                        foreach (var value in modulatedMessage)
                        {
                            if (value.Imaginary != 0)
                            {
                                isReal = false;
                                break;
                            }
                        }

                        double noiseSigma;

                        if (isReal)
                        {
                            noiseSigma = Math.Sqrt(N0); // Стандартное отклонение для AWGN-шума, когда x - вещественное
                        }
                        else
                        {
                            noiseSigma = Math.Sqrt(N0 / 2); // Стандартное отклонение для AWGN-шума, когда x - комплексное
                        }

                        for (int k = 0; k < L; k++)
                        {
                            if (isReal)
                            {
                                Random random = new Random();

                                double u1 = 1.0 - random.NextDouble();
                                double u2 = 1.0 - random.NextDouble();
                                double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

                                n[k] = new Complex(noiseSigma * randStdNormal, 0);
                            }
                            else
                            {
                                Random random_1 = new Random();

                                double u1_1 = 1.0 - random_1.NextDouble();
                                double u2_1 = 1.0 - random_1.NextDouble();
                                double randStdNormal_1 = Math.Sqrt(-2.0 * Math.Log(u1_1)) * Math.Sin(2.0 * Math.PI * u2_1);

                                Random random_2 = new Random();

                                double u1_2 = 1.0 - random_2.NextDouble();
                                double u2_2 = 1.0 - random_2.NextDouble();
                                double randStdNormal_2 = Math.Sqrt(-2.0 * Math.Log(u1_2)) * Math.Sin(2.0 * Math.PI * u2_2);

                                n[k] = new Complex(noiseSigma * randStdNormal_1, noiseSigma * randStdNormal_2);
                            }
                        }

                        Complex[] complexNoisyMessage = new Complex[L];

                        for (int k = 0; k < L; k++)
                        {
                            complexNoisyMessage[k] = modulatedMessage[k] + n[k];
                        }

                        if (!manualModeIsEnabled)
                        {
                            if (showStepsInAutomaticMode_if_6)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    informationMessage_step4.Text = "";
                                    informationMessage_step4.Text = "Поэтапная визуализация преобразования одного из сообщений моделируемого потока";

                                    noiseExposureMessage.Text = "";
                                    noiseExposureMessage.Text = "Воздействие шума:";

                                    firstStep_noiseExposure.Text = "";
                                    firstStep_noiseExposure.Text = "----------" + Environment.NewLine;
                                    firstStep_noiseExposure.Text += $"1) Генирируется вектор шума размерности значения количества запусков модели ({numberOfRuns}) с заданным шагом между значениями ({noiseStep}):";

                                    noise_Text.Text = "";

                                    for (int k = 0; k < noise.Length; k++)
                                    {
                                        if (k < noise.Length - 1)
                                        {
                                            noise_Text.Text += noise[k] + " | ";
                                        }
                                        else
                                        {
                                            noise_Text.Text += noise[k];
                                        }
                                    }

                                    secondStep_noiseExposure.Text = "";
                                    secondStep_noiseExposure.Text = "----------" + Environment.NewLine;
                                    secondStep_noiseExposure.Text += "2) Вектор шума воздействует на модулированное сообщение:";

                                    complexNoisyMessage_Text.Text = "";

                                    for (int k = 0; k < complexNoisyMessage.Length; k++)
                                    {
                                        // Получить текущее комплексное число
                                        Complex currentComplexNumber = complexNoisyMessage[k];

                                        // Форматировать комплексное число как строку
                                        string formattedComplexNumber = $"{currentComplexNumber.Real} + {currentComplexNumber.Imaginary}i";

                                        // Добавить отформатированное комплексное число к тексту
                                        complexNoisyMessage_Text.Text += formattedComplexNumber;

                                        // Добавить пробел между числами, если это не последнее число
                                        if (k < complexNoisyMessage.Length - 1)
                                        {
                                            complexNoisyMessage_Text.Text += " | ";
                                        }
                                    }

                                    noiseExposure_finalInformationMessage.Text = "";

                                    noiseExposure_finalInformationMessage.Text = "Полученное сообщение является зашумленным сигналом." + Environment.NewLine;
                                    noiseExposure_finalInformationMessage.Text += "Воздействие шума на модулированное сообщение завершено.";
                                });

                                showStepsInAutomaticMode_if_6 = false;
                            }
                        }

                        // Демодуляция сообщения
                        int[] demodulatedMessage = Demodulator.MessageDemodulating(messageLength, sig, pointsArray, a, b, complexNoisyMessage);

                        // Приведение демодулированного сообщения к верной длине
                        int[] rightDemodulatedMessage = new int[messageLength];
                        for (int k = 0; k < messageLength; k++)
                        {
                            rightDemodulatedMessage[k] = demodulatedMessage[k];
                        }

                        if (!manualModeIsEnabled)
                        {
                            if (showStepsInAutomaticMode_if_7)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    informationMessage_step5.Text = "";
                                    informationMessage_step5.Text = "Поэтапная визуализация преобразования одного из сообщений моделируемого потока";

                                    demodulatingStageMessage.Text = "";
                                    demodulatingStageMessage.Text = "Этапы демодуляции:";

                                    firstStep_demodulatingStageMessage.Text = "";
                                    firstStep_demodulatingStageMessage.Text = "----------" + Environment.NewLine;
                                    firstStep_demodulatingStageMessage.Text += "1) В качестве метода демодуляции выбирается метод построения областей Вороного.";

                                    secondStep_demodulatingStageMessage.Text = "";
                                    secondStep_demodulatingStageMessage.Text = "----------" + Environment.NewLine;
                                    secondStep_demodulatingStageMessage.Text += "2) Расчёт коэффициентов прямых a и b, разделяющих две соседние точки:";

                                    a_Text.Text = "";
                                    a_Text.Text = "Прямая a: ";

                                    for (int k = 0; k < a.Length; k++)
                                    {
                                        if (k < a.Length - 1)
                                        {
                                            a_Text.Text += a[k] + " | ";
                                        }
                                        else
                                        {
                                            a_Text.Text += a[k];
                                        }
                                    }

                                    b_Text.Text = "";
                                    b_Text.Text = "Прямая b: ";

                                    for (int k = 0; k < b.Length; k++)
                                    {
                                        if (k < b.Length - 1)
                                        {
                                            b_Text.Text += b[k] + " | ";
                                        }
                                        else
                                        {
                                            b_Text.Text += b[k];
                                        }
                                    }
                                });

                                showStepsInAutomaticMode_if_7 = false;
                            }
                        }

                        if (!manualModeIsEnabled)
                        {
                            if (showStepsInAutomaticMode_if_8)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    informationMessage_step6.Text = "";
                                    informationMessage_step6.Text = "Поэтапная визуализация преобразования одного из сообщений моделируемого потока";

                                    thirdStep_demodulatingStageMessage.Text = "";
                                    thirdStep_demodulatingStageMessage.Text = "----------" + Environment.NewLine;
                                    thirdStep_demodulatingStageMessage.Text += "3) Вычисление мягких метрик для каждой группы из трёх битов (для каждого значения: 0 и 1) и выяснение побитовой принадлежности принятой точки одной из областей (более подробно в справке):";

                                    rightDemodulatedMessage_Text.Text = "";

                                    for (int k = 0; k < rightDemodulatedMessage.Length; k++)
                                    {
                                        rightDemodulatedMessage_Text.Text += rightDemodulatedMessage[k] + "  ";
                                    }

                                    demodulatingStageMessage_preFinalInformationMessage.Text = "";
                                    demodulatingStageMessage_preFinalInformationMessage.Text += "Полученное сообщение является демодулированным сигналом.";

                                    comparisonOfCodedMessageAndRightDemodulatedMessage.Text = "";
                                    comparisonOfCodedMessageAndRightDemodulatedMessage.Text = "----------" + Environment.NewLine;
                                    comparisonOfCodedMessageAndRightDemodulatedMessage.Text += "Сравнение кодового слова и демодулированного сигнала:";

                                    codedMessage_Text_forComparison.Text = "";

                                    for (int k = 0; k < codedMessage.Length; k++)
                                    {
                                        Run run = new Run(codedMessage[k] + "  ");

                                        for (int l = 0; l < frozenBitsIndices.Length; l++)
                                        {
                                            if (k == frozenBitsIndices[l])
                                            {
                                                run.Foreground = Brushes.Blue;
                                            }
                                        }

                                        for (int l = 0; l < infoBitsIndices.Length; l++)
                                        {
                                            if (k == infoBitsIndices[l])
                                            {
                                                run.Foreground = Brushes.Green;
                                            }
                                        }

                                        codedMessage_Text_forComparison.Inlines.Add(run);
                                    }

                                    rightDemodulatedMessage_Text_forComparison.Text = "";

                                    for (int k = 0; k < rightDemodulatedMessage.Length; k++)
                                    {
                                        Run run = new Run(rightDemodulatedMessage[k] + "  ");

                                        for (int l = 0; l < frozenBitsIndices.Length; l++)
                                        {
                                            if (k == frozenBitsIndices[l])
                                            {
                                                run.Foreground = Brushes.Blue;
                                            }
                                        }

                                        for (int l = 0; l < infoBitsIndices.Length; l++)
                                        {
                                            if (k == infoBitsIndices[l])
                                            {
                                                run.Foreground = Brushes.Green;
                                            }
                                        }

                                        rightDemodulatedMessage_Text_forComparison.Inlines.Add(run);
                                    }

                                    resultOfComparisonCodedMessageAndRightDemodulatedMessage.Text = "";

                                    if (codedMessage.SequenceEqual(rightDemodulatedMessage))
                                    {
                                        resultOfComparisonCodedMessageAndRightDemodulatedMessage.Text = "Демодуляция сообщения завершена успешно.";
                                    }
                                    else
                                    {
                                        resultOfComparisonCodedMessageAndRightDemodulatedMessage.Text = "Демодуляция сообщения завершена с ошибкой (ошибками).";
                                    }
                                });

                                showStepsInAutomaticMode_if_8 = false;
                            }
                        }

                        // Декодирование сообщения
                        int[] decodedMessage = new int[messageLength];
                        if (selectedDecodingMethod == 0)
                        {
                            decodedMessage = DirectDecoder.MessageDecoding(messageLength, errorProbability, identificationOfFrozenAndInfoBits, G, rightDemodulatedMessage);
                        }
                        else if (selectedDecodingMethod == 1)
                        {
                            decodedMessage = RecursiveDecoder.MessageDecoding(messageLength, errorProbability, identificationOfFrozenAndInfoBits, precodedMessage, rightDemodulatedMessage);
                        }

                        if (!manualModeIsEnabled)
                        {
                            if (showStepsInAutomaticMode_if_9)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    informationMessage_step7.Text = "";
                                    informationMessage_step7.Text = "Поэтапная визуализация преобразования одного из сообщений моделируемого потока";

                                    decodingStageMessage.Text = "";
                                    decodingStageMessage.Text = "Этапы декодирования:";

                                    if (selectedDecodingMethod == 0)
                                    {
                                        firstStep_decodingStageMessage.Text = "";
                                        firstStep_decodingStageMessage.Text = "----------" + Environment.NewLine;
                                        firstStep_decodingStageMessage.Text += "1) В качестве метода декодирования был выбран прямой метод.";
                                    }
                                    else if (selectedDecodingMethod == 1)
                                    {
                                        firstStep_decodingStageMessage.Text = "";
                                        firstStep_decodingStageMessage.Text = "----------" + Environment.NewLine;
                                        firstStep_decodingStageMessage.Text += "1) В качестве метода декодирования был выбран рекурсивный метод.";
                                    }

                                    secondStep_decodingStageMessage.Text = "";
                                    secondStep_decodingStageMessage.Text = "----------" + Environment.NewLine;
                                    secondStep_decodingStageMessage.Text += "2) По принятым жёстким решениям вычисляется вектор, который является оценкой вектора кодового слова, то есть находится предкодовый вектор:";

                                    decodedMessage_Text.Text = "";

                                    for (int k = 0; k < decodedMessage.Length; k++)
                                    {
                                        Run run = new Run(decodedMessage[k] + "  ");

                                        for (int l = 0; l < frozenBitsIndices.Length; l++)
                                        {
                                            if (k == frozenBitsIndices[l])
                                            {
                                                run.Foreground = Brushes.Blue;
                                            }
                                        }

                                        for (int l = 0; l < infoBitsIndices.Length; l++)
                                        {
                                            if (k == infoBitsIndices[l])
                                            {
                                                run.Foreground = Brushes.Green;
                                            }
                                        }

                                        decodedMessage_Text.Inlines.Add(run);
                                    }
                                });

                                showStepsInAutomaticMode_if_9 = false;
                            }
                        }

                        // Выделение информационного слова
                        int[] receivedInfoWord = new int[messageLength / 2];
                        for (int k = 0; k < receivedInfoWord.Length; k++)
                        {
                            receivedInfoWord[k] = decodedMessage[infoBitsIndices[k]];
                        }

                        if (!manualModeIsEnabled)
                        {
                            if (showStepsInAutomaticMode_if_10)
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    thirdStep_decodingStageMessage.Text = "";
                                    thirdStep_decodingStageMessage.Text = "----------" + Environment.NewLine;
                                    thirdStep_decodingStageMessage.Text += "3) По индексам информационных бит из предкодового вектора выделяется декодированное сообщение:";

                                    receivedInfoWord_Text.Text = "";

                                    for (int k = 0; k < receivedInfoWord.Length; k++)
                                    {
                                        Run run = new Run(receivedInfoWord[k] + "  ");

                                        for (int l = 0; l < infoBitsIndices.Length; l++)
                                        {
                                            run.Foreground = Brushes.Green;
                                        }

                                        receivedInfoWord_Text.Inlines.Add(run);
                                    }

                                    comparisonOfSentInfoWordAndReceivedInfoWord.Text = "";
                                    comparisonOfSentInfoWordAndReceivedInfoWord.Text = "----------" + Environment.NewLine;
                                    comparisonOfSentInfoWordAndReceivedInfoWord.Text += "Сравнение информационного слова и декодированного сообщения:";

                                    sentInfoWord_Text_forComparison.Text = "";

                                    for (int k = 0; k < sentInfoWord.Length; k++)
                                    {
                                        sentInfoWord_Text_forComparison.Text += sentInfoWord[k] + "  ";
                                    }

                                    receivedInfoWord_Text_forComparison.Text = "";

                                    for (int k = 0; k < receivedInfoWord.Length; k++)
                                    {
                                        receivedInfoWord_Text_forComparison.Text += receivedInfoWord[k] + "  ";
                                    }

                                    resultOfComparisonSentInfoWordAndReceivedInfoWord.Text = "";

                                    if (sentInfoWord.SequenceEqual(receivedInfoWord))
                                    {
                                        resultOfComparisonSentInfoWordAndReceivedInfoWord.Text = "Декодирование сообщения завершено успешно.";
                                    }
                                    else
                                    {
                                        resultOfComparisonSentInfoWordAndReceivedInfoWord.Text = "Декодирование сообщения завершено с ошибкой (ошибками).";
                                    }
                                });

                                showStepsInAutomaticMode_if_10 = false;
                            }
                        }

                        // Вычисление ошибок демодуляции
                        double demodErr = 0;
                        for (int k = 0; k < rightDemodulatedMessage.Length; k++)
                        {
                            demodErr += (double)((codedMessage[k] + rightDemodulatedMessage[k]) % 2) / (messageLength * numberOfPackets);
                        }

                        demodulationError[i] += demodErr;

                        // Вычисление ошибок декодирования
                        double decodErr = 0;
                        for (int k = 0; k < receivedInfoWord.Length; k++)
                        {
                            decodErr += (double)((receivedInfoWord[k] + sentInfoWord[k]) % 2) / (messageLength * numberOfPackets);
                        }

                        decodingError[i] += decodErr;
                    }
                }
            }

            if (resetModelingSignal)
            {
                Dispatcher.Invoke(() =>
                {
                    modelRunNumber.AppendText(Environment.NewLine + "Моделирование остановлено.");
                    modelRunNumber.ScrollToEnd();

                    stopModeling.IsEnabled = false;
                    resetModelingResults.IsEnabled = true;
                    previousStepButton_screen3.IsEnabled = true;

                    resetModelingSignal = false;
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    modelRunNumber.AppendText(Environment.NewLine + "Моделирование завершено.");
                    modelRunNumber.ScrollToEnd();

                    stopModeling.IsEnabled = false;
                    resetModelingResults.IsEnabled = true;
                    showBitErrorPlot.IsEnabled = true;
                    showVoronoiDiagrams.IsEnabled = true;
                    previousStepButton_screen3.IsEnabled = true;
                });
            }
        }

        private void ManualInputMode_click(object sender, RoutedEventArgs e)
        {
            manualInputMode.Visibility = Visibility.Hidden;
            automaticMode.Visibility = Visibility.Visible;

            manualInputMode_StackPanel.Visibility = Visibility.Visible;
            showSteps.Visibility = Visibility.Hidden;

            step0_StackPanel.Visibility = Visibility.Hidden;

            help_step0.Visibility = Visibility.Hidden;

            nextStepButton_step0.IsEnabled = false;

            sentInfoWord_TextBox.IsEnabled = true;
            sentInfoWord_confirmButton.IsEnabled = true;
            sentInfoWord_resetButton.IsEnabled = false;
            showSteps.Visibility = Visibility.Hidden;

            sentInfoWord_validationMessage.Text = "";
            sentInfoWord_resetedMessage.Text = "";
            sentInfoWord_confirmedMessage.Text = "";

            sentInfoWord_TextBox.TextChanged += SentInfoWord_TextChanged;
        }

        private void AutomaticMode_click(object sender, RoutedEventArgs e)
        {
            manualModeIsEnabled = false;

            manualInputMode.Visibility = Visibility.Visible;
            automaticMode.Visibility = Visibility.Hidden;
            manualInputMode_StackPanel.Visibility = Visibility.Hidden;

            help_step0.Visibility = Visibility.Visible;

            if (stopModeling.IsEnabled)
            {
                showStepsInAutomaticMode_if_1 = true;
                showStepsInAutomaticMode_if_1_2 = true;
                showStepsInAutomaticMode_if_2 = true;
                showStepsInAutomaticMode_if_3 = true;
                showStepsInAutomaticMode_if_4 = true;
                showStepsInAutomaticMode_if_5 = true;
                showStepsInAutomaticMode_if_6 = true;
                showStepsInAutomaticMode_if_7 = true;
                showStepsInAutomaticMode_if_8 = true;
                showStepsInAutomaticMode_if_9 = true;
                showStepsInAutomaticMode_if_10 = true;

                nextStepButton_step0.IsEnabled = true;

                step0_StackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                informationMessage_step0.Text = "";
                _BhattacharyaParameters_Text.Text = "";
                sortedIndices_Text.Text = "";
                definitionOfInfoAndFrozenBits.Text = "";
                BhattacharyaParameters_frozenBitsIndices_Text.Text = "";
                BhattacharyaParameters_infoBitsIndices_Text.Text = "";

                nextStepButton_step0.IsEnabled = false;

                step0_StackPanel.Visibility = Visibility.Hidden;
            }
        }

        private void SentInfoWord_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Проверяем, превышает ли текущая длина текста половину значения messageLength
            if (sentInfoWord_TextBox.Text.Length > messageLength / 2)
            {
                // Если превышает, обрезаем текст до нужной длины
                sentInfoWord_TextBox.Text = sentInfoWord_TextBox.Text.Substring(0, messageLength / 2);
                // Устанавливаем курсор в конец текста
                sentInfoWord_TextBox.CaretIndex = sentInfoWord_TextBox.Text.Length;
            }
        }

        private void TextBox_PreviewTextInput_ManualInputMode(object sender, TextCompositionEventArgs e)
        {
            // Проверка, является ли введенный символ '0' или '1'
            if (!(e.Text == "0" || e.Text == "1"))
            {
                e.Handled = true; // Отмена ввода символа, если это не '0' или '1'
            }
        }

        private int[] sentInfoWord_manual;

        private void ConfirmButton_sentInfoWord_click(object sender, RoutedEventArgs e)
        {
            string inputSentInfoWord = sentInfoWord_TextBox.Text;

            sentInfoWord_validationMessage.FontSize = 14;
            sentInfoWord_resetedMessage.FontSize = 14;
            sentInfoWord_confirmedMessage.FontSize = 14;

            if (messageLength == 32)
            {
                if (inputSentInfoWord.Length == 16)
                {
                    sentInfoWord_manual = new int[inputSentInfoWord.Length];

                    for (int i = 0; i < inputSentInfoWord.Length; i++)
                    {
                        sentInfoWord_manual[i] = int.Parse(inputSentInfoWord[i].ToString());
                    }

                    sentInfoWord_validationMessage.Text = "";
                    sentInfoWord_resetedMessage.Text = "";
                    sentInfoWord_confirmedMessage.Text = "Введённое информационное слово: ";

                    for (int i = 0; i < sentInfoWord_manual.Length; i++)
                    {
                        sentInfoWord_confirmedMessage.Text += sentInfoWord_manual[i];
                    }

                    sentInfoWord_TextBox.Text = "";

                    sentInfoWord_TextBox.IsEnabled = false;
                    sentInfoWord_confirmButton.IsEnabled = false;
                    sentInfoWord_resetButton.IsEnabled = true;
                    showSteps.Visibility = Visibility.Visible;
                }
                else
                {
                    sentInfoWord_validationMessage.Text = $"Введите слово длиной {messageLength / 2}.";
                    sentInfoWord_resetedMessage.Text = "";
                    sentInfoWord_confirmedMessage.Text = "";

                    sentInfoWord_TextBox.Text = "";
                }
            }
            else if (messageLength == 16)
            {
                if (inputSentInfoWord.Length == 8)
                {
                    sentInfoWord_manual = new int[inputSentInfoWord.Length];

                    for (int i = 0; i < inputSentInfoWord.Length; i++)
                    {
                        sentInfoWord_manual[i] = int.Parse(inputSentInfoWord[i].ToString());
                    }

                    sentInfoWord_validationMessage.Text = "";
                    sentInfoWord_resetedMessage.Text = "";
                    sentInfoWord_confirmedMessage.Text = "Введённое информационное слово: ";

                    for (int i = 0; i < sentInfoWord_manual.Length; i++)
                    {
                        sentInfoWord_confirmedMessage.Text += sentInfoWord_manual[i];
                    }

                    sentInfoWord_TextBox.Text = "";

                    sentInfoWord_TextBox.IsEnabled = false;
                    sentInfoWord_confirmButton.IsEnabled = false;
                    sentInfoWord_resetButton.IsEnabled = true;
                    showSteps.Visibility = Visibility.Visible;
                }
                else
                {
                    sentInfoWord_validationMessage.Text = $"Введите слово длиной {messageLength / 2}.";
                    sentInfoWord_resetedMessage.Text = "";
                    sentInfoWord_confirmedMessage.Text = "";

                    sentInfoWord_TextBox.Text = "";
                }
            }
            else if (messageLength == 8)
            {
                if (inputSentInfoWord.Length == 4)
                {
                    sentInfoWord_manual = new int[inputSentInfoWord.Length];

                    for (int i = 0; i < inputSentInfoWord.Length; i++)
                    {
                        sentInfoWord_manual[i] = int.Parse(inputSentInfoWord[i].ToString());
                    }

                    sentInfoWord_validationMessage.Text = "";
                    sentInfoWord_resetedMessage.Text = "";
                    sentInfoWord_confirmedMessage.Text = "Введённое информационное слово: ";

                    for (int i = 0; i < sentInfoWord_manual.Length; i++)
                    {
                        sentInfoWord_confirmedMessage.Text += sentInfoWord_manual[i];
                    }

                    sentInfoWord_TextBox.Text = "";

                    sentInfoWord_TextBox.IsEnabled = false;
                    sentInfoWord_confirmButton.IsEnabled = false;
                    sentInfoWord_resetButton.IsEnabled = true;
                    showSteps.Visibility = Visibility.Visible;
                }
                else
                {
                    sentInfoWord_validationMessage.Text = $"Введите слово длиной {messageLength / 2}.";
                    sentInfoWord_resetedMessage.Text = "";
                    sentInfoWord_confirmedMessage.Text = "";

                    sentInfoWord_TextBox.Text = "";
                }
            }
            else if (messageLength == 4)
            {
                if (inputSentInfoWord.Length == 2)
                {
                    sentInfoWord_manual = new int[inputSentInfoWord.Length];

                    for (int i = 0; i < inputSentInfoWord.Length; i++)
                    {
                        sentInfoWord_manual[i] = int.Parse(inputSentInfoWord[i].ToString());
                    }

                    sentInfoWord_validationMessage.Text = "";
                    sentInfoWord_resetedMessage.Text = "";
                    sentInfoWord_confirmedMessage.Text = "Введённое информационное слово: ";

                    for (int i = 0; i < sentInfoWord_manual.Length; i++)
                    {
                        sentInfoWord_confirmedMessage.Text += sentInfoWord_manual[i];
                    }

                    sentInfoWord_TextBox.Text = "";

                    sentInfoWord_TextBox.IsEnabled = false;
                    sentInfoWord_confirmButton.IsEnabled = false;
                    sentInfoWord_resetButton.IsEnabled = true;
                    showSteps.Visibility = Visibility.Visible;
                }
                else
                {
                    sentInfoWord_validationMessage.Text = $"Введите слово длиной {messageLength / 2}.";
                    sentInfoWord_resetedMessage.Text = "";
                    sentInfoWord_confirmedMessage.Text = "";

                    sentInfoWord_TextBox.Text = "";
                }
            }
        }

        private void ResetButton_sentInfoWord_click(object sender, RoutedEventArgs e)
        {
            sentInfoWord_manual = new int[messageLength / 2];

            sentInfoWord_confirmedMessage.Text = "";
            sentInfoWord_resetedMessage.Text = "Значение сброшено.";
            sentInfoWord_validationMessage.Text = "";

            sentInfoWord_TextBox.IsEnabled = true;
            sentInfoWord_confirmButton.IsEnabled = true;
            sentInfoWord_resetButton.IsEnabled = false;
            showSteps.Visibility = Visibility.Hidden;
        }

        private void ShowSteps_click(object sender, RoutedEventArgs e)
        {
            // Определение выбранного метода декодирования
            selectedDecodingMethod = decodingMethod_ComboBox.SelectedIndex;

            step0_StackPanel.Visibility = Visibility.Visible;
            help_step0.Visibility = Visibility.Visible;

            informationMessage_step0.Text = "";
            _BhattacharyaParameters_Text.Text = "";
            sortedIndices_Text.Text = "";
            definitionOfInfoAndFrozenBits.Text = "";
            BhattacharyaParameters_frozenBitsIndices_Text.Text = "";
            BhattacharyaParameters_infoBitsIndices_Text.Text = "";

            nextStepButton_step0.IsEnabled = true;

            manualInputMode_StackPanel.Visibility = Visibility.Hidden;

            manualModeIsEnabled = true;

            ManualMode();
        }

        private void ManualMode()
        {
            // Задание параметров моделирования
            int modulatingPointsArray = (messageLength + 1) / 3; // количество точек в модулированном сигнале (каждая точка состоит из трёх бит)
            //int numberOfCheckBits = 4; // количество проверочных бит
            double sig = 0.5; // дисперсия плотности распределения шума
            double errorProbability = 0.19; // вероятность ошибки

            // Расчёт параметров Бхаттачарья и получение индексов замороженных и информационных бит
            (double[] _BhattacharyaParameters, int[] sortedIndices, double[] valuesOfFrozenBits, double[] valuesOfInfoBits, int[] frozenBitsIndices, int[] infoBitsIndices) = BhattacharyaParameters.FindingBhattacharyaParameters(messageLength, errorProbability);

            // Задание массива для идентификации замороженных и информационных бит
            int[] identificationOfFrozenAndInfoBits = new int[messageLength];

            // Инициализация массива идентификации замороженных и информационных бит
            for (int i = 0; i < messageLength; i++)
            {
                // Поиск индексов замороженных и информационных бит и их идентификация
                for (int j = 0; j < messageLength / 2; j++)
                {
                    if (i == frozenBitsIndices[j])
                    {
                        identificationOfFrozenAndInfoBits[i] = 0; // замороженный бит
                    }
                }
                for (int j = 0; j < messageLength / 2; j++)
                {
                    if (i == infoBitsIndices[j])
                    {
                        identificationOfFrozenAndInfoBits[i] = 1; // информационный бит
                    }
                }
            }

            // Построение матрицы G
            int[,] G = Gmatrix.ConstructionOfGmatrix(messageLength);

            // Создание сигнального созвездия
            (Complex[] pointsArray, double[] R) = Constellation.ConstellationConstruction();

            // Описание прямых для областей Вороного
            (double[] a, double[] b) = StraightLines.GettingPointCoordinatesForStraightLines(pointsArray);

            int[] codedMessage = new int[messageLength];
            int[] precodedMessage = new int[messageLength];

            Dispatcher.Invoke(() =>
            {
                informationMessage_step0.Text = "";
                informationMessage_step0.Text = "Операции до начала кодирования";

                _BhattacharyaParameters_Text.Text = "";
                _BhattacharyaParameters_Text.Text = "Рассчитанные и отсортированные по возрастанию параметры Бхаттачарья:" + Environment.NewLine + Environment.NewLine;

                for (int k = 0; k < _BhattacharyaParameters.Length; k++)
                {
                    if (k < (_BhattacharyaParameters.Length - 1))
                    {
                        _BhattacharyaParameters_Text.Text += _BhattacharyaParameters[k] + " | ";
                    }
                    else
                    {
                        _BhattacharyaParameters_Text.Text += _BhattacharyaParameters[k];
                    }
                }

                sortedIndices_Text.Text = "";
                sortedIndices_Text.Text = "Индексы параметров Бхаттачарья после их сортировки по возрастанию:" + Environment.NewLine + Environment.NewLine;

                for (int k = 0; k < sortedIndices.Length; k++)
                {
                    sortedIndices_Text.Text += sortedIndices[k] + "  ";
                }

                definitionOfInfoAndFrozenBits.Text = "";

                if (messageLength == 32 || messageLength == 16)
                {
                    definitionOfInfoAndFrozenBits.Text = $"Первые {messageLength / 2} бит были определены, как информационные, вторые {messageLength / 2} бит - как замороженные:" + Environment.NewLine + Environment.NewLine;
                }
                else
                {
                    definitionOfInfoAndFrozenBits.Text = $"Первые {messageLength / 2} бита были определены, как информационные, вторые {messageLength / 2} бита - как замороженные:" + Environment.NewLine + Environment.NewLine;
                }

                for (int k = 0; k < sortedIndices.Length; k++)
                {
                    Run run = new Run(sortedIndices[k] + "  ");

                    if (k < sortedIndices.Length / 2)
                    {
                        run.Foreground = Brushes.Green;
                    }
                    else
                    {
                        run.Foreground = Brushes.Blue;
                    }

                    definitionOfInfoAndFrozenBits.Inlines.Add(run);
                }

                // Отображение индексов замороженных бит
                BhattacharyaParameters_frozenBitsIndices_Text.Text = "";
                BhattacharyaParameters_frozenBitsIndices_Text.Text = "Итоговые индексы замороженных бит (по возрастанию):" + Environment.NewLine;

                for (int k = 0; k < frozenBitsIndices.Length; k++)
                {
                    Run run = new Run(frozenBitsIndices[k] + "  ")
                    {
                        Foreground = Brushes.Blue
                    };

                    BhattacharyaParameters_frozenBitsIndices_Text.Inlines.Add(run);
                }

                // Отображение индексов информационных бит
                BhattacharyaParameters_infoBitsIndices_Text.Text = "";
                BhattacharyaParameters_infoBitsIndices_Text.Text = "Итоговые индексы информационных бит (по возрастанию):" + Environment.NewLine;

                for (int k = 0; k < infoBitsIndices.Length; k++)
                {
                    Run run = new Run(infoBitsIndices[k] + "  ")
                    {
                        Foreground = Brushes.Green
                    };

                    BhattacharyaParameters_infoBitsIndices_Text.Inlines.Add(run);
                }
            });

            // Получаем количество строк и столбцов в матрице G
            int rowsOfG = G.GetLength(0);
            int colsOfG = G.GetLength(1);

            Dispatcher.Invoke(() =>
            {
                GmatrixMessage.Text = "";
                GmatrixMessage.Text = $"Построение порождающей матрицы (произведение перестановочной матрицы на {Math.Log(messageLength, 2)} степень кронекерова произведения матрицы (1, 0; 1, 1):";
            });

            Dispatcher.Invoke(() =>
            {
                if (messageLength == 32)
                {
                    Gmatrix_Text.FontSize = 12;
                }
                else if (messageLength == 16)
                {
                    Gmatrix_Text.FontSize = 16;
                }
                else if (messageLength == 8)
                {
                    Gmatrix_Text.FontSize = 20;
                }
                else if (messageLength == 4)
                {
                    Gmatrix_Text.FontSize = 20;
                }

                Gmatrix_Text.Text = "";
            });

            // Перебираем элементы матрицы и добавляем их в Gmatrix_Text
            for (int k = 0; k < rowsOfG; k++)
            {
                for (int l = 0; l < colsOfG; l++)
                {
                    Dispatcher.Invoke(() =>
                    {
                        // Добавляем элемент матрицы в Gmatrix_Text с пробелом после каждого элемента
                        Gmatrix_Text.Text += G[k, l] + "  ";
                    });
                }

                Dispatcher.Invoke(() =>
                {
                    // Добавляем новую строку после каждой строки матрицы
                    Gmatrix_Text.Text += Environment.NewLine;
                });
            }

            Dispatcher.Invoke(() =>
            {
                informationMessage_step05.Text = "";
                informationMessage_step05.Text = "Операции до начала кодирования";
            });

            Dispatcher.Invoke(() =>
            {
                informationMessage_step1.Text = "";
                informationMessage_step1.Text = "Поэтапная визуализация преобразования заданного сообщения";

                sentInfoWord_Text.Text = "Исходное информационное сообщение:" + Environment.NewLine + Environment.NewLine;

                for (int k = 0; k < sentInfoWord_manual.Length; k++)
                {
                    sentInfoWord_Text.Text += sentInfoWord_manual[k] + "  ";
                }
            });

            // Кодирование сообщения
            (codedMessage, precodedMessage) = PolarEncoder.PolarMessageEncoding(messageLength, sentInfoWord_manual, frozenBitsIndices, infoBitsIndices, G);

            Dispatcher.Invoke(() =>
            {
                encodingStageMessage.Text = "";
                encodingStageMessage.Text = "Этапы кодирования:";

                precodedMessage_Text.Text = "";
                precodedMessage_Text.Text = "----------" + Environment.NewLine;
                precodedMessage_Text.Text += "1) Распределение замороженных и информационных бит в сообщении в соответствии с их индексами:" + Environment.NewLine + Environment.NewLine;

                for (int k = 0; k < precodedMessage.Length; k++)
                {
                    Run run = new Run(precodedMessage[k] + "  ");

                    for (int l = 0; l < frozenBitsIndices.Length; l++)
                    {
                        if (k == frozenBitsIndices[l])
                        {
                            run.Foreground = Brushes.Blue;
                        }
                    }

                    for (int l = 0; l < infoBitsIndices.Length; l++)
                    {
                        if (k == infoBitsIndices[l])
                        {
                            run.Foreground = Brushes.Green;
                        }
                    }

                    precodedMessage_Text.Inlines.Add(run);
                }

                Span span = new Span();

                Run run1 = new Run("Примечание: ");
                span.Inlines.Add(run1);

                Run run2 = new Run("синим");
                run2.Foreground = Brushes.Blue;
                span.Inlines.Add(run2);

                Run run3 = new Run(" показаны замороженные биты, ");
                span.Inlines.Add(run3);

                Run run4 = new Run("зелёным");
                run4.Foreground = Brushes.Green;
                span.Inlines.Add(run4);

                Run run5 = new Run(" - информационные.");
                span.Inlines.Add(run5);

                encoding_informationMessage.Text = "";
                encoding_informationMessage.Inlines.Add(span);

                codedMessage_Text.Text = "";
                codedMessage_Text.Text = "----------" + Environment.NewLine;
                codedMessage_Text.Text += "2) Умножение сообщения на порождающую матрицу G в соответствии с правилами матричного умножения и взятие остатка от деления на 2 (поэлементно):" + Environment.NewLine + Environment.NewLine;

                for (int k = 0; k < codedMessage.Length; k++)
                {
                    Run run = new Run(codedMessage[k] + "  ");

                    for (int l = 0; l < frozenBitsIndices.Length; l++)
                    {
                        if (k == frozenBitsIndices[l])
                        {
                            run.Foreground = Brushes.Blue;
                        }
                    }

                    for (int l = 0; l < infoBitsIndices.Length; l++)
                    {
                        if (k == infoBitsIndices[l])
                        {
                            run.Foreground = Brushes.Green;
                        }
                    }

                    codedMessage_Text.Inlines.Add(run);
                }

                encoding_finalInformationMessage.Text = "";
                encoding_finalInformationMessage.Text = "Полученное сообщение является кодовым вектором." + Environment.NewLine;
                encoding_finalInformationMessage.Text += "Кодирование сообщения завершено.";
            });

            // Модуляция сообщения
            Complex[] modulatedMessage = Modulator.MessageModulating(messageLength, modulatingPointsArray, pointsArray, codedMessage);

            Dispatcher.Invoke(() =>
            {
                informationMessage_step2.Text = "";
                informationMessage_step2.Text = "Поэтапная визуализация преобразования заданного сообщения";

                modulatingStageMessage.Text = "";
                modulatingStageMessage.Text = "Этапы модуляции:";

                firstStep_modulatingStageMessage.Text = "";
                firstStep_modulatingStageMessage.Text = "----------" + Environment.NewLine;
                firstStep_modulatingStageMessage.Text += "1) Длина модулированного сигнала определяется, как длина кода + 1 / 3. Кодовый вектор разбивается на группы по три бита:" + Environment.NewLine + Environment.NewLine;

                for (int k = 0; k < codedMessage.Length; k++)
                {
                    if ((k + 1) % 3 == 0)
                    {
                        firstStep_modulatingStageMessage.Text += codedMessage[k] + " | ";
                    }
                    else
                    {
                        firstStep_modulatingStageMessage.Text += codedMessage[k] + " ";
                    }

                    if (k == codedMessage.Length - 1)
                    {
                        firstStep_modulatingStageMessage.Text += "0"; // дополнительный бит, добавляемый к концу кодового вектора при модуляции
                    }
                }

                secondStep_modulatingStageMessage.Text = "";
                secondStep_modulatingStageMessage.Text = "----------" + Environment.NewLine;
                secondStep_modulatingStageMessage.Text += "2) В качестве метода модуляции выбирается восьмипозиционная фазовая модуляция (8-PSK) с заданным сигнальным созвездием:";

                CreateConstellationPlot();
            });

            Dispatcher.Invoke(() =>
            {
                informationMessage_step3.Text = "";
                informationMessage_step3.Text = "Поэтапная визуализация преобразования заданного сообщения";

                thirdStep_modulatingStageMessage.Text = "";
                thirdStep_modulatingStageMessage.Text = "----------" + Environment.NewLine;
                thirdStep_modulatingStageMessage.Text += "3) Координаты точек представляются в комплексном виде:";

                complexPointsArray.Text = "";

                Complex pointsArray0Complex = pointsArray[0];
                string formattedPointsArray0Complex = $"{pointsArray0Complex.Real} + {pointsArray0Complex.Imaginary}i";
                complexPointsArray.Text += "Точка 100: " + formattedPointsArray0Complex + Environment.NewLine;

                Complex pointsArray1Complex = pointsArray[1];
                string formattedPointsArray1Complex = $"{pointsArray1Complex.Real} + {pointsArray1Complex.Imaginary}i";
                complexPointsArray.Text += "Точка 001: " + formattedPointsArray1Complex + Environment.NewLine;

                Complex pointsArray2Complex = pointsArray[2];
                string formattedPointsArray2Complex = $"{pointsArray2Complex.Real} + {pointsArray2Complex.Imaginary}i";
                complexPointsArray.Text += "Точка 000: " + formattedPointsArray2Complex + Environment.NewLine;

                Complex pointsArray3Complex = pointsArray[3];
                string formattedPointsArray3Complex = $"{pointsArray3Complex.Real} + {pointsArray3Complex.Imaginary}i";
                complexPointsArray.Text += "Точка 101: " + formattedPointsArray3Complex + Environment.NewLine;

                Complex pointsArray4Complex = pointsArray[4];
                string formattedPointsArray4Complex = $"{pointsArray4Complex.Real} + {pointsArray4Complex.Imaginary}i";
                complexPointsArray.Text += "Точка 010: " + formattedPointsArray4Complex + Environment.NewLine;

                Complex pointsArray5Complex = pointsArray[5];
                string formattedPointsArray5Complex = $"{pointsArray5Complex.Real} + {pointsArray5Complex.Imaginary}i";
                complexPointsArray.Text += "Точка 110: " + formattedPointsArray5Complex + Environment.NewLine;

                Complex pointsArray6Complex = pointsArray[6];
                string formattedPointsArray6Complex = $"{pointsArray6Complex.Real} + {pointsArray6Complex.Imaginary}i";
                complexPointsArray.Text += "Точка 011: " + formattedPointsArray6Complex + Environment.NewLine;

                Complex pointsArray7Complex = pointsArray[7];
                string formattedPointsArray7Complex = $"{pointsArray7Complex.Real} + {pointsArray7Complex.Imaginary}i";
                complexPointsArray.Text += "Точка 111: " + formattedPointsArray7Complex;

                fourthStep_modulatingStageMessage.Text = "";
                fourthStep_modulatingStageMessage.Text = "----------" + Environment.NewLine;
                fourthStep_modulatingStageMessage.Text += "4) Каждому значению модулированного сигнала присваивается значение одной из восьми точек созвездия:";

                modulatedMessage_Text.Text = "";

                for (int k = 0; k < modulatedMessage.Length; k++)
                {
                    // Получить текущее комплексное число
                    Complex currentComplexNumber = modulatedMessage[k];

                    // Форматировать комплексное число как строку
                    string formattedComplexNumber = $"{currentComplexNumber.Real} + {currentComplexNumber.Imaginary}i";

                    // Добавить отформатированное комплексное число к тексту
                    modulatedMessage_Text.Text += formattedComplexNumber;

                    // Добавить пробел между числами, если это не последнее число
                    if (k < modulatedMessage.Length - 1)
                    {
                        modulatedMessage_Text.Text += " | ";
                    }
                }

                modulating_finalInformationMessage.Text = "";
                modulating_finalInformationMessage.Text = "Полученное сообщение является модулированным сигналом." + Environment.NewLine;
                modulating_finalInformationMessage.Text += "Модуляция сообщения завершена.";
            });

            // Демодуляция сообщения
            int[] demodulatedMessage = Demodulator.MessageDemodulating(messageLength, sig, pointsArray, a, b, modulatedMessage);

            // Приведение демодулированного сообщения к верной длине
            int[] rightDemodulatedMessage = new int[messageLength];
            for (int k = 0; k < messageLength; k++)
            {
                rightDemodulatedMessage[k] = demodulatedMessage[k];
            }

            Dispatcher.Invoke(() =>
            {
                informationMessage_step5.Text = "";
                informationMessage_step5.Text = "Поэтапная визуализация преобразования заданного сообщения";

                demodulatingStageMessage.Text = "";
                demodulatingStageMessage.Text = "Этапы демодуляции:";

                firstStep_demodulatingStageMessage.Text = "";
                firstStep_demodulatingStageMessage.Text = "----------" + Environment.NewLine;
                firstStep_demodulatingStageMessage.Text += "1) В качестве метода демодуляции выбирается метод построения областей Вороного.";

                secondStep_demodulatingStageMessage.Text = "";
                secondStep_demodulatingStageMessage.Text = "----------" + Environment.NewLine;
                secondStep_demodulatingStageMessage.Text += "2) Расчёт коэффициентов прямых a и b, разделяющих две соседние точки:";

                a_Text.Text = "";
                a_Text.Text = "Прямая a: ";

                for (int k = 0; k < a.Length; k++)
                {
                    if (k < a.Length - 1)
                    {
                        a_Text.Text += a[k] + " | ";
                    }
                    else
                    {
                        a_Text.Text += a[k];
                    }
                }

                b_Text.Text = "";
                b_Text.Text = "Прямая b: ";

                for (int k = 0; k < b.Length; k++)
                {
                    if (k < b.Length - 1)
                    {
                        b_Text.Text += b[k] + " | ";
                    }
                    else
                    {
                        b_Text.Text += b[k];
                    }
                }
            });

            Dispatcher.Invoke(() =>
            {
                informationMessage_step6.Text = "";
                informationMessage_step6.Text = "Поэтапная визуализация преобразования заданного сообщения";

                thirdStep_demodulatingStageMessage.Text = "";
                thirdStep_demodulatingStageMessage.Text = "----------" + Environment.NewLine;
                thirdStep_demodulatingStageMessage.Text += "3) Вычисление мягких метрик для каждой группы из трёх битов (для каждого значения: 0 и 1) и выяснение побитовой принадлежности принятой точки одной из областей (более подробно в справке):";

                rightDemodulatedMessage_Text.Text = "";

                for (int k = 0; k < rightDemodulatedMessage.Length; k++)
                {
                    rightDemodulatedMessage_Text.Text += rightDemodulatedMessage[k] + "  ";
                }

                demodulatingStageMessage_preFinalInformationMessage.Text = "";
                demodulatingStageMessage_preFinalInformationMessage.Text += "Полученное сообщение является демодулированным сигналом.";

                comparisonOfCodedMessageAndRightDemodulatedMessage.Text = "";
                comparisonOfCodedMessageAndRightDemodulatedMessage.Text = "----------" + Environment.NewLine;
                comparisonOfCodedMessageAndRightDemodulatedMessage.Text += "Сравнение кодового слова и демодулированного сигнала:";

                codedMessage_Text_forComparison.Text = "";

                for (int k = 0; k < codedMessage.Length; k++)
                {
                    Run run = new Run(codedMessage[k] + "  ");

                    for (int l = 0; l < frozenBitsIndices.Length; l++)
                    {
                        if (k == frozenBitsIndices[l])
                        {
                            run.Foreground = Brushes.Blue;
                        }
                    }

                    for (int l = 0; l < infoBitsIndices.Length; l++)
                    {
                        if (k == infoBitsIndices[l])
                        {
                            run.Foreground = Brushes.Green;
                        }
                    }

                    codedMessage_Text_forComparison.Inlines.Add(run);
                }

                rightDemodulatedMessage_Text_forComparison.Text = "";

                for (int k = 0; k < rightDemodulatedMessage.Length; k++)
                {
                    Run run = new Run(rightDemodulatedMessage[k] + "  ");

                    for (int l = 0; l < frozenBitsIndices.Length; l++)
                    {
                        if (k == frozenBitsIndices[l])
                        {
                            run.Foreground = Brushes.Blue;
                        }
                    }

                    for (int l = 0; l < infoBitsIndices.Length; l++)
                    {
                        if (k == infoBitsIndices[l])
                        {
                            run.Foreground = Brushes.Green;
                        }
                    }

                    rightDemodulatedMessage_Text_forComparison.Inlines.Add(run);
                }

                resultOfComparisonCodedMessageAndRightDemodulatedMessage.Text = "";

                if (codedMessage.SequenceEqual(rightDemodulatedMessage))
                {
                    resultOfComparisonCodedMessageAndRightDemodulatedMessage.Text = "Демодуляция сообщения завершена успешно.";
                }
                else
                {
                    resultOfComparisonCodedMessageAndRightDemodulatedMessage.Text = "Демодуляция сообщения завершена с ошибкой (ошибками).";
                }
            });

            // Декодирование сообщения
            int[] decodedMessage = new int[messageLength];
            if (selectedDecodingMethod == 0)
            {
                decodedMessage = DirectDecoder.MessageDecoding(messageLength, errorProbability, identificationOfFrozenAndInfoBits, G, rightDemodulatedMessage);
            }
            else if (selectedDecodingMethod == 1)
            {
                decodedMessage = RecursiveDecoder.MessageDecoding(messageLength, errorProbability, identificationOfFrozenAndInfoBits, precodedMessage, rightDemodulatedMessage);
            }

            Dispatcher.Invoke(() =>
            {
                informationMessage_step7.Text = "";
                informationMessage_step7.Text = "Поэтапная визуализация преобразования заданного сообщения";

                decodingStageMessage.Text = "";
                decodingStageMessage.Text = "Этапы декодирования:";

                if (selectedDecodingMethod == 0)
                {
                    firstStep_decodingStageMessage.Text = "";
                    firstStep_decodingStageMessage.Text = "----------" + Environment.NewLine;
                    firstStep_decodingStageMessage.Text += "1) В качестве метода декодирования был выбран прямой метод.";
                }
                else if (selectedDecodingMethod == 1)
                {
                    firstStep_decodingStageMessage.Text = "";
                    firstStep_decodingStageMessage.Text = "----------" + Environment.NewLine;
                    firstStep_decodingStageMessage.Text += "1) В качестве метода декодирования был выбран рекурсивный метод.";
                }

                secondStep_decodingStageMessage.Text = "";
                secondStep_decodingStageMessage.Text = "----------" + Environment.NewLine;
                secondStep_decodingStageMessage.Text += "2) По принятым жёстким решениям вычисляется вектор, который является оценкой вектора кодового слова, то есть находится предкодовый вектор:";

                decodedMessage_Text.Text = "";

                for (int k = 0; k < decodedMessage.Length; k++)
                {
                    Run run = new Run(decodedMessage[k] + "  ");

                    for (int l = 0; l < frozenBitsIndices.Length; l++)
                    {
                        if (k == frozenBitsIndices[l])
                        {
                            run.Foreground = Brushes.Blue;
                        }
                    }

                    for (int l = 0; l < infoBitsIndices.Length; l++)
                    {
                        if (k == infoBitsIndices[l])
                        {
                            run.Foreground = Brushes.Green;
                        }
                    }

                    decodedMessage_Text.Inlines.Add(run);
                }
            });

            // Выделение информационного слова
            int[] receivedInfoWord = new int[messageLength / 2];
            for (int k = 0; k < receivedInfoWord.Length; k++)
            {
                receivedInfoWord[k] = decodedMessage[infoBitsIndices[k]];
            }

            Dispatcher.Invoke(() =>
            {
                thirdStep_decodingStageMessage.Text = "";
                thirdStep_decodingStageMessage.Text = "----------" + Environment.NewLine;
                thirdStep_decodingStageMessage.Text += "3) По индексам информационных бит из предкодового вектора выделяется декодированное сообщение:";

                receivedInfoWord_Text.Text = "";

                for (int k = 0; k < receivedInfoWord.Length; k++)
                {
                    Run run = new Run(receivedInfoWord[k] + "  ");

                    for (int l = 0; l < infoBitsIndices.Length; l++)
                    {
                        run.Foreground = Brushes.Green;
                    }

                    receivedInfoWord_Text.Inlines.Add(run);
                }

                comparisonOfSentInfoWordAndReceivedInfoWord.Text = "";
                comparisonOfSentInfoWordAndReceivedInfoWord.Text = "----------" + Environment.NewLine;
                comparisonOfSentInfoWordAndReceivedInfoWord.Text += "Сравнение информационного слова и декодированного сообщения:";

                sentInfoWord_Text_forComparison.Text = "";

                for (int k = 0; k < sentInfoWord_manual.Length; k++)
                {
                    sentInfoWord_Text_forComparison.Text += sentInfoWord_manual[k] + "  ";
                }

                receivedInfoWord_Text_forComparison.Text = "";

                for (int k = 0; k < receivedInfoWord.Length; k++)
                {
                    receivedInfoWord_Text_forComparison.Text += receivedInfoWord[k] + "  ";
                }

                resultOfComparisonSentInfoWordAndReceivedInfoWord.Text = "";

                if (sentInfoWord_manual.SequenceEqual(receivedInfoWord))
                {
                    resultOfComparisonSentInfoWordAndReceivedInfoWord.Text = "Декодирование сообщения завершено успешно.";
                }
                else
                {
                    resultOfComparisonSentInfoWordAndReceivedInfoWord.Text = "Декодирование сообщения завершено с ошибкой (ошибками).";
                }
            });
        }

        private void StartModeling_click(object sender, RoutedEventArgs e)
        {
            modelRunNumber.Text = ""; // Очистка содержимого TextBox

            stopModeling.IsEnabled = true;
            previousStepButton_screen3.IsEnabled = false;

            // Присвоение введённой пользователем левой границы первому элементу массива шума (по нулевому индексу)
            noise[0] = tempFirstValueOfNoise;

            // Установка максимального значения для ProgressBar
            numberOfRunsProgressbar.Maximum = numberOfRuns;

            if (nextStepButton_step0.Visibility == Visibility.Visible)
            {
                step0_StackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                step0_StackPanel.Visibility = Visibility.Hidden;
            }

            if (!manualModeIsEnabled)
            {
                informationMessage_step0.Text = "";
                _BhattacharyaParameters_Text.Text = "";
                sortedIndices_Text.Text = "";
                definitionOfInfoAndFrozenBits.Text = "";
                BhattacharyaParameters_frozenBitsIndices_Text.Text = "";
                BhattacharyaParameters_infoBitsIndices_Text.Text = "";

                showStepsInAutomaticMode_if_1 = true;
                showStepsInAutomaticMode_if_1_2 = true;
                showStepsInAutomaticMode_if_2 = true;
                showStepsInAutomaticMode_if_3 = true;
                showStepsInAutomaticMode_if_4 = true;
                showStepsInAutomaticMode_if_5 = true;
                showStepsInAutomaticMode_if_6 = true;
                showStepsInAutomaticMode_if_7 = true;
                showStepsInAutomaticMode_if_8 = true;
                showStepsInAutomaticMode_if_9 = true;
                showStepsInAutomaticMode_if_10 = true;
            }

            startModeling.IsEnabled = false;
            nextStepButton_step0.IsEnabled = true;

            // Инициализация BackgroundWorker
            backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true
            };

            backgroundWorker.DoWork += Main;
            backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

            // Запуск фонового процесса
            backgroundWorker.RunWorkerAsync();
        }

        private void StopModeling_click(object sender, RoutedEventArgs e)
        {
            resetModelingSignal = true;
        }

        private void ResetModelingResults_click(object sender, RoutedEventArgs e)
        {
            modelRunNumber.Text = ""; // Очистка содержимого TextBox
            modelRunNumber.AppendText("Программа готова к моделированию." + Environment.NewLine);
            modelRunNumber.AppendText("Нажмите «Начать моделирование»..." + Environment.NewLine);
            modelRunNumber.ScrollToEnd();

            if (!manualModeIsEnabled)
            {
                step0_StackPanel.Visibility = Visibility.Hidden;
                nextStepButton_step0.Visibility = Visibility.Visible;
                nextStepButton_step0.IsEnabled = false;

                step05_StackPanel.Visibility = Visibility.Hidden;
                nextStepButton_step05.Visibility = Visibility.Hidden;
                previousStepButton_step05.Visibility = Visibility.Hidden;

                step1_StackPanel.Visibility = Visibility.Hidden;
                nextStepButton_step1.Visibility = Visibility.Hidden;
                previousStepButton_step1.Visibility = Visibility.Hidden;

                step2_StackPanel.Visibility = Visibility.Hidden;
                nextStepButton_step2.Visibility = Visibility.Hidden;
                previousStepButton_step2.Visibility = Visibility.Hidden;

                step3_StackPanel.Visibility = Visibility.Hidden;
                nextStepButton_step3.Visibility = Visibility.Hidden;
                previousStepButton_step3.Visibility = Visibility.Hidden;

                step4_StackPanel.Visibility = Visibility.Hidden;
                nextStepButton_step4.Visibility = Visibility.Hidden;
                previousStepButton_step4.Visibility = Visibility.Hidden;

                step5_StackPanel.Visibility = Visibility.Hidden;
                nextStepButton_step5.Visibility = Visibility.Hidden;
                previousStepButton_step5.Visibility = Visibility.Hidden;

                step6_StackPanel.Visibility = Visibility.Hidden;
                nextStepButton_step6.Visibility = Visibility.Hidden;
                previousStepButton_step6.Visibility = Visibility.Hidden;

                step7_StackPanel.Visibility = Visibility.Hidden;
                previousStepButton_step7.Visibility = Visibility.Hidden;

                manualInputMode.Visibility = Visibility.Visible;

                help_step0.Visibility = Visibility.Visible;
                help_step05.Visibility = Visibility.Hidden;
                help_step1.Visibility = Visibility.Hidden;
                help_step2.Visibility = Visibility.Hidden;
                help_step3.Visibility = Visibility.Hidden;
                help_step4.Visibility = Visibility.Hidden;
                help_step5.Visibility = Visibility.Hidden;
                help_step6.Visibility = Visibility.Hidden;
                help_step7.Visibility = Visibility.Hidden;
            }

            numberOfRunsProgressbar.Value = 0;
            startModeling.IsEnabled = true;
            resetModelingResults.IsEnabled = false;
            showBitErrorPlot.IsEnabled = false;
            showVoronoiDiagrams.IsEnabled = false;
        }

        private BitErrorPlot BitErrorPlot_graphWindow;

        private void ShowBitErrorPlot_click(object sender, RoutedEventArgs e)
        {
            // Создание PlotModel и настройка осей и данных
            PlotModel plotModel = new PlotModel
            {
                DefaultFontSize = 18
            };
            LinearAxis xAxis = new LinearAxis { Position = AxisPosition.Bottom, Title = "Отношение сигнал/шум" };
            LogarithmicAxis yAxis = new LogarithmicAxis { Position = AxisPosition.Left, Title = "Вероятность битовой ошибки (BER)" };

            xAxis.TitleFontSize = 18;
            yAxis.TitleFontSize = 18;

            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            LineSeries decodingErrorSeries = new LineSeries { Title = "Битовая ошибка демодуляции и декодирования" };
            LineSeries demodulationErrorSeries = new LineSeries { Title = "Битовая ошибка демодуляции" };

            for (int i = 0; i < noise.Length; i++)
            {
                decodingErrorSeries.Points.Add(new DataPoint(noise[i], decodingError[i]));
                demodulationErrorSeries.Points.Add(new DataPoint(noise[i], demodulationError[i]));
            }

            plotModel.Series.Add(decodingErrorSeries);
            plotModel.Series.Add(demodulationErrorSeries);

            CloseIfOpen(BitErrorPlot_graphWindow);

            // Создание и отображение нового окна с графиком
            BitErrorPlot_graphWindow = new BitErrorPlot();
            childWindows.Add(BitErrorPlot_graphWindow);
            BitErrorPlot_graphWindow.SetPlotModel(plotModel);
            BitErrorPlot_graphWindow.Show();
        }

        private VoronoiDiagram_10 VoronoiDiagram_10_graphWindow;
        private VoronoiDiagram_11 VoronoiDiagram_11_graphWindow;
        private VoronoiDiagram_20 VoronoiDiagram_20_graphWindow;
        private VoronoiDiagram_21 VoronoiDiagram_21_graphWindow;
        private VoronoiDiagram_30 VoronoiDiagram_30_graphWindow;
        private VoronoiDiagram_31 VoronoiDiagram_31_graphWindow;

        private void ShowVoronoiDiagrams_click(object sender, RoutedEventArgs e)
        {
            CloseIfOpen(VoronoiDiagram_10_graphWindow);
            CloseIfOpen(VoronoiDiagram_11_graphWindow);
            CloseIfOpen(VoronoiDiagram_20_graphWindow);
            CloseIfOpen(VoronoiDiagram_21_graphWindow);
            CloseIfOpen(VoronoiDiagram_30_graphWindow);
            CloseIfOpen(VoronoiDiagram_31_graphWindow);

            VoronoiDiagram_10_graphWindow = new VoronoiDiagram_10();
            VoronoiDiagram_11_graphWindow = new VoronoiDiagram_11();
            VoronoiDiagram_20_graphWindow = new VoronoiDiagram_20();
            VoronoiDiagram_21_graphWindow = new VoronoiDiagram_21();
            VoronoiDiagram_30_graphWindow = new VoronoiDiagram_30();
            VoronoiDiagram_31_graphWindow = new VoronoiDiagram_31();

            childWindows.Add(VoronoiDiagram_10_graphWindow);
            childWindows.Add(VoronoiDiagram_11_graphWindow);
            childWindows.Add(VoronoiDiagram_20_graphWindow);
            childWindows.Add(VoronoiDiagram_21_graphWindow);
            childWindows.Add(VoronoiDiagram_30_graphWindow);
            childWindows.Add(VoronoiDiagram_31_graphWindow);

            VoronoiDiagram_10_graphWindow.Show();
            VoronoiDiagram_11_graphWindow.Show();
            VoronoiDiagram_20_graphWindow.Show();
            VoronoiDiagram_21_graphWindow.Show();
            VoronoiDiagram_30_graphWindow.Show();
            VoronoiDiagram_31_graphWindow.Show();
        }

        private void CloseIfOpen(Window window)
        {
            if (window != null && window.IsVisible)
            {
                window.Close();
            }
        }

        /// <summary>
        /// Кнопка перехода с экрана #3 на экран #2 (задание параметров моделирования)
        /// </summary>
        private void PreviousStepButton_screen3_click(object sender, RoutedEventArgs e)
        {
            settingModelingParameters_Text.Visibility = Visibility.Visible;
            codingMethod_StackPanel.Visibility = Visibility.Visible;
            modulationMethod_StackPanel.Visibility = Visibility.Visible;
            noiseExposureMethod_StackPanel.Visibility = Visibility.Visible;
            demodulationMethod_StackPanel.Visibility = Visibility.Visible;
            decodingMethod_StackPanel.Visibility = Visibility.Visible;
            numberOfRuns_StackPanel.Visibility = Visibility.Visible;
            numberOfPackets_StackPanel.Visibility = Visibility.Visible;
            messageLength_StackPanel.Visibility = Visibility.Visible;
            firstValueOfNoise_StackPanel.Visibility = Visibility.Visible;
            noiseStep_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_screen2.Visibility = Visibility.Visible;
            previousStepButton_screen2.Visibility = Visibility.Visible;

            // Скрытие элементов экрана #3
            leftSide_StackPanel.Visibility = Visibility.Hidden;
            previousStepButton_screen3.Visibility = Visibility.Hidden;

            step0_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step0.Visibility = Visibility.Hidden;

            step05_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step05.Visibility = Visibility.Hidden;
            previousStepButton_step05.Visibility = Visibility.Hidden;

            step1_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step1.Visibility = Visibility.Hidden;
            previousStepButton_step1.Visibility = Visibility.Hidden;

            step2_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step2.Visibility = Visibility.Hidden;
            previousStepButton_step2.Visibility = Visibility.Hidden;

            step3_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step3.Visibility = Visibility.Hidden;
            previousStepButton_step3.Visibility = Visibility.Hidden;

            step4_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step4.Visibility = Visibility.Hidden;
            previousStepButton_step4.Visibility = Visibility.Hidden;

            step5_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step5.Visibility = Visibility.Hidden;
            previousStepButton_step5.Visibility = Visibility.Hidden;

            step6_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step6.Visibility = Visibility.Hidden;
            previousStepButton_step6.Visibility = Visibility.Hidden;

            step7_StackPanel.Visibility = Visibility.Hidden;
            previousStepButton_step7.Visibility = Visibility.Hidden;

            manualInputMode.Visibility = Visibility.Hidden;
            automaticMode.Visibility = Visibility.Hidden;
            manualInputMode_StackPanel.Visibility = Visibility.Hidden;

            help_step0.Visibility = Visibility.Hidden;
            help_step05.Visibility = Visibility.Hidden;
            help_step1.Visibility = Visibility.Hidden;
            help_step2.Visibility = Visibility.Hidden;
            help_step3.Visibility = Visibility.Hidden;
            help_step4.Visibility = Visibility.Hidden;
            help_step5.Visibility = Visibility.Hidden;
            help_step6.Visibility = Visibility.Hidden;
            help_step7.Visibility = Visibility.Hidden;
        }

        private void NextStepButton_step0_click(object sender, RoutedEventArgs e)
        {
            step0_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step0.Visibility = Visibility.Hidden;
            manualInputMode.Visibility = Visibility.Hidden;
            automaticMode.Visibility = Visibility.Hidden;

            step05_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_step05.Visibility = Visibility.Visible;
            previousStepButton_step05.Visibility = Visibility.Visible;

            help_step0.Visibility = Visibility.Hidden;
            help_step05.Visibility = Visibility.Visible;
        }

        private readonly string directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private readonly string resourcesFolder = "Resources";

        private void Help_step0_click(object sender, RoutedEventArgs e)
        {
            string help_0 = "Справка по параметрам Бхаттачарья и инф и заморож битам.pdf";

            string resourcesPath = System.IO.Path.Combine(directory, resourcesFolder);

            string help_0_Path = System.IO.Path.Combine(resourcesPath, help_0);

            if (File.Exists(help_0_Path))
            {
                Process.Start(help_0_Path);
            }
            else
            {
                MessageBox.Show("Файл «Справка по параметрам Бхаттачарья, информационным и замороженным битам» не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousStepButton_step05_click(object sender, RoutedEventArgs e)
        {
            step0_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_step0.Visibility = Visibility.Visible;

            if (!manualModeIsEnabled)
            {
                manualInputMode.Visibility = Visibility.Visible;
            }
            else
            {
                manualInputMode.Visibility = Visibility.Hidden;
                automaticMode.Visibility = Visibility.Visible;
            }

            step05_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step05.Visibility = Visibility.Hidden;
            previousStepButton_step05.Visibility = Visibility.Hidden;

            help_step0.Visibility = Visibility.Visible;
            help_step05.Visibility = Visibility.Hidden;
        }

        private void NextStepButton_step05_click(object sender, RoutedEventArgs e)
        {
            step1_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_step1.Visibility = Visibility.Visible;
            previousStepButton_step1.Visibility = Visibility.Visible;

            step05_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step05.Visibility = Visibility.Hidden;
            previousStepButton_step05.Visibility = Visibility.Hidden;

            help_step05.Visibility = Visibility.Hidden;
            help_step1.Visibility = Visibility.Visible;
        }

        private void Help_step05_click(object sender, RoutedEventArgs e)
        {
            string help_05 = "Справка по порождающей матрице.pdf";

            string resourcesPath = System.IO.Path.Combine(directory, resourcesFolder);

            string help_05_Path = System.IO.Path.Combine(resourcesPath, help_05);

            if (File.Exists(help_05_Path))
            {
                Process.Start(help_05_Path);
            }
            else
            {
                MessageBox.Show("Файл «Справка по порождающей матрице» не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousStepButton_step1_click(object sender, RoutedEventArgs e)
        {
            step05_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_step05.Visibility = Visibility.Visible;
            previousStepButton_step05.Visibility = Visibility.Visible;

            step1_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step1.Visibility = Visibility.Hidden;
            previousStepButton_step1.Visibility = Visibility.Hidden;

            help_step05.Visibility = Visibility.Visible;
            help_step1.Visibility = Visibility.Hidden;
        }

        private void NextStepButton_step1_click(object sender, RoutedEventArgs e)
        {
            step1_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step1.Visibility = Visibility.Hidden;
            previousStepButton_step1.Visibility = Visibility.Hidden;

            previousStepButton_step2.Visibility = Visibility.Visible;
            nextStepButton_step2.Visibility = Visibility.Visible;
            step2_StackPanel.Visibility = Visibility.Visible;

            help_step1.Visibility = Visibility.Hidden;
            help_step2.Visibility = Visibility.Visible;
        }

        private void Help_step1_click(object sender, RoutedEventArgs e)
        {
            string help_1 = "Справка по кодированию.pdf";

            string resourcesPath = System.IO.Path.Combine(directory, resourcesFolder);

            string help_1_Path = System.IO.Path.Combine(resourcesPath, help_1);

            if (File.Exists(help_1_Path))
            {
                Process.Start(help_1_Path);
            }
            else
            {
                MessageBox.Show("Файл «Справка по кодированию» не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousStepButton_step2_click(object sender, RoutedEventArgs e)
        {
            step1_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_step1.Visibility = Visibility.Visible;
            previousStepButton_step1.Visibility = Visibility.Visible;

            previousStepButton_step2.Visibility = Visibility.Hidden;
            nextStepButton_step2.Visibility = Visibility.Hidden;
            step2_StackPanel.Visibility = Visibility.Hidden;

            help_step1.Visibility = Visibility.Visible;
            help_step2.Visibility = Visibility.Hidden;
        }

        private void CreateConstellationPlot()
        {
            var model = new PlotModel
            {
                DefaultFontSize = 20
            };

            // Оси X и Y
            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = -2.5, // Установка минимального значения
                Maximum = 2.5    // Установка максимального значения
            };

            // Отключение изменения масштаба и перемещения осей
            xAxis.IsZoomEnabled = false;
            xAxis.IsPanEnabled = false;

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = -2.5, // Установка минимального значения
                Maximum = 2.5    // Установка максимального значения
            };

            // Отключение изменения масштаба и перемещения осей
            yAxis.IsZoomEnabled = false;
            yAxis.IsPanEnabled = false;

            model.Axes.Add(xAxis);
            model.Axes.Add(yAxis);

            // Точки и радиусы окружностей
            (Complex[] pointsArray, double[] R) = Constellation.ConstellationConstruction();

            var pointsSeries = new ScatterSeries
            {
                Title = "Точки сигнального созвездия",
                MarkerType = MarkerType.Circle,
                MarkerSize = 5,
                MarkerFill = OxyColors.Blue // Цвет точек
            };

            for (int i = 0; i < pointsArray.Length; i++)
            {
                pointsSeries.Points.Add(new ScatterPoint(pointsArray[i].Real, pointsArray[i].Imaginary));

                // Вычисление смещений в зависимости от значения i
                double xOffset = 0, yOffset = 0;

                if (i == 0b111) // Для точки 111
                {
                    xOffset = -0.45;
                    yOffset = -0.3;
                }
                else if (i == 0b101) // Для точки 101
                {
                    xOffset = 0.45;
                    yOffset = -0.3;
                }
                else if (i == 0b100) // Для точки 100
                {
                    xOffset = 0.45;
                    yOffset = 0.1;
                }
                else if (i == 0b110) // Для точки 110
                {
                    xOffset = -0.45;
                    yOffset = 0.1;
                }
                else if (i == 0b001) // Для точки 001
                {
                    xOffset = -0.52;
                    yOffset = 0.05;
                }
                else if (i == 0b000) // Для точки 000
                {
                    xOffset = 0.52;
                    yOffset = 0.05;
                }
                else if (i == 0b010) // Для точки 010
                {
                    xOffset = 0.5;
                    yOffset = 0.1;
                }
                else if (i == 0b011) // Для точки 011
                {
                    xOffset = 0.5;
                    yOffset = -0.3;
                }

                // Добавление подписи к точке с учетом смещений
                var textAnnotation = new TextAnnotation
                {
                    TextPosition = new DataPoint(pointsArray[i].Real + xOffset, pointsArray[i].Imaginary + yOffset),
                    Text = $"Точка {Convert.ToString(i, 2).PadLeft(3, '0')}",
                    Stroke = OxyColors.Transparent,
                    StrokeThickness = 0,
                    FontSize = 12
                };

                model.Annotations.Add(textAnnotation);
            }

            // Окружности
            var circle1 = CreateCircleSeries(0, 0, R[0], OxyColors.Black);
            var circle2 = CreateCircleSeries(0, 0, R[1], OxyColors.Black);
            var circle3 = CreateCircleSeries(0, 0, R[2], OxyColors.Black);

            model.Series.Add(pointsSeries);
            model.Series.Add(circle1);
            model.Series.Add(circle2);
            model.Series.Add(circle3);

            // Добавление пунктирных линий
            AddDashedLines(model);

            constellationDiagram.Model = model;
        }

        private LineSeries CreateCircleSeries(double centerX, double centerY, double radius, OxyColor color)
        {
            var series = new LineSeries
            {
                Title = $"Окружность с радиусом {radius}",
                StrokeThickness = 2,
                MarkerType = MarkerType.None,
                Color = color
            };

            const int resolution = 100; // Количество точек для аппроксимации окружности
            for (int i = 0; i <= resolution; i++)
            {
                double angle = 2 * Math.PI * i / resolution;
                double x = centerX + radius * Math.Cos(angle);
                double y = centerY + radius * Math.Sin(angle);
                series.Points.Add(new DataPoint(x, y));
            }

            return series;
        }

        private void AddDashedLines(PlotModel model)
        {
            // Вертикальная пунктирная линия через центр окружностей
            var verticalLine = new LineAnnotation
            {
                Type = LineAnnotationType.Vertical,
                X = 0,
                Color = OxyColors.Black,
                StrokeThickness = 1,
                LineStyle = LineStyle.Dash
            };
            model.Annotations.Add(verticalLine);

            // Горизонтальная пунктирная линия через центр окружностей
            var horizontalLine = new LineAnnotation
            {
                Type = LineAnnotationType.Horizontal,
                Y = 0,
                Color = OxyColors.Black,
                StrokeThickness = 1,
                LineStyle = LineStyle.Dash
            };
            model.Annotations.Add(horizontalLine);
        }

        private void NextStepButton_step2_click(object sender, RoutedEventArgs e)
        {
            step2_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step2.Visibility = Visibility.Hidden;
            previousStepButton_step2.Visibility = Visibility.Hidden;

            nextStepButton_step3.Visibility = Visibility.Visible;
            previousStepButton_step3.Visibility = Visibility.Visible;
            step3_StackPanel.Visibility = Visibility.Visible;

            help_step2.Visibility = Visibility.Hidden;
            help_step3.Visibility = Visibility.Visible;
        }

        private void Help_step2_click(object sender, RoutedEventArgs e)
        {
            string help_2 = "Справка по модуляции.pdf";

            string resourcesPath = System.IO.Path.Combine(directory, resourcesFolder);

            string help_2_Path = System.IO.Path.Combine(resourcesPath, help_2);

            if (File.Exists(help_2_Path))
            {
                Process.Start(help_2_Path);
            }
            else
            {
                MessageBox.Show("Файл «Справка по модуляции» не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousStepButton_step3_click(object sender, RoutedEventArgs e)
        {
            step2_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_step2.Visibility = Visibility.Visible;
            previousStepButton_step2.Visibility = Visibility.Visible;

            nextStepButton_step3.Visibility = Visibility.Hidden;
            previousStepButton_step3.Visibility = Visibility.Hidden;
            step3_StackPanel.Visibility = Visibility.Hidden;

            help_step2.Visibility = Visibility.Visible;
            help_step3.Visibility = Visibility.Hidden;
        }

        private void NextStepButton_step3_click(object sender, RoutedEventArgs e)
        {
            step3_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step3.Visibility = Visibility.Hidden;
            previousStepButton_step3.Visibility = Visibility.Hidden;

            if (manualModeIsEnabled)
            {
                nextStepButton_step5.Visibility = Visibility.Visible;
                previousStepButton_step5.Visibility = Visibility.Visible;
                step5_StackPanel.Visibility = Visibility.Visible;

                help_step3.Visibility = Visibility.Hidden;
                help_step5.Visibility = Visibility.Visible;
            }
            else
            {
                nextStepButton_step4.Visibility = Visibility.Visible;
                previousStepButton_step4.Visibility = Visibility.Visible;
                step4_StackPanel.Visibility = Visibility.Visible;

                help_step3.Visibility = Visibility.Hidden;
                help_step4.Visibility = Visibility.Visible;
            }
        }

        private void Help_step3_click(object sender, RoutedEventArgs e)
        {
            string help_3 = "Справка по модуляции.pdf";

            string resourcesPath = System.IO.Path.Combine(directory, resourcesFolder);

            string help_3_Path = System.IO.Path.Combine(resourcesPath, help_3);

            if (File.Exists(help_3_Path))
            {
                Process.Start(help_3_Path);
            }
            else
            {
                MessageBox.Show("Файл «Справка по модуляции» не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousStepButton_step4_click(object sender, RoutedEventArgs e)
        {
            step4_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step4.Visibility = Visibility.Hidden;
            previousStepButton_step4.Visibility = Visibility.Hidden;

            step3_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_step3.Visibility = Visibility.Visible;
            previousStepButton_step3.Visibility = Visibility.Visible;

            help_step3.Visibility = Visibility.Visible;
            help_step4.Visibility = Visibility.Hidden;
        }

        private void NextStepButton_step4_click(object sender, RoutedEventArgs e)
        {
            step5_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_step5.Visibility = Visibility.Visible;
            previousStepButton_step5.Visibility = Visibility.Visible;

            step4_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step4.Visibility = Visibility.Hidden;
            previousStepButton_step4.Visibility = Visibility.Hidden;

            help_step4.Visibility = Visibility.Hidden;
            help_step5.Visibility = Visibility.Visible;
        }

        private void Help_step4_click(object sender, RoutedEventArgs e)
        {
            string help_4 = "Справка по шуму.pdf";

            string resourcesPath = System.IO.Path.Combine(directory, resourcesFolder);

            string help_4_Path = System.IO.Path.Combine(resourcesPath, help_4);

            if (File.Exists(help_4_Path))
            {
                Process.Start(help_4_Path);
            }
            else
            {
                MessageBox.Show("Файл «Справка по шуму» не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousStepButton_step5_click(object sender, RoutedEventArgs e)
        {
            step5_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step5.Visibility = Visibility.Hidden;
            previousStepButton_step5.Visibility = Visibility.Hidden;

            if (manualModeIsEnabled)
            {
                nextStepButton_step3.Visibility = Visibility.Visible;
                previousStepButton_step3.Visibility = Visibility.Visible;
                step3_StackPanel.Visibility = Visibility.Visible;

                help_step3.Visibility = Visibility.Visible;
                help_step5.Visibility = Visibility.Hidden;
            }
            else
            {
                step4_StackPanel.Visibility = Visibility.Visible;
                nextStepButton_step4.Visibility = Visibility.Visible;
                previousStepButton_step4.Visibility = Visibility.Visible;

                help_step4.Visibility = Visibility.Visible;
                help_step5.Visibility = Visibility.Hidden;
            }
        }

        private void NextStepButton_step5_click(object sender, RoutedEventArgs e)
        {
            step5_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step5.Visibility = Visibility.Hidden;
            previousStepButton_step5.Visibility = Visibility.Hidden;

            step6_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_step6.Visibility = Visibility.Visible;
            previousStepButton_step6.Visibility = Visibility.Visible;

            help_step5.Visibility = Visibility.Hidden;
            help_step6.Visibility = Visibility.Visible;
        }

        private void Help_step5_click(object sender, RoutedEventArgs e)
        {
            string help_5 = "Справка по демодуляции.pdf";

            string resourcesPath = System.IO.Path.Combine(directory, resourcesFolder);

            string help_5_Path = System.IO.Path.Combine(resourcesPath, help_5);

            if (File.Exists(help_5_Path))
            {
                Process.Start(help_5_Path);
            }
            else
            {
                MessageBox.Show("Файл «Справка по демодуляции» не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousStepButton_step6_click(object sender, RoutedEventArgs e)
        {
            step6_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step6.Visibility = Visibility.Hidden;
            previousStepButton_step6.Visibility = Visibility.Hidden;

            step5_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_step5.Visibility = Visibility.Visible;
            previousStepButton_step5.Visibility = Visibility.Visible;

            help_step5.Visibility = Visibility.Visible;
            help_step6.Visibility = Visibility.Hidden;
        }

        private void NextStepButton_step6_click(object sender, RoutedEventArgs e)
        {
            step7_StackPanel.Visibility = Visibility.Visible;
            previousStepButton_step7.Visibility = Visibility.Visible;

            step6_StackPanel.Visibility = Visibility.Hidden;
            nextStepButton_step6.Visibility = Visibility.Hidden;
            previousStepButton_step6.Visibility = Visibility.Hidden;

            help_step6.Visibility = Visibility.Hidden;
            help_step7.Visibility = Visibility.Visible;
        }

        private void Help_step6_click(object sender, RoutedEventArgs e)
        {
            string help_6 = "Справка по демодуляции.pdf";

            string resourcesPath = System.IO.Path.Combine(directory, resourcesFolder);

            string help_6_Path = System.IO.Path.Combine(resourcesPath, help_6);

            if (File.Exists(help_6_Path))
            {
                Process.Start(help_6_Path);
            }
            else
            {
                MessageBox.Show("Файл «Справка по демодуляции» не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PreviousStepButton_step7_click(object sender, RoutedEventArgs e)
        {
            step7_StackPanel.Visibility = Visibility.Hidden;
            previousStepButton_step7.Visibility = Visibility.Hidden;

            step6_StackPanel.Visibility = Visibility.Visible;
            nextStepButton_step6.Visibility = Visibility.Visible;
            previousStepButton_step6.Visibility = Visibility.Visible;

            help_step6.Visibility = Visibility.Visible;
            help_step7.Visibility = Visibility.Hidden;
        }

        private void Help_step7_click(object sender, RoutedEventArgs e)
        {
            string help_7 = "Справка по декодированию.pdf";

            string resourcesPath = System.IO.Path.Combine(directory, resourcesFolder);

            string help_7_Path = System.IO.Path.Combine(resourcesPath, help_7);

            if (File.Exists(help_7_Path))
            {
                Process.Start(help_7_Path);
            }
            else
            {
                MessageBox.Show("Файл «Справка по декодированию» не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            if (stopModeling.IsVisible && stopModeling.IsEnabled)
            {
                MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите закрыть программу? Прогресс моделирования будет сброшен.", "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }

            foreach (var childWindow in childWindows)
            {
                childWindow.Close();
            }
        }
    }
}