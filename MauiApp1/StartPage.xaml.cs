using Microsoft.Maui.Controls;

namespace MauiApp1;

public partial class StartPage : ContentPage
{
    // список страниц (куда будут переходить кнопки)
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

    // тексты для кнопок
    public List<string> tekstid = new List<string>()
    {
        "Страница с текстом",
        "Фигуры",
        "Таймер",
        "Светофор",
        "Дата и время",
        "Снеговик",
        "Крестики нолики"
    };

    ScrollView sv;              // скролл для прокрутки
    VerticalStackLayout vsl;    // контейнер для кнопок

    public StartPage()
    {
        //InitializeComponent(); // XAML не использую
        Title = "Avaleht";

        // фон-картинка на всю страницу (положи bg.jpg в папку Resources/Images/)
        BackgroundImageSource = "bgapp.jpg";

        // создаём вертикальный контейнер
        vsl = new VerticalStackLayout
        {
            Spacing = 15,
            Padding = 20
        };

        // создаём кнопки из списка tekstid
        for (int i = 0; i < lehed.Count; i++)
        {
            Button nupp = new Button
            {
                Text = tekstid[i],
                FontSize = 20,
                TextColor = Colors.White,
                CornerRadius = 20,
                FontFamily = "verdana",
                ZIndex = i // запомним индекс, чтобы знать, какую страницу открывать
            };

            // добавляем кнопку в контейнер
            vsl.Add(nupp);

            // подписываемся на событие нажатия
            nupp.Clicked += Nupp_Clicked;
        }

        // оборачиваем всё в скролл
        sv = new ScrollView { Content = vsl };

        // делаем скролл основным содержимым страницы
        Content = sv;
    }

    // обработка клика по кнопке
    private async void Nupp_Clicked(object? sender, EventArgs e)
    {
        Button nupp = (Button)sender;
        // открываем страницу по индексу кнопки
        await Navigation.PushAsync(lehed[nupp.ZIndex]);
    }
}
