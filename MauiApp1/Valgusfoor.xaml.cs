using Microsoft.Maui.Controls;

namespace MauiApp1;

public partial class Valgusfoor : ContentPage
{
    // включён ли светофор
    bool isOn = false;

    // выбран ли ночной режим (переключатель)
    bool isNight = false;

    // таймеры: день (переключение фаз), ночь (мигание), часы
    IDispatcherTimer? dayTimer;
    IDispatcherTimer? nightTimer;
    IDispatcherTimer? clockTimer;

    // фазы дневного цикла
    enum Phase { Red, Yellow, Green }
    Phase current = Phase.Red;

    // секунд осталось в текущей фазе (для обратного отсчёта)
    int secondsLeft = 0;

    // длительности фаз
    const int RED_SEC = 7;
    const int YELLOW_SEC = 2;
    const int GREEN_SEC = 6;

    public Valgusfoor()
    {
        InitializeComponent();

        StartClock(); // часы сверху
        PaintOff();   // изначально всё серое
    }

    //КНОПКИ

    //включить
    void OnSisseClicked(object sender, EventArgs e)
    {
        isOn = true;
        HeaderLabel.Text = "Светофор включен";

        if (isNight) StartNightBlink();
        else StartDayCycle();
    }

    //выключить
    void OnValjaClicked(object sender, EventArgs e)
    {
        isOn = false;
        HeaderLabel.Text = "Светофор выключен";

        StopDayCycle();
        StopNightBlink();
        PaintOff();
    }

    // переключатель День/Ночь
    void OnNightToggled(object sender, ToggledEventArgs e)
    {
        isNight = e.Value;

        if (!isOn)
        {
            // просто обновим внешний вид
            PaintOff();
            HeaderLabel.Text = isNight ? "Ночной режим (выключено)" : "Дневной режим (выключено)";
            return;
        }

        // если включено — переключаем логику
        if (isNight)
        {
            StopDayCycle();
            StartNightBlink();
        }
        else
        {
            StopNightBlink();
            StartDayCycle();
        }
    }

    //ДНЕВНОЙ ЦИКЛ (красный жёлтый зелёный)

    void StartDayCycle()
    {
        dayTimer ??= Dispatcher.CreateTimer();
        dayTimer.Interval = TimeSpan.FromSeconds(1);
        dayTimer.Tick -= OnDayTick;
        dayTimer.Tick += OnDayTick;

        // начинаем с красного
        current = Phase.Red;
        secondsLeft = RED_SEC;
        PaintDayPhase();

        if (!dayTimer.IsRunning) dayTimer.Start();
        HeaderLabel.Text = "Дневной режим";
    }

    void StopDayCycle()
    {
        dayTimer?.Stop();
        ClearCountdowns();
    }

    void OnDayTick(object? sender, EventArgs e)
    {
        if (!isOn || isNight) return;

        secondsLeft--;
        UpdateCountdown();

        if (secondsLeft <= 0)
        {
            current = current switch
            {
                Phase.Red => Phase.Yellow,
                Phase.Yellow => Phase.Green,
                Phase.Green => Phase.Red,
                _ => Phase.Red
            };

            secondsLeft = current switch
            {
                Phase.Red => RED_SEC,
                Phase.Yellow => YELLOW_SEC,
                Phase.Green => GREEN_SEC,
                _ => 0
            };

            PaintDayPhase();
            UpdateCountdown();
        }
    }

    void PaintDayPhase()
    {
        // сначала гасим всё
        RedCircle.BackgroundColor = Colors.Gray;
        YellowCircle.BackgroundColor = Colors.Gray;
        GreenCircle.BackgroundColor = Colors.Gray;

        // подсвечиваем текущую фазу
        switch (current)
        {
            case Phase.Red: RedCircle.BackgroundColor = Colors.Red; break;
            case Phase.Yellow: YellowCircle.BackgroundColor = Colors.Yellow; break;
            case Phase.Green: GreenCircle.BackgroundColor = Colors.Green; break;
        }
    }

    //НОЧНОЙ РЕЖИМ (мигает жёлтый)

    bool yellowOn = false; // текущее состояние мигания

    void StartNightBlink()
    {
        // гасим красный/зелёный
        RedCircle.BackgroundColor = Colors.Gray;
        GreenCircle.BackgroundColor = Colors.Gray;
        ClearCountdowns();

        nightTimer ??= Dispatcher.CreateTimer();
        nightTimer.Interval = TimeSpan.FromSeconds(1); // период мигания
        nightTimer.Tick -= OnNightTick;
        nightTimer.Tick += OnNightTick;

        yellowOn = false; // начнём с выкл, на первом тике включим
        if (!nightTimer.IsRunning) nightTimer.Start();

        HeaderLabel.Text = "Ночной режим: мигает жёлтый";
    }

    void StopNightBlink()
    {
        nightTimer?.Stop();
        YellowCircle.BackgroundColor = Colors.Gray;
    }

    void OnNightTick(object? sender, EventArgs e)
    {
        if (!isOn || !isNight) return;

        yellowOn = !yellowOn;
        YellowCircle.BackgroundColor = yellowOn ? Colors.Yellow : Colors.Gray;
    }


    void PaintOff()
    {
        RedCircle.BackgroundColor = Colors.Gray;
        YellowCircle.BackgroundColor = Colors.Gray;
        GreenCircle.BackgroundColor = Colors.Gray;
        ClearCountdowns();
    }

    void UpdateCountdown()
    {
        ClearCountdowns();
        string t = secondsLeft.ToString();
        switch (current)
        {
            case Phase.Red: RedCountdown.Text = t; break;
            case Phase.Yellow: YellowCountdown.Text = t; break;
            case Phase.Green: GreenCountdown.Text = t; break;
        }
    }

    void ClearCountdowns()
    {
        RedCountdown.Text = "";
        YellowCountdown.Text = "";
        GreenCountdown.Text = "";
    }

    // Часы сверху
    void StartClock()
    {
        clockTimer ??= Dispatcher.CreateTimer();
        clockTimer.Interval = TimeSpan.FromSeconds(1);
        clockTimer.Tick += (_, __) => ClockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
        if (!clockTimer.IsRunning) clockTimer.Start();
    }
}
