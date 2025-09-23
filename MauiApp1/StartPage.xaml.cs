using Microsoft.Maui.Controls;

namespace MauiApp1;

public partial class StartPage : ContentPage
{
    // ������ ������� (���� ����� ���������� ������)
    public List<ContentPage> lehed = new List<ContentPage>()
    {
        new TextPage(),
        new FigurePage(),
        new TimerPage(),
        new Valgusfoor(),
        new DateTimePage(),
        new Snowman(),
        new GridPage()
    };

    // ������ ��� ������
    public List<string> tekstid = new List<string>()
    {
        "�������� � �������",
        "������",
        "������",
        "��������",
        "���� � �����",
        "��������",
        "�������� ������"
    };

    ScrollView sv;              // ������ ��� ���������
    VerticalStackLayout vsl;    // ��������� ��� ������

    public StartPage()
    {
        //InitializeComponent(); // XAML �� ���������
        Title = "Avaleht";

        // ���-�������� �� ��� �������� (������ bg.jpg � ����� Resources/Images/)
        BackgroundImageSource = "bgapp.jpg";

        // ������ ������������ ���������
        vsl = new VerticalStackLayout
        {
            Spacing = 15,
            Padding = 20
        };

        // ������ ������ �� ������ tekstid
        for (int i = 0; i < lehed.Count; i++)
        {
            Button nupp = new Button
            {
                Text = tekstid[i],
                FontSize = 20,
                TextColor = Colors.White,
                CornerRadius = 20,
                FontFamily = "verdana",
                ZIndex = i // �������� ������, ����� �����, ����� �������� ���������
            };

            // ��������� ������ � ���������
            vsl.Add(nupp);

            // ������������� �� ������� �������
            nupp.Clicked += Nupp_Clicked;
        }

        // ����������� �� � ������
        sv = new ScrollView { Content = vsl };

        // ������ ������ �������� ���������� ��������
        Content = sv;
    }

    // ��������� ����� �� ������
    private async void Nupp_Clicked(object? sender, EventArgs e)
    {
        Button nupp = (Button)sender;
        // ��������� �������� �� ������� ������
        await Navigation.PushAsync(lehed[nupp.ZIndex]);
    }
}
