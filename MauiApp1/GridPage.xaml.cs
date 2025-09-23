using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace MauiApp1;

public partial class GridPage : ContentPage
{
    Grid grid;

    public GridPage()
    {
        //InitializeComponent(); // �� �����, �� ������ � ����

        // ��� �������� (���� �����, ����� ����� ������ ���� �����)
        BackgroundColor = Colors.LightGray;

        grid = new Grid
        {
            BackgroundColor = Color.FromArgb("#EAEAEA"),
            Padding = 12,          // ������� ������ �����
            RowSpacing = 8,        // ���������� ����� ��������
            ColumnSpacing = 8,     // ���������� ����� ���������

            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star },
                new RowDefinition { Height = GridLength.Star }
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };

        // �����: ���������� ����� �� ��������
        Content = grid;

        // 3x3 ������
        for (int rida = 0; rida < 3; rida++)
        {
            for (int veerg = 0; veerg < 3; veerg++)
            {
                var box = new BoxView
                {
                    Color = Colors.White, // ���� "������"
                    Opacity = 1.0
                    // CornerRadius � BoxView ��� � �������
                };

                var tap = new TapGestureRecognizer();
                tap.Tapped += Tap_Tapped;
                box.GestureRecognizers.Add(tap);

                // ��������� � ����� (�������, ������)
                grid.Add(box, veerg, rida);
            }
        }
    }

    private async void Tap_Tapped(object sender, TappedEventArgs e)
    {
        var box = (BoxView)sender;
        var r = Grid.GetRow(box);
        var v = Grid.GetColumn(box);
        //await DisplayAlert("����", $"��� {r}, ������ {v}", "OK");
    }
}
