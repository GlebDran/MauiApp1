using System.Threading.Tasks;

namespace MauiApp1;

public partial class StartPage : ContentPage
{
	public List<ContentPage> lehed = new List<ContentPage>() { new TextPage(), new FigurePage(), new TimerPage() }; //spisok stranic kotorqe v prilozhenii
	public List<string> tekstid = new List<string>() { "Tee lahti leht Tekstiga", "Tee lahti Figure leht", "Timer leht" };
	ScrollView sv;
	VerticalStackLayout vsl; //zadali nazvanie peremennoj
	public StartPage()
	{
		//InitializeComponent();
		Title = "Avaleht";
		vsl = new VerticalStackLayout { BackgroundColor = Color.FromRgb(120, 30, 50) };
		for (int i = 0; i < lehed.Count; i++)
		{
			Button nupp = new Button
			{
				Text = tekstid[i],
				FontSize = 20,
				TextColor = Colors.Black,
				CornerRadius = 20,
				FontFamily = "verdana",
				ZIndex=i
			};
			vsl.Add(nupp);
            nupp.Clicked += Nupp_Clicked;
		}
		sv = new ScrollView { Content = vsl };
		Content = sv;
	}

    private async void Nupp_Clicked(object? sender, EventArgs e)
    {
        Button nupp = (Button)sender;
		await Navigation.PushAsync(lehed[nupp.ZIndex]);
    }
}