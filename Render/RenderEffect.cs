using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace UISEditor.Render
{
    public class ScreenEffect : ShaderEffect
    {
        private static string asm = typeof(ScreenEffect).Assembly.ToString().Split(new[] { ',' })[0];
        private static PixelShader mPixelShader = new PixelShader() { UriSource = new Uri($"pack://application:,,,/{asm};component/Resource/screen.fx.ps") };
        public ScreenEffect()
        {
            PixelShader = mPixelShader;
            UpdateShaderValue(InputProperty);
        }

        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }
        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(ScreenEffect), 0);

    }
}
