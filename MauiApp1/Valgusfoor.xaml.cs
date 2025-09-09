using Microsoft.Maui.Controls;

namespace MauiApp1;

public partial class Valgusfoor : ContentPage
{
    // включён ли светофор
    bool isOn = false;

    // выбран ли ночной режим
    bool isNight = false;

    // таймеры
    IDispatcherTimer? dayTimer;
    IDispatcherTimer? nightTimer;
    IDispatcherTimer? clockTimer;

    // фазы днём
    enum Phase { Red, Yellow, Green }
    Phase current = Phase.Red;

    // сколько секунд осталось
    int secondsLeft = 0;

    // длительность каждой фазы
    const int RED_SEC = 7;
    const int YELLOW_SEC = 2;
    const int GREEN_SEC = 6;

    public Valgusfoor()
    {
        InitializeComponent();

        StartClock(); // запускаю часы
        PaintOff();   // всё серое в начале
    }

    // ===== кнопки =====

    void OnSisseClicked(object sender, EventArgs e)
    {
        isOn = true;
        HeaderLabel.Text = "Светофор включен";

        if (isNight) StartNightBlink();
        else StartDayCycle();
    }

    void OnValjaClicked(object sender, EventArgs e)
    {
        isOn = false;
        HeaderLabel.Text = "Светофор выключен";

        StopDayCycle();
        StopNightBlink();
        PaintOff();
    }

    // ===== переключатель день/ночь =====

    void OnNightToggled(object sender, ToggledEventArgs e)
    {
        isNight = e.Value;

        if (!isOn)
        {
            PaintOff();
            HeaderLabel.Text = isNight ? "Ночной режим (выключено)" : "Дневной режим (выключено)";
            return;
        }

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

    // ===== дневной цикл =====

    void StartDayCycle()
    {
        if (dayTimer == null)
        {
            dayTimer = Dispatcher.CreateTimer();
        }

        dayTimer.Interval = TimeSpan.FromSeconds(1);
        dayTimer.Tick -= OnDayTick;
        dayTimer.Tick += OnDayTick;

        current = Phase.Red;
        secondsLeft = RED_SEC;
        PaintDayPhase();
        UpdateCountdown();

        if (!dayTimer.IsRunning)
            dayTimer.Start();

        HeaderLabel.Text = "Дневной режим";
    }

    void StopDayCycle()
    {
        if (dayTimer != null)
            dayTimer.Stop();

        ClearCountdowns();
    }

    void OnDayTick(object? sender, EventArgs e)
    {
        if (!isOn || isNight) return;

        secondsLeft--;
        UpdateCountdown();

        if (secondsLeft <= 0)
        {
            if (current == Phase.Red) current = Phase.Yellow;
            else if (current == Phase.Yellow) current = Phase.Green;
            else current = Phase.Red;

            if (current == Phase.Red) secondsLeft = RED_SEC;
            else if (current == Phase.Yellow) secondsLeft = YELLOW_SEC;
            else if (current == Phase.Green) secondsLeft = GREEN_SEC;

            PaintDayPhase();
            UpdateCountdown();
        }
    }

    void PaintDayPhase()
    {
        // всё серое
        RedCircle.BackgroundColor = Colors.Gray;
        YellowCircle.BackgroundColor = Colors.Gray;
        GreenCircle.BackgroundColor = Colors.Gray;

        // рамки выключаю
        RedCircle.BorderColor = Colors.Transparent;
        YellowCircle.BorderColor = Colors.Transparent;
        GreenCircle.BorderColor = Colors.Transparent;

        // активный круг подсвечиваю
        if (current == Phase.Red)
        {
            RedCircle.BackgroundColor = Colors.Red;
            RedCircle.BorderColor = Colors.White;
        }
        else if (current == Phase.Yellow)
        {
            YellowCircle.BackgroundColor = Colors.Yellow;
            YellowCircle.BorderColor = Colors.White;
        }
        else if (current == Phase.Green)
        {
            GreenCircle.BackgroundColor = Colors.Green;
            GreenCircle.BorderColor = Colors.White;
        }
    }

    // ===== ночной режим =====

    bool yellowOn = false;

    void StartNightBlink()
    {
        // красный и зелёный выключаю
        RedCircle.BackgroundColor = Colors.Gray;
        GreenCircle.BackgroundColor = Colors.Gray;
        RedCircle.BorderColor = Colors.Transparent;
        GreenCircle.BorderColor = Colors.Transparent;

        ClearCountdowns();

        if (nightTimer == null)
        {
            nightTimer = Dispatcher.CreateTimer();
        }

        nightTimer.Interval = TimeSpan.FromSeconds(1);
        nightTimer.Tick -= OnNightTick;
        nightTimer.Tick += OnNightTick;

        yellowOn = false;

        if (!nightTimer.IsRunning)
            nightTimer.Start();

        HeaderLabel.Text = "Ночной режим: мигает жёлтый";
    }

    void StopNightBlink()
    {
        if (nightTimer != null)
            nightTimer.Stop();

        YellowCircle.BackgroundColor = Colors.Gray;
        YellowCircle.BorderColor = Colors.Transparent;
    }

    void OnNightTick(object? sender, EventArgs e)
    {
        if (!isOn || !isNight) return;

        yellowOn = !yellowOn;

        if (yellowOn)
        {
            YellowCircle.BackgroundColor = Colors.Yellow;
            YellowCircle.BorderColor = Colors.White;
        }
        else
        {
            YellowCircle.BackgroundColor = Colors.Gray;
            YellowCircle.BorderColor = Colors.Transparent;
        }
    }

    // ===== вспомогательные методы =====

    void PaintOff()
    {
        RedCircle.BackgroundColor = Colors.Gray;
        YellowCircle.BackgroundColor = Colors.Gray;
        GreenCircle.BackgroundColor = Colors.Gray;

        RedCircle.BorderColor = Colors.Transparent;
        YellowCircle.BorderColor = Colors.Transparent;
        GreenCircle.BorderColor = Colors.Transparent;

        ClearCountdowns();
    }

    void UpdateCountdown()
    {
        ClearCountdowns();

        string t = secondsLeft.ToString();

        if (current == Phase.Red) RedCountdown.Text = t;
        else if (current == Phase.Yellow) YellowCountdown.Text = t;
        else if (current == Phase.Green) GreenCountdown.Text = t;
    }

    void ClearCountdowns()
    {
        RedCountdown.Text = "";
        YellowCountdown.Text = "";
        GreenCountdown.Text = "";
    }

    void StartClock()
    {
        if (clockTimer == null)
        {
            clockTimer = Dispatcher.CreateTimer();
        }

        clockTimer.Interval = TimeSpan.FromSeconds(1);
        clockTimer.Tick += (_, __) =>
        {
            ClockLabel.Text = DateTime.Now.ToString("HH:mm:ss");
        };

        if (!clockTimer.IsRunning)
            clockTimer.Start();
    }
}
