using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace SPTask2FileCopying;

public partial class MainWindow : Window, INotifyPropertyChanged
{
	public MainWindow()
	{
		InitializeComponent();

		DataContext = this;

		Min = 0;
		Value = 0;
		Max = 100;
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	private int _min;
	private int _max;
	private int _value;

	public int Min
	{
		get => _min;
		set { _min = value; OnPropertyChanged(); }
	}
	public int Max
	{
		get => _max;
		set { _max = value; OnPropertyChanged(); }
	}
	public int Value
	{
		get => _value;
		set { _value = value; OnPropertyChanged(); }
	}

	public string GetFileName()
	{
		var dialog = new OpenFileDialog();

		dialog.Filter = "(*.txt)|*.txt";

		dialog.ShowDialog();

		return dialog.FileName;
	}

	private void FromBtn_Click(object sender, RoutedEventArgs e) => FromTB.Text = GetFileName();

	private void ToBtn_Click(object sender, RoutedEventArgs e) => ToTB.Text = GetFileName();

	private void CopyBtn_Click(object sender, RoutedEventArgs e)
	{
		string FromFile = FromTB.Text;
		string ToFile = ToTB.Text;

		if (FromFile == string.Empty || ToFile == string.Empty)
		{
			MessageBox.Show("Please, Select Files.", "File Operation.", MessageBoxButton.OK, MessageBoxImage.Warning);
			return;
		}

		Value = 0;

		MessageBox.Show("Copying Started.", "File Operation.", MessageBoxButton.OK, MessageBoxImage.Information);

		var operationThread = new Thread(() =>
		{
			byte[] FromArr = File.ReadAllBytes(FromFile);

			int currentIndex = 0;

			Max = FromArr.Length;

			using (FileStream fs = new(ToFile, FileMode.OpenOrCreate))
			{
				while (currentIndex < Max)
				{
					fs.WriteByte(FromArr[currentIndex++]);

					fs.Flush();

					Thread.Sleep(10);

					Dispatcher.Invoke(() => Value++);
				}

				MessageBox.Show("Copying Success.", "File Operation.", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		});

		operationThread.Start();
	}

	private void MainWin_Closing(object sender, CancelEventArgs e) => App.Current.Shutdown();

}