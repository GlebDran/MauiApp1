using System.Linq;

// -------- убираем конфликт "Image" ----------
using Image = Microsoft.Maui.Controls.Image;

#if ANDROID
using AMedia = Android.Media;  // alias для Android.Media.*
#endif
// --------------------------------------------

namespace MauiApp1;

public partial class TttPage : ContentPage
{
    // подписи кнопки бота
    const string BotOnLabel = "Игра с Димоном: вкл";
    const string BotOffLabel = "Игра с Димоном: выкл";

    // параметры/состояние
    int size = 3;
    bool vsBot = false;       // играет ли бот (Димон, всегда O)
    bool gameOver = false;
    readonly Random rng = new();

    // визуал/логика доски
    Image[,]? pics;           // изображения в клетках
    char?[,]? board;          // 'X' / 'O' / null

    // оформление клеток/линий
    readonly Color cellColor = Color.FromArgb("#F7F3EE");
    readonly Color lineColor = Color.FromArgb("#3A3F45");

    // файлы стикеров
    string userSticker = "user1.png"; // твой текущий (X)
    string botSticker = "bot.png";    // Димон (O)

    // красивые имена -> файлы стикеров пользователя
    readonly Dictionary<string, string> _userSkins = new()
    {
        ["Красный"] = "user1.png",
        ["Зелёный"] = "user2.png",
        ["Чёрный"] = "user3.png",
        ["Синий"] = "user4.png"
    };

    // === добавлено для звука ===
    // Файл должен лежать в Resources/Raw/dimonover.wav (MauiAsset)
    const string GameOverWav = "dimonover.wav";
#if ANDROID
    string? _tmpAndroidPath; // путь к временному файлу, чтобы MediaPlayer мог его воспроизвести
#endif
    // === конец: добавлено для звука ===

    public TttPage()
    {
        InitializeComponent();
        BuildBoard();
        BotButton.Text = vsBot ? BotOnLabel : BotOffLabel;
    }

    // Построение поля
    void BuildBoard()
    {
        BoardGrid.Children.Clear();
        BoardGrid.RowDefinitions.Clear();
        BoardGrid.ColumnDefinitions.Clear();

        const int t = 6;
        BoardGrid.BackgroundColor = lineColor;
        BoardGrid.Padding = t;
        BoardGrid.RowSpacing = t;
        BoardGrid.ColumnSpacing = t;

        for (int i = 0; i < size; i++)
        {
            BoardGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            BoardGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        pics = new Image[size, size];
        board = new char?[size, size];

        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
            {
                var img = new Image { Aspect = Aspect.AspectFit };
                var cell = new ContentView { BackgroundColor = cellColor, Content = img };

                var tap = new TapGestureRecognizer();
                int rr = r, cc = c;
                tap.Tapped += (s, e) => OnCellTapped(rr, cc);
                cell.GestureRecognizers.Add(tap);

                Grid.SetRow(cell, r);
                Grid.SetColumn(cell, c);
                BoardGrid.Children.Add(cell);

                pics[r, c] = img;
                board[r, c] = null;
            }

        gameOver = false;
        BoardGrid.InputTransparent = false;
    }

    // Клик по клетке (ходит игрок X). Бот всегда O.
    async void OnCellTapped(int r, int c)
    {
        if (gameOver || board![r, c] != null) return;

        PutSticker(r, c, 'X');

        if (CheckWinner('X')) { await EndGameAsync("X выиграл!"); return; }
        if (IsDraw()) { await EndGameAsync("Ничья!"); return; }

        if (vsBot) // ход Димона
        {
            await Task.Delay(150);
            var move = ChooseMediumForO() ?? RandomEmpty();
            if (move is not null)
            {
                var (rr, cc) = move.Value;
                PutSticker(rr, cc, 'O');

                if (CheckWinner('O')) { await EndGameAsync("O выиграл!"); return; }
                if (IsDraw()) { await EndGameAsync("Ничья!"); return; }
            }
        }
    }

    // Поставить картинку и обновить внутреннюю доску
    void PutSticker(int r, int c, char who)
    {
        board![r, c] = who;
        pics![r, c].Source = ImageSource.FromFile(who == 'X' ? userSticker : botSticker);
    }

    // Победа/ничья/рандомы
    bool CheckWinner(char who)
    {
        for (int r = 0; r < size; r++)
            if (Enumerable.Range(0, size).All(c => board![r, c] == who)) return true;

        for (int c = 0; c < size; c++)
            if (Enumerable.Range(0, size).All(r => board![r, c] == who)) return true;

        if (Enumerable.Range(0, size).All(i => board![i, i] == who)) return true;
        if (Enumerable.Range(0, size).All(i => board![i, size - 1 - i] == who)) return true;

        return false;
    }

    bool IsDraw()
    {
        foreach (var v in board!) if (v is null) return false;
        return !CheckWinner('X') && !CheckWinner('O');
    }

    (int r, int c)? RandomEmpty()
    {
        var list = new List<(int r, int c)>();
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                if (board![r, c] is null) list.Add((r, c));
        return list.Count == 0 ? null : list[rng.Next(list.Count)];
    }

    IEnumerable<(int r, int c)> EmptyCells()
    {
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                if (board![r, c] is null) yield return (r, c);
    }

    // «Средний» Димон: выиграть -> блок -> центр -> случайно
    (int r, int c)? ChooseMediumForO()
    {
        foreach (var (r, c) in EmptyCells())
            if (WouldWinAt(r, c, 'O')) return (r, c);

        foreach (var (r, c) in EmptyCells())
            if (WouldWinAt(r, c, 'X')) return (r, c);

        if (size % 2 == 1)
        {
            int m = size / 2;
            if (board![m, m] is null) return (m, m);
        }
        return null;
    }

    bool WouldWinAt(int r, int c, char who)
    {
        var old = board![r, c];
        board[r, c] = who;
        bool win = CheckWinner(who);
        board[r, c] = old;
        return win;
    }

    async Task EndGameAsync(string message)
    {
        gameOver = true;
        BoardGrid.InputTransparent = true;
        await PlayGameOverAsync();                 // === звук конца игры ===
        await DisplayAlert("Игра окончена", message, "OK");
    }

    // -------- КНОПКИ --------

    void NewGameClicked(object sender, EventArgs e) => BuildBoard();

    // Кто первый? — Димон / Тапки / Вообще похую
    async void WhoStartsClicked(object sender, EventArgs e)
    {
        var ch = await DisplayActionSheet("Кто первый?", "Отмена", null,
                                          "Димон", "Тапки", "Вообще похую");
        if (string.IsNullOrEmpty(ch) || ch == "Отмена") return;

        bool botFirst = ch switch
        {
            "Димон" => true,
            "Тапки" => false,
            "Вообще похую" => rng.Next(2) == 0,
            _ => false
        };

        BuildBoard();

        if (botFirst)
        {
            if (!vsBot) { vsBot = true; BotButton.Text = BotOnLabel; }

            await Task.Delay(120);
            var move = ChooseMediumForO() ?? RandomEmpty();
            if (move is not null)
            {
                var (r, c) = move.Value;
                PutSticker(r, c, 'O');
            }
        }
    }

    // Выбор стикера для игрока X красивыми названиями
    async void ChooseStickerClicked(object sender, EventArgs e)
    {
        var choice = await DisplayActionSheet("Стикер для Тапков (X)", "Отмена", null,
                                              _userSkins.Keys.ToArray());
        if (string.IsNullOrEmpty(choice) || choice == "Отмена") return;

        userSticker = _userSkins[choice];

        // обновим уже поставленные X
        for (int r = 0; r < size; r++)
            for (int c = 0; c < size; c++)
                if (board![r, c] == 'X')
                    pics![r, c].Source = ImageSource.FromFile(userSticker);
    }

    // Вкл/выкл Димона
    void ToggleBotClicked(object sender, EventArgs e)
    {
        vsBot = !vsBot;
        BotButton.Text = vsBot ? BotOnLabel : BotOffLabel;
        BuildBoard();
    }

    // Размер поля
    async void ChangeSizeClicked(object sender, EventArgs e)
    {
        var ch = await DisplayActionSheet("Размер поля", "Отмена", null, "3×3", "4×4", "5×5");
        if (string.IsNullOrEmpty(ch) || ch == "Отмена") return;

        int ns = ch switch { "4×4" => 4, "5×5" => 5, _ => 3 };
        if (ns != size) { size = ns; BuildBoard(); }
    }

    // (если нужна кнопка «Выход» — повесь на Navigation.PopToRootAsync())
    async void ExitClicked(object sender, EventArgs e)
        => await Navigation.PopToRootAsync();

    // === добавлено для звука ===
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await EnsureSoundPreparedAsync(); // прогреем звук, чтобы не лагало
    }

    async Task EnsureSoundPreparedAsync()
    {
#if ANDROID
        if (_tmpAndroidPath != null) return;
        using var s = await FileSystem.OpenAppPackageFileAsync(GameOverWav);
        var tmp = Path.Combine(FileSystem.CacheDirectory, GameOverWav);
        using (var fs = File.Create(tmp))
            await s.CopyToAsync(fs);
        _tmpAndroidPath = tmp;
#else
        await Task.CompletedTask; // другие платформы — ничего не делаем
#endif
    }

    async Task PlayGameOverAsync()
    {
#if ANDROID
        try
        {
            if (_tmpAndroidPath == null) await EnsureSoundPreparedAsync();

            var mp = new AMedia.MediaPlayer();
            mp.Prepared += (s, e) => mp.Start();
            mp.Completion += (s, e) => { mp.Release(); mp.Dispose(); };
            mp.SetDataSource(_tmpAndroidPath);
            mp.PrepareAsync();
        }
        catch { /* ignore */ }
#else
        await Task.CompletedTask; // другие платформы — без звука
#endif
    }
    // === конец: добавлено для звука ===
}
