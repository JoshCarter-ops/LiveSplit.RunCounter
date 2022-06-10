using LiveSplit.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace LiveSplit.UI.Components
{
    public class RunCounterComponent : IComponent
    {
        protected InfoTextComponent InternalComponent { get; set; }
        public RunCounterSettings Settings { get; set; }
        public GraphicsCache Cache { get; set; }
        protected LiveSplitState CurrentState { get; set; }
        public string ComponentName => "Run Counter";
        private int counterValue = 0;
        private string counterField = "xx Runs";

        public float HorizontalWidth => InternalComponent.HorizontalWidth;
        public float MinimumWidth => InternalComponent.MinimumWidth;
        public float VerticalHeight => InternalComponent.VerticalHeight;
        public float MinimumHeight => InternalComponent.MinimumHeight;

        public float PaddingTop => InternalComponent.PaddingTop;
        public float PaddingLeft => InternalComponent.PaddingLeft;
        public float PaddingBottom => InternalComponent.PaddingBottom;
        public float PaddingRight => InternalComponent.PaddingRight;

        public IDictionary<string, Action> ContextMenuControls => null;

        public RunCounterComponent(LiveSplitState state)
        {
            Settings = new RunCounterSettings();
            Cache = new GraphicsCache();
            InternalComponent = new InfoTextComponent("xx Runs", "0");
            CurrentState = state;
        }

        public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion)
        {
            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            InternalComponent.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = state.LayoutSettings.TextColor;

            InternalComponent.DrawHorizontal(g, state, height, clipRegion);
        }

        public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion)
        {
            InternalComponent.DisplayTwoRows = Settings.Display2Rows;

            InternalComponent.NameLabel.HasShadow
                = InternalComponent.ValueLabel.HasShadow
                = state.LayoutSettings.DropShadows;

            InternalComponent.NameLabel.ForeColor = state.LayoutSettings.TextColor;
            InternalComponent.ValueLabel.ForeColor = state.LayoutSettings.TextColor;

            InternalComponent.DrawVertical(g, state, width, clipRegion);
        }

        public Control GetSettingsControl(LayoutMode mode)
        {
            Settings.Mode = mode;
            return Settings;
        }

        public System.Xml.XmlNode GetSettings(System.Xml.XmlDocument document)
        {
            return Settings.GetSettings(document);
        }

        public void SetSettings(System.Xml.XmlNode settings)
        {
            Settings.SetSettings(settings);
        }

        public void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            string count = GetSimilarTimeCount(getPBTime()).ToString();
            InternalComponent.InformationName = counterField;
            InternalComponent.InformationValue = counterValue >= 0 ? count : "?";
            InternalComponent.Update(invalidator, state, width, height, mode);
        }

        public void Dispose()
        {

        }

        public int GetSettingsHashCode() => Settings.GetSettingsHashCode();


        // Lets get the PB Time to track
        private Attempt getPBTime()
        {
            var bestTime = TimeSpan.MaxValue;
            int bestIndex = 0;

            if (CurrentState.Run.AttemptHistory.Count <= 0)
            {
                Attempt TempAttempt = new Attempt();
                TempAttempt.Index = 0;
                return TempAttempt;
            }

            foreach (Attempt attempt in CurrentState.Run.AttemptHistory)
            {
                if (attempt.Time[CurrentState.CurrentTimingMethod].HasValue)
                {
                    if (attempt.Time[CurrentState.CurrentTimingMethod].Value < bestTime)
                    {
                        bestTime = attempt.Time[CurrentState.CurrentTimingMethod].Value;
                        bestIndex = attempt.Index;
                    }
                }
            }

            if (bestIndex == 0)
                return CurrentState.Run.AttemptHistory[0];
            return CurrentState.Run.AttemptHistory[bestIndex - 1];
        }

        private int GetSimilarTimeCount(Attempt pbAttempt)
        {
            int tempCounter = 0;
            string pbdays = pbAttempt.Duration.Value.Days.ToString(); // get the days (for really long pbs)
            string pbhour = pbAttempt.Duration.Value.Hours.ToString(); // get the hour value (for long pbs)
            string pbmin = pbAttempt.Duration.Value.Minutes.ToString(); // get the min value (for all pbs)

            counterField = (pbhour.Length.Equals(1) ? (pbhour.Equals("0") ? "" : "0" + pbhour + ":") : pbhour);
            counterField += (pbmin.Length.Equals(1) ? (pbmin.Equals("0") ? "" : "0" + pbmin) : pbmin);
            counterField += ":xx's";

            foreach (Attempt attempt in CurrentState.Run.AttemptHistory) {

                // get the attempts that match the same hour and minuite barriers so that any game can use this
                 string tempd = attempt.Duration.Value.Days.ToString();
                 string temph = attempt.Duration.Value.Hours.ToString();
                 string tempm = attempt.Duration.Value.Minutes.ToString();

                if (pbdays.Equals(tempd) && pbhour.Equals(temph) && pbmin.Equals(tempm))
                {
                    tempCounter++;
                }
            }

            return tempCounter;
        }
    }
}
