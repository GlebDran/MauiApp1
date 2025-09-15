using Microsoft.Maui.Layouts;

namespace MauiApp1;

public partial class DateTimePage : ContentPage
{
	Label mis_on_valitud;
	DatePicker datePicker;
	TimePicker timePicker;
	AbsoluteLayout al;
    public DateTimePage()
	{
		//InitializeComponent();
		mis_on_valitud = new Label 
		{ 
		Text = "Siin kuvatakse valitud kuupaev voi kellaeg",
		FontSize = 20,
		TextColor = Colors.Blue,
		FontFamily="Verdana"
		};
		datePicker = new DatePicker
		{
			FontSize = 20,
			Background = Color.FromRgb(200,200,100),
			TextColor = Colors.Black,
			FontFamily = "Verdana",
			MinimumDate = DateTime.Now.AddDays(-7),
			MaximumDate = new DateTime(2100, 12, 31),
			Date = DateTime.Now,
			Format = "D"

        };
		datePicker.DateSelected += Kuupaeva_valimine;
		al = new AbsoluteLayout { Children = { mis_on_valitud, datePicker } };
		AbsoluteLayout.SetLayoutBounds(mis_on_valitud, new Rect(0.5, 0.2, 0.9, 0.2));
		AbsoluteLayout.SetLayoutFlags(mis_on_valitud, AbsoluteLayoutFlags.All);
        AbsoluteLayout.SetLayoutBounds(mis_on_valitud, new Rect(0.5, 0.5, AbsoluteLayout.AutoSize, 0.2));
        AbsoluteLayout.SetLayoutFlags(mis_on_valitud, AbsoluteLayoutFlags.All);
		Content = al;
    }
	partial void Kuupaeva_valimine(object? sender, DateChangedEventArgs e)
	{
		mis_on_valitud.Text = $"Valisite kuupaevaks: " + e.NewDate.ToString("D");
    }

}