using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ПР8;

namespace CarConfigurator
{
    public partial class MainWindow : Window
    {
        private CarConfig config = new CarConfig();
        private int currentStep = 1;

        public MainWindow()
        {
            InitializeComponent();

          
            UpdateStepUI();
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep == 5)
            {
                MessageBox.Show("Заявка успешно оформлена!\nМенеджер свяжется с вами в ближайшее время.", "Успех");
                return;
            }

            currentStep++;
            UpdateStepUI();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (currentStep == 5 && (!string.IsNullOrWhiteSpace(NameBox.Text) || !string.IsNullOrWhiteSpace(PhoneBox.Text)))
            {
                var result = MessageBox.Show("Вы уверены, что хотите вернуться? Введенные данные могут быть потеряны.", "Внимание", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.No) return;
            }

            currentStep--;
            UpdateStepUI();
        }

        private void UpdateStepUI()
        {
            StepsTab.SelectedIndex = currentStep - 1;
            StepProgress.Value = currentStep;

            string[] titles = {
                "Шаг 1: Модель и двигатель",
                "Шаг 2: Цвет и опции",
                "Шаг 3: Стоимость",
                "Шаг 4: Кредит",
                "Шаг 5: Оформление"
            };
            StepTitle.Text = titles[currentStep - 1];

            BackBtn.IsEnabled = currentStep > 1;
            NextBtn.Content = currentStep == 5 ? "Оформить заявку" : "Далее";

            if (currentStep == 3) UpdateSummary();
            if (currentStep == 4) UpdateLoanDetails();
            if (currentStep == 5) UpdateFinalSummary();
        }

        private void StepsTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StepsTab.SelectedIndex + 1 != currentStep)
            {
                StepsTab.SelectedIndex = currentStep - 1;
            }
        }

        private void ModelCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ModelCombo.SelectedIndex == 0) { config.Model = "Базовая"; config.BasePrice = 1000000; }
            if (ModelCombo.SelectedIndex == 1) { config.Model = "Спортивная"; config.BasePrice = 1500000; }
            if (ModelCombo.SelectedIndex == 2) { config.Model = "Люкс"; config.BasePrice = 2000000; }
        }

        private void EngineCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EngineCombo.SelectedIndex == 0) { config.Engine = "Бензин"; config.EnginePrice = 0; }
            if (EngineCombo.SelectedIndex == 1) { config.Engine = "Дизель"; config.EnginePrice = 50000; }
            if (EngineCombo.SelectedIndex == 2) { config.Engine = "Электро"; config.EnginePrice = 150000; }
        }

        private void ColorCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ColorCombo.SelectedIndex == 0) { config.Color = "Белый"; config.ColorPrice = 0; }
            if (ColorCombo.SelectedIndex == 1) { config.Color = "Черный"; config.ColorPrice = 20000; }
            if (ColorCombo.SelectedIndex == 2) { config.Color = "Красный"; config.ColorPrice = 40000; }
        }

        private void Options_Changed(object sender, RoutedEventArgs e)
        {
            if (ClimateCheck == null || AudioCheck == null) return;
            config.HasClimate = ClimateCheck.IsChecked == true;
            config.HasAudio = AudioCheck.IsChecked == true;
        }

        private void UpdateSummary()
        {
            double total = config.GetTotalCarPrice();
            TotalPriceText.Text = $"{total:N0} руб.";

            string options = "";
            if (config.HasClimate) options += "Климат-контроль (+50 000), ";
            if (config.HasAudio) options += "Премиум аудио (+30 000), ";
            if (options == "") options = "нет";

            SummaryText.Text = $"Модель: {config.Model} ({config.BasePrice:N0} руб.)\n" +
                               $"Двигатель: {config.Engine} (+{config.EnginePrice:N0} руб.)\n" +
                               $"Цвет: {config.Color} (+{config.ColorPrice:N0} руб.)\n" +
                               $"Опции: {options}";
        }
        private void LoanSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (InitialPayText == null || LoanTermText == null) return;

            config.InitialPaymentPercent = InitialPaySlider.Value;
            config.LoanTerm = (int)LoanTermSlider.Value;

            InitialPayText.Text = $"{config.InitialPaymentPercent}%";
            LoanTermText.Text = $"{config.LoanTerm} мес.";

            UpdateLoanDetails();
        }

        private void UpdateLoanDetails()
        {
            if (LoanDetailsText == null) return;

            double total = config.GetTotalCarPrice();
            double initialSum = total * (config.InitialPaymentPercent / 100);
            double creditSum = total - initialSum;

            double monthsInYears = config.LoanTerm / 12.0;
            double overpayment = creditSum * 0.10 * monthsInYears;
            double monthlyPayment = (creditSum + overpayment) / config.LoanTerm;

            LoanDetailsText.Text = $"Стоимость авто: {total:N0} руб.\n" +
                                   $"Первоначальный взнос: {initialSum:N0} руб.\n" +
                                   $"Сумма кредита: {creditSum:N0} руб.\n" +
                                   $"Ежемесячный платеж: {monthlyPayment:N0} руб.";
        }

        private void UpdateFinalSummary()
        {
            double total = config.GetTotalCarPrice();
            FinalSummaryText.Text = $"К оплате: {total:N0} руб.";
        }

        private void PhoneBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text[0])) e.Handled = true;
        }

        private void Form_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NextBtn == null) return;

            bool isValid = NameBox.Text.Length >= 2 &&
                           PhoneBox.Text.Length >= 5 &&
                           EmailBox.Text.Contains("@");

            if (currentStep == 5)
                NextBtn.IsEnabled = isValid;
        }
    }
}