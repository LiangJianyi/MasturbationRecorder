using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace MasturbationRecorder {
    static class Blink {
        private static SortedDictionary<string, Storyboard> _rectangleRegister = new SortedDictionary<string, Storyboard>();

        public static void PlayBlink(Rectangle target) {
            if (!_rectangleRegister.ContainsKey(target.Name)) {
                _rectangleRegister.Add(target.Name, new Storyboard());
                KeyTime redTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 300));
                KeyTime orginalTime = KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 600));

                ColorAnimationUsingKeyFrames colorAnimationUsingKeyFrames = new ColorAnimationUsingKeyFrames() {
                    EnableDependentAnimation = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };
                DiscreteColorKeyFrame OrginalColorToRed = new DiscreteColorKeyFrame() {
                    Value = Windows.UI.Colors.Red,
                    KeyTime = redTime
                };
                DiscreteColorKeyFrame RedToOrginalColor = new DiscreteColorKeyFrame() {
                    Value = (target.Fill as SolidColorBrush).Color,
                    KeyTime = orginalTime
                };

                colorAnimationUsingKeyFrames.KeyFrames.Add(OrginalColorToRed);
                colorAnimationUsingKeyFrames.KeyFrames.Add(RedToOrginalColor);
                _rectangleRegister[target.Name].Children.Add(colorAnimationUsingKeyFrames);
                Storyboard.SetTarget(colorAnimationUsingKeyFrames, target);
                Storyboard.SetTargetName(colorAnimationUsingKeyFrames, target.Name);
                Storyboard.SetTargetProperty(colorAnimationUsingKeyFrames, "(Rectangle.Fill).(SolidColorBrush.Color)");

                _rectangleRegister[target.Name].Begin();
            }
        }

        public static void StopBlink(Rectangle target) {
            _rectangleRegister[target.Name].Stop();
        }
    }
}
