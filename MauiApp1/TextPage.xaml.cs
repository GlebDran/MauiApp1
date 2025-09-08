namespace MauiApp1;

public partial class TextPage : ContentPage
{
	Label lblTekst;
	Editor editorTekst;
	HorizontalStackLayout hsl;
	public TextPage()
	{
		lblTekst = new Label
		{
			Text = "Tekst: ",
			FontSize = 25,
			TextColor = Colors.Black,
			BackgroundColor = Colors.LightGray,
			FontFamily = "verdana",
			HorizontalTextAlignment = TextAlignment.Center,
			Margin = new Thickness(10)
		};
		editorTekst = new Editor
		{
			FontSize = 20,
			BackgroundColor = Color.FromRgb(200, 200, 100),
			TextColor = Colors.Black,
			FontFamily = "verdana",
			AutoSize = EditorAutoSizeOption.TextChanges,
			Placeholder = "Sisesta tekst",
			FontAttributes = FontAttributes.Italic,
		};
		editorTekst.TextChanged += EditorTekst_TextChanged;
		hsl = new HorizontalStackLayout
		{
			BackgroundColor = Color.FromRgb(120, 30, 50),
			Children = { lblTekst, editorTekst },
			HorizontalOptions = LayoutOptions.Center,
		};

		Content = hsl;

	}
	private void EditorTekst_TextChanged(object? sender, TextChangedEventArgs e)
	{
		lblTekst.Text = "Tekst: " + editorTekst.Text;
	}
}