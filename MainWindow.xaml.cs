using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Calc
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Свойства

		#region system
		// Раздел системных свойств

		/// <summary>
		/// Результат расчетов
		/// </summary>
		private string _resultShow = "0";
		/// <summary>
		/// Результат расчетов (на экране)
		/// </summary>
		public string ResultShow
		{
			get => _resultShow;
			set
			{
				if (_resultShow != value)
				{
					_resultShow = value;
					ResultTextBlock.Text = _resultShow;
				}
			}
		}
		/// <summary>
		/// Результат расчетов (готовое число)
		/// </summary>
		public double Result
		{
			get
			{
				try { return double.Parse(_resultShow); }
				catch (Exception) { return 0; }
			}
			set
			{
				ResultShow = value.ToString();
			}
		}

		/// <summary>
		/// Текущее значение
		/// </summary>
		private string _innerValueShow = "0";
		/// <summary>
		/// Текущее значение (на экране)
		/// </summary>
		public string InnerValueShow
		{
			get => _innerValueShow;
			set
			{
				if (_innerValueShow != value)
				{
					_innerValueShow = value;
					InnerValueTextBlock.Text = _innerValueShow;
				}
			}
		}
		/// <summary>
		/// Текущее значение (готовое число)
		/// </summary>
		public double InnerValue
		{
			get
			{
				try { return double.Parse(InnerValueTextBlock.Text); }
				catch (Exception) { return 0; }
			}
			set
			{
				InnerValueShow = value.ToString();
			}
		}

		/// <summary>
		/// Значение в памяти
		/// </summary>
		private string _memoryShow = "0";
		/// <summary>
		/// Значение в памяти (на экране)
		/// </summary>
		public string MemoryShow
		{
			get => _memoryShow;
			set
			{
				if (_memoryShow != value)
				{
					_memoryShow = value;
					MemoryTextBlock.Text = _memoryShow;
				}
			}
		}
		/// <summary>
		/// Значение в памяти (готовое число)
		/// </summary>
		public double Memory
		{
			get
			{
				try { return double.Parse(_memoryShow); }
				catch (Exception) { return 0; }
			}
			set
			{
				MemoryShow = value.ToString();
			}
		}

		/// <summary>
		/// Последняя выбранная операция для взаимодействия результата и текущего значения
		/// </summary>
		private EOperationType? operation = null;
		/// <summary>
		/// Последняя выбранная операция для взаимодействия результата и текущего значения
		/// </summary>
		private string _operationShow = "";
		/// <summary>
		/// Последняя выбранная операция 
		/// для взаимодействия результата и текущего значения (на экране)
		/// </summary>
		public string OperationShow
		{
			get => _operationShow;
			set
			{
				if (_operationShow != value)
				{
					_operationShow = value;
					OperationTextBlock.Text = _operationShow;
				}
			}
		}

		/// <summary>
		/// Предоставляет актуальную информацию о зажатой клавише Shift
		/// </summary>
		private bool _isShift = false;
		/// <summary>
		/// Предоставляет актуальную информацию о зажатой клавише Shift
		/// </summary>
		public bool IsShift { get => _isShift; set => _isShift = value; }

		#endregion

		#endregion

		public MainWindow() => InitializeComponent();

		#region Методы

		#region system

		/// <summary>
		/// Метод отчистки строки от лишних нулей
		/// </summary>
		/// <param name="text">Текст, который необходимо отчистить</param>
		/// <returns>Отчищеный текст</returns>
		private string ClearZeros(string text) 
		{
			Regex regex = new Regex(@"^-?0{1,}[0-9]");

			var target = regex.Match(text).Value;
			text = regex.Replace(text, target.Replace("0", ""));
			return string.IsNullOrWhiteSpace(text) ? "0" : text;
		}

		/// <summary>
		/// Метод внесения новых символов в строку
		/// </summary>
		/// <param name="sumbol">Символ, который будет внесен</param>
		private void AddSymbol(char sumbol)
		{
			try
			{
				if (InnerValueShow.Length + 1 > 15) return;

				double.Parse(InnerValueShow + sumbol);
				InnerValueShow += sumbol;
				InnerValueShow = ClearZeros(InnerValueShow);
			}
			catch (Exception) { }
		}

		/// <summary>
		/// Метод удаление последнего символа в текущем значении
		/// </summary>
		/// <param name="sumbol">Символ, который будет внесен</param>
		private void ClearSymbol()
		{
			try
			{
				if (InnerValueShow.Length <= 1) InnerValueShow = "0";

				double.Parse(InnerValueShow.Remove(InnerValueShow.Length - 1));
				InnerValueShow = InnerValueShow.Remove(InnerValueShow.Length - 1);
				InnerValueShow = ClearZeros(InnerValueShow);

			}
			catch (Exception) { }
		}

		/// <summary>
		/// Обработчик взаимодействия с пустым результатом
		/// </summary>
		private void ResultZero()
		{
			if (Result == 0)
			{
				Result = InnerValue;
				InnerValue = 0;
			}
		}
		
		/// <summary>
		/// Обработка применения выбранного оператора
		/// </summary>
		private void Operation()
		{
			ResultZero();
			if (Result != 0 && operation != null)
			{
				switch (operation)
				{
					case EOperationType.Addition: Result = Result + InnerValue; break;
					case EOperationType.Subtraction: Result = Result - InnerValue; break;
					case EOperationType.DivisionBy: try { Result = Result / InnerValue; } catch (Exception) { } break;
					case EOperationType.Multiplication: Result = Result * InnerValue; break;
					default: break;
				}
				_innerValueShow = "0";
			}
		}

		/// <summary>
		/// Обработчик события KeyDown
		/// </summary>
		private void _KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Delete: InnerValue = 0; break;
				case Key.Back: ClearSymbol(); break;

				case Key.Subtract: SubtractionButton_Click(); break;
				case Key.OemMinus: SubtractionButton_Click(); break;

				case Key.Return: EqualityButton_Click(); break;

				case Key.OemPlus: if (IsShift) AdditionButton_Click(); else EqualityButton_Click(); break;
				case Key.Add: AdditionButton_Click(); break;

				case Key.Divide: DivisionButton_Click(); break;
				case Key.Oem5: DivisionButton_Click(); break;
				case Key.OemQuestion: if (IsShift) DoteButton_Click(); else DivisionButton_Click(); break;

				case Key.Decimal: DoteButton_Click(); break;

				case Key.Multiply: MultiplicationButton_Click(); break;

				case Key.D0: ZeroButton_Click(); break;
				case Key.NumPad0: ZeroButton_Click(); break;

				case Key.D1: OneButton_Click(); break;
				case Key.NumPad1: OneButton_Click(); break;
				
				case Key.D2: TwoButton_Click(); break;
				case Key.NumPad2: TwoButton_Click(); break;
				
				case Key.D3: ThreeButton_Click(); break;
				case Key.NumPad3: ThreeButton_Click(); break;
				
				case Key.D4: FourButton_Click(); break;
				case Key.NumPad4: FourButton_Click(); break;
				
				case Key.D5: if (IsShift) PercentButton_Click(); else FiveButton_Click(); break;
				case Key.NumPad5: FiveButton_Click(); break;
				
				case Key.D6: if (IsShift) SquareDegreeButton_Click(); else SixButton_Click(); break;
				case Key.NumPad6: SixButton_Click(); break;
				
				case Key.D7: SevenButton_Click(); break;
				case Key.NumPad7: SevenButton_Click(); break;
				
				case Key.D8: if (IsShift) MultiplicationButton_Click(); else EightButton_Click(); break;
				case Key.NumPad8: EightButton_Click(); break;
				
				case Key.D9: NineButton_Click(); break;
				case Key.NumPad9: NineButton_Click(); break;
				
				case Key.LeftShift: IsShift = true; break;
				case Key.RightShift: IsShift = true; break;
				
				// default: MessageBox.Show($"Key:{e.Key}"); break;
			}
			
		}
		/// <summary>
		/// Обработчик события KeyUp 
		/// </summary>
		private void _KeyUp(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.LeftShift: IsShift = false; break;
				case Key.RightShift: IsShift = false; break;
				default: break;
			}
		}

		#region system click

		/// <summary>
		/// Очистка Текущего значения
		/// </summary>
		private void CEButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			InnerValueShow = "0";
		}

		/// <summary>
		/// Очистка общая
		/// </summary>
		private void CButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			InnerValueShow = "0";
			ResultShow = "0";
			OperationShow = "";
			operation = null;
		}

		/// <summary>
		/// Стереть один символ
		/// </summary>
		private void BackSpaceButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			ClearSymbol();
		}

		/// <summary>
		/// Подытожить
		/// </summary>
		private void EqualityButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			Operation();
		}

		#region обработка символов

		private void SevenButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			AddSymbol('7');
		}

		private void EightButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			AddSymbol('8');
		}

		private void NineButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			AddSymbol('9');
		}

		private void FourButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			AddSymbol('4');
		}

		private void FiveButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			AddSymbol('5');
		}

		private void SixButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			AddSymbol('6');
		}

		private void OneButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			AddSymbol('1');
		}

		private void TwoButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			AddSymbol('2');
		}

		private void ThreeButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			AddSymbol('3');
		}

		private void ZeroButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			AddSymbol('0');
		}

		private void DoteButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			AddSymbol(',');
		}

		#endregion

		#region работа с памятью

		/// <summary>
		/// Очистка памяти
		/// </summary>
		private void MCButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			Memory = 0;
		}
		/// <summary>
		/// Вставить из памяти
		/// </summary>
		private void MRButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			InnerValue = Memory;
		}
		/// <summary>
		/// Прибавить к памяти
		/// </summary>
		private void MPlusButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			Memory += InnerValue;
		}
		/// <summary>
		/// Отнять от памяти
		/// </summary>
		private void MMinusButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			Memory -= InnerValue;
		}

		#endregion

		#endregion

		#endregion

		#region click

		#region Парные операции

		/// <summary>
		/// Обработчик нажатия кнопки деления
		/// </summary>
		private void DivisionButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			operation = EOperationType.DivisionBy;
			OperationShow = DivisionButton.Content.ToString();

			ResultZero();
		}

		/// <summary>
		/// Обработчик нажатия кнопки умножения
		/// </summary>
		private void MultiplicationButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			operation = EOperationType.Multiplication;
			OperationShow = MultiplicationButton.Content.ToString();

			ResultZero();
		}

		/// <summary>
		/// Обработчик нажатия кнопки вычитание
		/// </summary>
		private void SubtractionButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			operation = EOperationType.Subtraction;
			OperationShow = SubtractionButton.Content.ToString();

			ResultZero();
		}

		/// <summary>
		/// Обработчик нажатия кнопки сложения
		/// </summary>
		private void AdditionButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			operation = EOperationType.Addition;
			OperationShow = AdditionButton.Content.ToString();

			ResultZero();
		}

		#endregion

		#region Унарные операции

		/// <summary>
		/// Операция процента
		/// </summary>
		private void PercentButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			try { InnerValue = InnerValue * Result / 100; }
			catch (Exception) { }
}

		/// <summary>
		/// Делить один на текущее число
		/// </summary>
		private void OneDivideButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			try { InnerValue = 1 / InnerValue; }
			catch (Exception) { }
		}

		/// <summary>
		/// Возводит текущее число в квадрат
		/// </summary>
		private void SquareDegreeButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			try { InnerValue = Math.Pow(InnerValue, 2); }
			catch (Exception) { }
		}
		
		/// <summary>
		/// Получает квадратный корень из текущего числа
		/// </summary>
		private void SquareRootButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			try { InnerValue = Math.Sqrt(InnerValue); }
			catch (Exception) { }
		}

		/// <summary>
		/// Инверсирует знак текущего числа
		/// </summary>
		private void NegateButton_Click(object sender = null, RoutedEventArgs e = null)
		{
			try { InnerValue *= -1; }
			catch (Exception) { }
		}

		#endregion

		#endregion

		#endregion

		/// <summary>
		/// Типы операций
		/// </summary>
		enum EOperationType
		{
			/// <summary>
			/// Деление
			/// </summary>
			DivisionBy,
			/// <summary>
			/// Умножение
			/// </summary>
			Multiplication,
			/// <summary>
			/// Вычитание
			/// </summary>
			Subtraction,
			/// <summary>
			/// Сложение
			/// </summary>
			Addition
		}

	}
}
